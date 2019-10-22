using Chop9ja.API.Data;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.Responses;
using Chop9ja.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Chop9ja.API.Controllers
{
    [Route("api/webhooks")]
    [ApiController]
    [AutoBuild]
    public class WebHookController : ControllerBase
    {
        #region Properties

        #region Services
        [DeepDependency]
        MongoDataContext DataContext { get; }

        [DeepDependency]
        IPaymentService PaymentService { get; }

        [DeepDependency]
        ILogger Logger { get; }
        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Webhook for paystack. 
        /// This method is only for calls from paystack.
        /// </summary>
        /// <param name="response"></param>
        /// <returns>An OK Response</returns>
        [HttpPost("paystack")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OkResult))]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(string), Description = "An unexpected error occured.")]
        public async Task<IActionResult> Paystack([FromBody]PaystackTransactionResponse response)
        {
            if (response == null)
            {
                Core.Log.Error("An error occured while using paystack. The response seems to be null!");
                Core.Log.Debug("Attempting to output request body.");

                try
                {
                    Request.Body.Seek(0, SeekOrigin.Begin);

                    using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                        Core.Log.Debug(await reader.ReadToEndAsync());
                }
                catch (Exception ex)
                {
                    Core.Log.Error($"An error occured while attempting to output a Paystack Request Body\n{ex}");
                }
                
                return BadRequest("Response was invalid");
            }

            // Make sure the webhook is calling to report success
            if (response.Event.ToLower() != PaymentService.PaystackOptions.TransactionEvent)
                return BadRequest("Unknown Paystack Event");

            Request.Body.Seek(0, SeekOrigin.Begin);

            string request = string.Empty;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                request = await reader.ReadToEndAsync();

            string signature = Request.Headers["x-paystack-signature"].ToString();

            bool success = await PaymentService.ValidatePaystackSignature(signature, request);
            if (!success) return BadRequest("Paystack Signature appears to be invalid");

            success = Guid.TryParse(response.Data.Reference, out Guid id);
            if (!success) return BadRequest("Invalid Reference ID");

            success = await PaymentService.ConcludePaystack(response.Data);
            if (!success) return BadRequest("An unexpected error occured during payment conclusion.");

            return Ok();
        }

        #endregion
    }
}

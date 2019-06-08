using Chop9ja.API.Extensions;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.ViewModels;
using Chop9ja.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Chop9ja.API.Controllers
{
    /// <summary>
    /// Handles API Messaging
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [AutoBuild]
    public class MessengerController : ControllerBase
    {
        #region Properties

        #region Services
        [DeepDependency]
        IEmailService EmailService { get; }
        [DeepDependency]
        ISmsService SmsService { get; }
        [DeepDependency]
        PhoneNumberUtil Phone { get; }
        #endregion

        #endregion

        #region Methods 
        /// <summary>
        /// Send an email to a user.
        /// </summary>
        /// <param name="model">Email object wrapper.</param>
        /// <returns></returns>
        [HttpPost("email")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OkObjectResult), 
            Description = "Email was sent successfully")]
        public async Task<IActionResult> SendEmail([FromBody]CustomEmailViewModel model)
        {
            bool success = await EmailService.SendEmailAsync(model.To, 
                model.Subject, model.Body);

            if (!success) return BadRequest();
            return Ok();
        }

        /// <summary>
        /// Send a text message to a user.
        /// </summary>
        /// <param name="model">SMS object wrapper.</param>
        /// <returns></returns>
        [HttpPost("sms")]
        public async Task<IActionResult> SendSMS([FromBody]CustomSMSViewModel model)
        {
            bool success = Phone.TryParse(model.Phone, Core.REGION, out PhoneNumber phone);
            if (!success) return BadRequest("The inputed phone number was invalid");
            if (!Phone.IsValidNumber(phone)) return BadRequest("The inputed phone number was invalid");

            success = await SmsService.SendMessage(model.Phone, model.Message);
            return Ok();
        }

        /// <summary>
        /// Gets the remaining SMS balance.
        /// </summary>
        /// <returns>The value of the remaining sms balance in units.</returns>
        [HttpGet("smsbalance")]
        public async Task<IActionResult> GetSmsBalance() => Ok(await SmsService.GetBalance());
        #endregion
    }
}

using AutoMapper;
using Chop9ja.API.Data;
using Chop9ja.API.Extensions.Attributes;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Chop9ja.API.Controllers.BackOffice
{
    [AutoBuild]
    [ApiController]
    [Route("api/backOffice/[controller]")]
    [AuthorizeRoles(UserRoles.Administrator)]
    public class ManagementController : ControllerBase
    {
        #region Properties

        #region Services

        [DeepDependency]
        ILogger Logger { get; }

        [DeepDependency]
        UserManager<User> UserManager { get; }

        [DeepDependency]
        MongoDataContext DataContext { get; }

        [DeepDependency]
        IMapper Mapper { get; }

        #endregion

        #endregion

        #region Methods

        #region RESTful API Calls

        /// <summary>
        /// Get the list of payment channels in the platform
        /// </summary>
        [HttpGet("paymentChannels")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<PaymentChannelViewModel>), Description = "List of available payment methods")]
        public async Task<IActionResult> GetPaymentChannels()
        {
            var channels = await DataContext.Store.GetAllAsync<PaymentChannel>(p => p.Type != ChannelType.Chop9ja);
            return Ok(Mapper.Map<IEnumerable<FullPaymentChannelViewModel>>(channels));
        }

        /// <summary>
        /// Activate or deactivate a payment channel
        /// </summary>
        [HttpPost("paymentChannels/update")]
        public async Task<IActionResult> UpdatePaymentChannel([FromBody]ChannelUpdateViewModel model)
        {
            PaymentChannel channel = await DataContext.Store.GetOneAsync<PaymentChannel>(c => c.Type == model.Type);

            if (channel == null) return NotFound();

            if (channel.IsActive == model.IsActive) return Ok();

            channel.IsActive = model.IsActive;
            await DataContext.Store.UpdateOneAsync(channel);

            return Ok();
        }

        /// <summary>
        /// Get list of the platform's bank accounts
        /// </summary>
        [HttpGet("bankAccounts")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<UserBankAccountViewModel>), Description = "List of available bank accounts for deposit")]
        public IActionResult GetPlatformBankAccounts()
        {
            return Ok(Mapper.Map<IEnumerable<UserBankAccountViewModel>>(DataContext.PlatformAccount.BankAccounts.Where(p => p.IsActive)));
        }

        
        /// <summary>
        /// Add an account number to the platform
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("bankAccounts/add")]
        public async Task<IActionResult> AddPlatformBankAccount([FromBody]BankAccountViewModel model)
        {
            User user = DataContext.PlatformAccount;

            BankAccount account = Mapper.Map<BankAccount>(model);

            BankAccount clone = user.BankAccounts.FirstOrDefault(b =>
            {
                return b.BankId == model.BankId
                && b.AccountNumber == model.AccountNumber;
            });

            if (clone != null)
            {
                int index = user.BankAccounts.IndexOf(clone);
                clone.IsActive = true;
                user.BankAccounts[index] = account;
                await DataContext.Store.UpdateOneAsync(user);
                return Ok();
            }

            user.BankAccounts.Add(account);
            await DataContext.Store.UpdateOneAsync(user);

            return Ok();
        }

        /// <summary>
        /// Delete a bank account from the platform
        /// </summary>
        /// <returns>An OK Response</returns>
        [HttpPost("bankAccounts/remove")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OkResult), Description = "Bank account was successfully removed")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(UnauthorizedResult), Description = "User was not found or the user credentials were invalid.")]
        public async Task<IActionResult> RemoveBankAccount([FromBody]BankAccountIdViewModel model)
        {
            User user = DataContext.PlatformAccount;

            BankAccount account = user.BankAccounts.FirstOrDefault(b => b.Id == Guid.Parse(model.Id));

            if (account == null) return NotFound($"No Bank Account exists with id {model.Id}");

            account.IsActive = false;

            bool success = await DataContext.Store.UpdateOneAsync(user);
            return Ok();
        }



        #endregion

        #endregion
    }
}

﻿using AutoMapper;
using Chop9ja.API.Data;
using Chop9ja.API.Extensions;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.ViewModels;
using Chop9ja.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Chop9ja.API.Controllers
{
    /// <summary>
    /// Manages user wallet details
    /// </summary>
    [Authorize]
    [Route("api/account/[controller]")]
    [ApiController]
    [AutoBuild]
    public class WalletController : ControllerBase
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

        [DeepDependency]
        IPaymentService PaymentService { get; }

        [DeepDependency]
        IHostingEnvironment Env { get; }
        #endregion

        #endregion

        #region Methods

        #region RESTful API Calls
        /// <summary>
        /// Get Wallet Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(WalletViewModel), Description = "User wallet balance and transactions.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(UnauthorizedResult), Description = "Invalid User Credentials.")]
        public async Task<IActionResult> GetWallet()
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);

            if (user == null) return Unauthorized();

            var view = Mapper.Map<WalletViewModel>(user.Wallet);
            
            return Ok(view);
        }

        /// <summary>
        /// Get list of payment methods.
        /// </summary>
        [HttpGet("deposit/paymentChannels")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<PaymentChannelViewModel>), Description = "List of available payment methods")]
        public async Task<IActionResult> GetPaymentChannels()
        {
            var channels = await DataContext.Store.GetAllAsync<PaymentChannel>(p => true);
            return Ok(Mapper.Map<IEnumerable<PaymentChannelViewModel>>(channels));
        }

        /// <summary>
        /// Get list of bank accounts for deposit
        /// </summary>
        [HttpGet("deposit/platformBankAccounts")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<UserBankAccountViewModel>), Description = "List of available bank accounts for deposit")]
        public IActionResult GetPlatformBankAccounts()
        {
            return Ok(Mapper.Map<IEnumerable<UserBankAccountViewModel>>(DataContext.PlatformAccount.BankAccounts));
        }

        /// <summary>
        /// Make a deposit
        /// </summary>
        /// <returns></returns>
        [HttpPost("deposit")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OkResult), Description = "Deposit was automatic and successful")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(UnauthorizedResult), Description = "Invalid User Credentials.")]
        [SwaggerResponse(HttpStatusCode.Accepted, typeof(AcceptedResult), Description = "Transaction has been created. User should be redirected to the returned url.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(BadRequestObjectResult), Description = "Deposit was automatic and successful")]
        public async Task<IActionResult> Deposit([FromBody]DepositViewModel model)
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);

            if (user == null) return Unauthorized();

            PaymentResult result = null;
            switch (model.PaymentChannel)
            {
                case ChannelType.Paystack:
                    if (!Env.IsDevelopment())
                    {
                        string baseUrl = new Uri(HttpContext.Request.GetEncodedUrl()).GetBaseUrl();
                        Core.ONLINE_BASE_ADDRESS = baseUrl;
                    }

                    result = await PaymentService.UsePaystack(user, model.Amount);
                    break;

                case ChannelType.Bank:
                    BankAccount account = user.BankAccounts.FirstOrDefault(b => b.IsActive && b.Id == model.UserBankAccountId);

                    if (account == null) return NotFound("Invalid User Account Id");

                    BankAccount platformAccount = DataContext.PlatformAccount.BankAccounts.FirstOrDefault(b => b.IsActive && b.Id == model.PlatformBankAccountId);

                    if (platformAccount == null) return NotFound("Invalid Platform Account Id");

                    result = await PaymentService.UseBank(user, model.Amount, account, platformAccount);
                    break;
            }

            switch (result.Status)
            {
                case PaymentStatus.Redirected:
                    return Accepted(result.Message, result.Message);
                case PaymentStatus.Success:
                    return Ok();
                case PaymentStatus.Pending:
                    return Accepted();

                default: return BadRequest(result.Message);
            }
        }

        /// <summary>
        /// Withdraw cash
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("withdraw")]
        public async Task<IActionResult> WithdrawFunds([FromBody]WithdrawalViewModel model)
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);

            if (user == null) return Unauthorized();

            if (model.Amount > user.Wallet.AvailableBalance) return BadRequest("Insufficient Funds");

            BankAccount account = user.BankAccounts.FirstOrDefault(b => b.Id == model.BankAccountId);

            if (account == null) return NotFound("The provided bank account could not be located for this user.");

            await PaymentService.BankWithdrawal(user, model.Amount, account);

            return Ok();
        }
        #endregion

        #endregion
    }
}

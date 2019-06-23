using AutoMapper;
using Chop9ja.API.Data;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.ViewModels;
using Chop9ja.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        /// Make a deposit
        /// </summary>
        /// <returns></returns>
        [HttpPost("deposit")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OkResult), Description = "Deposit was automatic and successful")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(UnauthorizedResult), Description = "Invalid User Credentials.")]
        [SwaggerResponse(HttpStatusCode.Accepted, typeof(RedirectResult), Description = "Transaction has been created. User should be redirected to paystack")]
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
                    result = await PaymentService.UsePaystack(user, model.Amount);
                    break;
            }

            switch (result.Status)
            {
                case PaymentStatus.Redirected:
                    return Ok(result.Message);
                case PaymentStatus.Success:
                    return Ok();

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

            if (model.Amount > user.Wallet.Balance) return BadRequest("Insufficient Funds");

            PaymentChannel channel = DataContext.Store.GetOne<PaymentChannel>(p => p.Type == ChannelType.BankDeposit);

            // TODO: Create a claim
            Transaction transaction = new Transaction()
            {
                Amount = model.Amount,
                PaymentChannel = channel,
                Type = TransactionType.Withdrawal
            };

            await user.Wallet.AddTransactionAsync(transaction);

            return Ok();
        }
        #endregion

        #endregion
    }
}

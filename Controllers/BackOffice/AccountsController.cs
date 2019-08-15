using AutoMapper;
using Chop9ja.API.Data;
using Chop9ja.API.Extensions;
using Chop9ja.API.Extensions.Attributes;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Controllers.BackOffice
{
    [AutoBuild]
    [ApiController]
    [Route("api/backOffice/[controller]")]
    [AuthorizeRoles(UserRoles.Administrator)]
    public class AccountsController : ControllerBase
    {
        #region Properties

        #region Services
        [DeepDependency]
        ILogger Logger { get; }

        [DeepDependency]
        UserManager<User> UserManager { get; }

        [DeepDependency]
        RoleManager<UserRole> RoleManager { get; }

        [DeepDependency]
        MongoDataContext DataContext { get; }

        [DeepDependency]
        IMapper Mapper { get; }
        #endregion

        #endregion

        #region Methods

        #region RESTful API Calls
        [HttpGet]
        public async Task<IActionResult> GetAccounts([FromQuery]DateSearchViewModel model)
        {
            bool search = !string.IsNullOrWhiteSpace(model.SearchQuery);


            DateRange range = new DateRange(model.Start, model.End);
            var filter = range.IncludesUser();

            if (search)
            {
                string query = model.SearchQuery.ToLower();
                filter = filter.CombineWithAndAlso(u => 
                    u.FirstName.ToLower().Contains(query) ||
                    u.LastName.ToLower().Contains(query) ||
                    u.UserName.ToLower().Contains(query));
            }

            IEnumerable<User> accounts = await DataContext.Store.GetAllAsync(filter);
                
            List<User> regularAccounts = new List<User>();

            foreach (var account in accounts)
                if (await UserManager.IsInRoleAsync(account, UserRoles.Regular.ToString()))
                    regularAccounts.Add(account);
            
            return Ok(Mapper.Map<IEnumerable<AccountViewModel>>(regularAccounts));
        }

        [HttpGet("claims")]
        public async Task<IActionResult> GetClaims([FromQuery]DateSearchViewModel model)
        {
            bool search = !string.IsNullOrWhiteSpace(model.SearchQuery);

            DateRange range = new DateRange(model.Start, model.End);
            var filter = range.Includes<Bet>().CombineWithAndAlso(b => b.CashOutRequested);

            if (search)
            {
                string query = model.SearchQuery.ToLower();
                filter = filter.CombineWithAndAlso(c =>
                    c.SlipNumber.ToLower().Contains(query));
            }

            IEnumerable<Bet> claims = await DataContext.Store.GetAllAsync(filter);

            return Ok(Mapper.Map<IEnumerable<BackOfficeClaimViewModel>>(claims));
        }

        [HttpPost("claims/update")]
        public async Task<IActionResult> UpdateClaim(BackOfficeClaimUpdateViewModel model)
        {
            if (!Guid.TryParse(model.Id, out Guid claimId)) return NotFound();

            Bet bet = await DataContext.Store.GetByIdAsync<Bet>(claimId);

            if (bet == null) return NotFound();

            bet.CashOutStatus = model.Status;

            await DataContext.Store.UpdateOneAsync(bet);

            return Ok();
        }


        /*
        [HttpPost("withdrawalRequests")]
        public async Task<IActionResult> GetWithdrawalRequests([FromBody]string userId)
        {
            User account = await UserManager.FindByIdAsync(userId);

            if (account == null) return NotFound("The specified user account was not found.");

            //return Ok(Mapper.Ma)
            //TODO: Finish this.
            return Ok();
        }

        [HttpPost("updateRequests")]
        public async Task<IActionResult> UpdatePaymentRequest([FromBody]PaymentRequestUpdateViewModel model)
        {
            PaymentRequest request = await DataContext.Store.GetByIdAsync<PaymentRequest>(model.Id);

            if (request == null) return NotFound("The specified payment request was not found");


            request.Status = model.Status;
            await DataContext.Store.UpdateOneAsync(request);

            return Ok(request);
        }
        */

        [HttpPost("banAccount")]
        public async Task<IActionResult> LockoutUser([FromBody]string userId)
        {
            User account = await UserManager.FindByIdAsync(userId);

            if (account == null) return NotFound("The specified user account was not found.");
            if (account.LockoutEnabled) return Ok();

            account.LockoutEnabled = true;
            await DataContext.Store.UpdateOneAsync(account);

            return Ok();
        }

        [HttpPost("unbanAccount")]
        public async Task<IActionResult> UnBlockUser([FromBody]string userId)
        {
            User account = await UserManager.FindByIdAsync(userId);

            if (account == null) return NotFound("The specified user account was not found.");
            if (!account.LockoutEnabled) return Ok();

            account.LockoutEnabled = false;
            await DataContext.Store.UpdateOneAsync(account);

            return Ok();
        }
        #endregion

        #endregion
    }
}

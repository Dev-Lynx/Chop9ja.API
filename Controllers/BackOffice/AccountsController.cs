using AutoMapper;
using Chop9ja.API.Data;
using Chop9ja.API.Extensions;
using Chop9ja.API.Extensions.Attributes;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.ViewModels;
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
    //[AuthorizeRoles(UserRoles.Agent, UserRoles.Administrator)]
    [Authorize]
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
        public async Task<IActionResult> GetAccounts()
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);

            if (user == null || !(await UserManager.IsInRoleAsync(user, UserRoles.Administrator.ToString()))) return Unauthorized();

            IEnumerable<User> accounts = await UserManager.GetUsersInRoleAsync(UserRoles.Regular.ToString());
            
            return Ok(Mapper.Map<IEnumerable<AccountViewModel>>(accounts));
        }

        [HttpGet("claims")]
        public async Task<IActionResult> GetClaims([FromQuery]SearchViewModel model)
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);

            if (user == null || !(await UserManager.IsInRoleAsync(user, UserRoles.Administrator.ToString()))) return Unauthorized();

            IEnumerable<Bet> claims = await DataContext.Store
                .GetAllAsync(model.DateRange.Includes<Bet>()
                .CombineWithAndAlso(b => b.CashOutRequested));

            return Ok(Mapper.Map<IEnumerable<BackOfficeClaimViewModel>>(claims));
        }

        [HttpPost("claims/update")]
        public async Task<IActionResult> UpdateClaim(BackOfficeClaimUpdateViewModel model)
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);

            if (user == null || !(await UserManager.IsInRoleAsync(user, UserRoles.Administrator.ToString()))) return Unauthorized();

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

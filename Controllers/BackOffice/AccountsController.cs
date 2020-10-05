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
using Newtonsoft.Json;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Controllers.BackOffice
{
    [AutoBuild]
    [ApiController]
    [Route("api/backOffice/[controller]")]
    [AuthorizeRoles(UserRoles.Administrator, UserRoles.Agent)]
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

        [DeepDependency]
        ISieveProcessor Sieve { get; }
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
                    u.UserName.ToLower().Contains(query) || 
                    u.Email.ToLower().Contains(query));
            }

            IEnumerable<User> accounts = await DataContext.Store.GetAllAsync(filter);
                
            List<User> regularAccounts = new List<User>();

            foreach (var account in accounts)
                if (await UserManager.IsInRoleAsync(account, UserRoles.Regular.ToString()))
                    regularAccounts.Add(account);
            
            return Ok(Mapper.Map<IEnumerable<AccountViewModel>>(regularAccounts));
        }

        [HttpGet("single")]
        public async Task<IActionResult> GetAccount([FromQuery]DocumentIdViewModel model)
        {
            User user = await UserManager.FindByIdAsync(model.Id);

            if (user == null) return NotFound();

            return Ok(Mapper.Map<FullUserViewModel>(user));
        }

        [HttpGet("single/transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery]DocumentIdViewModel model)
        {
            User user = await UserManager.FindByIdAsync(model.Id);

            if (user == null) return NotFound();

            var transactions = await user.Wallet.GetTransactions();

            return Ok(Mapper.Map<IEnumerable<TransactionViewModel>>(transactions));
        }

        [HttpGet("bets")]
        public async Task<IActionResult> GetBets([FromQuery]DateRangeSieveModel model)
        {
            bool search = !string.IsNullOrWhiteSpace(model.SearchQuery);

            DateRange range = new DateRange(model.Start, model.End);
            var filter = range.Includes<Bet>();

            if (search)
            {
                string query = model.SearchQuery.ToLower();

                // Also search using platform
                string platformPath = Path.Combine(Core.DATA_DIR, "betPlatforms.json");
                string json = await System.IO.File.ReadAllTextAsync(platformPath);
                var platforms = JsonConvert.DeserializeObject<List<BetPlatform>>(json)
                    .Where(p => p.Name.ToLower().Contains(query));

                filter = filter.And(c =>
                    c.SlipNumber.ToLower().Contains(query));

                foreach (var platform in platforms)
                    filter = filter.Or(c => c.PlatformId == platform.Id);
            }

            IEnumerable<Bet> claims = await DataContext.Store.GetAllAsync(filter);

            var data = Sieve.Apply(model, Mapper.Map<IEnumerable<BackOfficeClaimViewModel>>(claims).AsQueryable());

            return Ok(data);
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
        [AuthorizeRoles(UserRoles.Administrator)]
        public async Task<IActionResult> UpdateClaim(StatusUpdateViewModel model)
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
        [AuthorizeRoles(UserRoles.Administrator)]
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
        [AuthorizeRoles(UserRoles.Administrator)]
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

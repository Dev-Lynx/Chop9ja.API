using AutoMapper;
using Chop9ja.API.Data;
using Chop9ja.API.Extensions;
using Chop9ja.API.Extensions.Attributes;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.ViewModels;
using Google.Apis.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Controllers.Admin
{
    [AuthorizeRoles(nameof(UserRoles.Administrator))]
    [Route("api/admin/[controller]")]
    [ApiController]
    [AutoBuild]
    public class OverviewController : ControllerBase
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
        /*
        public async Task<IActionResult> GetOverview()
        {

        }
        */

        [HttpGet("registrations/count")]
        public async Task<IActionResult> CountRegistrations([FromQuery]DateRange dateRange)
        {
            dateRange = dateRange.ToLocalTime();

            long count = await DataContext.Store.
                CountAsync<User>(u => dateRange.Includes(u.CreatedOn.ToLocalTime()));

            return Ok(count);
        }

        [HttpGet("registrations/list")]
        public async Task<IActionResult> GetRegistrations([FromQuery]DateRange dateRange)
        {
            dateRange = dateRange.ToLocalTime();
            
            var users = await DataContext.Store.
                GetAllAsync<User>(u => dateRange.Includes(u.CreatedOn.ToLocalTime()));

            return Ok(Mapper.Map<IEnumerable<UserViewModel>>(users));
        }

        [HttpGet("bets/count")]
        public async Task<IActionResult> CountBets([FromQuery]DateRange dateRange)
        {
            dateRange = dateRange.ToLocalTime();

            long count = await DataContext.Store.
                CountAsync<Bet>(b => dateRange.Includes(b.AddedAtUtc.ToLocalTime()));

            return Ok(count);
        }

        [HttpGet("bets/list")]
        public async Task<IActionResult> GetBets([FromQuery]DateRange dateRange)
        {
            dateRange = dateRange.ToLocalTime();

            var bets = await DataContext.Store.
                GetAllAsync<Bet>(b => dateRange.Includes(b.AddedAtUtc.ToLocalTime()));

            return Ok(Mapper.Map<IEnumerable<Bet>>(bets));
        }

        [HttpGet("transactions/withdrawals/count")]
        public async Task<IActionResult> GetWithdrawals([FromQuery]DateRange dateRange)
        {
            dateRange = dateRange.ToLocalTime();

            var withdrawals = await DataContext.PlatformAccount.Wallet.
                GetTransactions(t => t.Type == TransactionType.Withdrawal 
                && dateRange.Includes(t.AddedAtUtc.ToLocalTime()));

            // TODO: Improve this output
            return Ok(Mapper.Map<IEnumerable<TransactionViewModel>>(withdrawals));
        }

        #endregion

        #endregion
    }
}

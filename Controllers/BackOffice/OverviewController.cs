﻿using AutoMapper;
using Chop9ja.API.Data;
using Chop9ja.API.Extensions;
using Chop9ja.API.Extensions.Attributes;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Controllers.BackOffice
{
    [AuthorizeRoles(nameof(UserRoles.Administrator))]
    [Route("api/backOffice/[controller]")]
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

        [HttpGet("registrations/count")]
        public async Task<IActionResult> GetRegisterationStats([FromQuery]DateRange dateRange)
        {
            if (dateRange.AutoGenerated) dateRange = DateRange.AllTime;
            else dateRange = dateRange.ToLocalTime();

            long count = await DataContext.Store.
                CountAsync<User>(dateRange.IncludesUser());

            TimeSpan timeDifference = dateRange.GetDifference();

            DateRange previousRange = new DateRange(dateRange.Start.TrySubtract(timeDifference), dateRange.End.TrySubtract(timeDifference));

            long previousCount = await DataContext.Store.
                CountAsync<User>(previousRange.IncludesUser());

            double difference = Calc.GetPercentageDifference(count, previousCount);

            return Ok(new StatsViewModel(count, previousCount, difference));   
        }

        [HttpGet("transactions/count")]
        public async Task<IActionResult> GetTransactionsStats([FromQuery]DateRange dateRange)
        {
            if (dateRange.AutoGenerated) dateRange = DateRange.AllTime;
            else dateRange = dateRange.ToLocalTime();

            var transactions = await DataContext.Store.GetAllAsync(dateRange.Includes<Transaction>());

            decimal total = 0;
            foreach (var transaction in transactions)
                switch (transaction.Type)
                {
                    case TransactionType.Deposit:
                    case TransactionType.Credit:
                        total += transaction.Amount;
                        break;

                    case TransactionType.Withdrawal:
                        total -= transaction.Amount;
                        break;
                }


            TimeSpan timeDifference = dateRange.GetDifference();
            DateRange previousRange = new DateRange(dateRange.Start.TrySubtract(timeDifference), dateRange.End.TrySubtract(timeDifference));

            var previousTransactions = await DataContext.Store.GetAllAsync(dateRange.Includes<Transaction>());

            decimal previousTotal = 0;
            foreach (var transaction in previousTransactions)
                switch (transaction.Type)
                {
                    case TransactionType.Deposit:
                    case TransactionType.Credit:
                        previousTotal += transaction.Amount;
                        break;

                    case TransactionType.Withdrawal:
                        previousTotal -= transaction.Amount;
                        break;
                }

            double difference = Calc.GetPercentageDifference((double)total, (double)previousTotal);

            return Ok(new TransactionStatsViewModel
            {
                Value = total,
                PreviousValue = previousTotal,
                PercentageDifference = difference,
                Transactions = Mapper.Map<List<BackOfficeTransactionViewModel>>(transactions),
            });
        }

        [HttpGet("bets/count")]
        public async Task<IActionResult> GetBetStats([FromQuery]DateRange dateRange)
        {
            if (dateRange.AutoGenerated) dateRange = DateRange.AllTime;
            else dateRange = dateRange.ToLocalTime();

            var filter = dateRange.Includes<Bet>().CombineWithAndAlso(b => !b.CashOutRequested);
            long count = await DataContext.Store.CountAsync(filter);

            TimeSpan timeDifference = dateRange.GetDifference();
            DateRange previousRange = new DateRange(dateRange.Start.TrySubtract(timeDifference), dateRange.End.TrySubtract(timeDifference));

            var previousFilter = previousRange.Includes<Bet>().CombineWithAndAlso(b => !b.CashOutRequested);
            long previousCount = await DataContext.Store.CountAsync(previousFilter);

            double difference = Calc.GetPercentageDifference(count, previousCount);

            return Ok(new StatsViewModel(count, previousCount, difference));
        }

        [HttpGet("bets/claimed/count")]
        public async Task<IActionResult> GetClaimedBetStats([FromQuery]DateRange dateRange)
        {
            if (dateRange.AutoGenerated) dateRange = DateRange.AllTime;
            else dateRange = dateRange.ToLocalTime();

            var filter = dateRange.Includes<Bet>().CombineWithAndAlso(b => b.CashOutRequested);
            long count = await DataContext.Store.CountAsync(filter);

            TimeSpan timeDifference = dateRange.GetDifference();
            DateRange previousRange = new DateRange(dateRange.Start.TrySubtract(timeDifference), dateRange.End.TrySubtract(timeDifference));

            var previousFilter = previousRange.Includes<Bet>().CombineWithAndAlso(b => !b.CashOutRequested);
            long previousCount = await DataContext.Store.CountAsync(previousFilter);

            double difference = Calc.GetPercentageDifference(count, previousCount);

            return Ok(new StatsViewModel(count, previousCount, difference));
        }

        #endregion

        #endregion

    }
}

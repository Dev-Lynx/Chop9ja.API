using AutoMapper;
using Chop9ja.API.Data;
using Chop9ja.API.Extensions;
using Chop9ja.API.Extensions.Attributes;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.ViewModels;
using Chop9ja.API.Services.Interfaces;
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
    [AuthorizeRoles(UserRoles.Agent, UserRoles.Administrator)]
    public class TransactionsController : ControllerBase
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

        [DeepDependency]
        IPaymentService PaymentService { get; }
        #endregion

        #endregion

        #region Methods
        [HttpGet]
        public async Task<IActionResult> GetTransactions([FromQuery]DateRangeSieveModel model)
        {
            bool search = !string.IsNullOrWhiteSpace(model.SearchQuery);

            DateRange range = new DateRange(model.Start, model.End);
            var filter = range.Includes<Transaction>();

            if (search)
            {
                string json = await System.IO.File.ReadAllTextAsync(Path.Combine(Core.DATA_DIR, "paymentChannels.json"));
                var channels = JsonConvert.DeserializeObject<List<PaymentChannel>>(json);

                foreach (var channel in channels)
                    filter = filter.Or(t => t.PaymentChannelId == channel.Id);
            }

            
            var transactions = await DataContext.Store.GetAllAsync(filter);//await (await DataContext.Store.GetOneAsync<User>(u => u.IsPlatform)).Wallet.GetTransactions(filter);

            var data = Sieve.Apply(model, Mapper.Map<IEnumerable<BackOfficeTransactionViewModel>>(transactions).AsQueryable());

            return Ok(data);
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetRequests([FromQuery]DateRangeSieveModel model)
        {
            bool search = !string.IsNullOrWhiteSpace(model.SearchQuery);
            DateRange range = new DateRange(model.Start, model.End);
            var filter = range.IncludesRequest();

            var request = await DataContext.Store.GetAllAsync(filter);

            var data = Sieve.Apply(model, Mapper.Map<IEnumerable<UserDepositPaymentRequestViewModel>>(request).AsQueryable());

            if (search)
            {
                string query = model.SearchQuery.ToLower();

                data.Where(d =>
                d.PlatformBankAccount.AccountName.ToLower().Contains(query) ||
                d.PlatformBankAccount.AccountNumber.Contains(query) ||
                d.Description.Contains(query));
                
                // TODO: Improve search.
            }

            return Ok(data);
        }

        [HttpPost("requests/update")]
        public async Task<IActionResult> UpdateRequest([FromBody]StatusUpdateViewModel model)
        {
            if (!Guid.TryParse(model.Id, out Guid requestId)) return NotFound();

            PaymentRequest request = await DataContext.Store
                .GetOneAsync<PaymentRequest>(r => r.Id == requestId);

            if (model.Status == RequestStatus.Pending) return BadRequest("Forbidden action attempted. Transactions cannot be reversed.");
            if (request.Status != RequestStatus.Pending) return BadRequest("The current transaction has already been completed");

            if (model.Status == RequestStatus.Declined)
            {
                request.Status = model.Status;
                await DataContext.Store.UpdateOneAsync(request);
                return Ok();
            }

            if (!await PaymentService.ConcludeTransaction(request)) return BadRequest("Failed to process request");

            return Ok();
        }
        #endregion
    }
}

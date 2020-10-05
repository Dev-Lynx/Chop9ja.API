using AutoMapper;
using Chop9ja.API.Data;
using Chop9ja.API.Extensions.Attributes;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.ViewModels;
using Chop9ja.API.Models.ViewModels.BackOffice;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sieve.Services;
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
    public class AgentsController : ControllerBase
    {
        #region Properties
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

        #region Methods
        [HttpGet]
        public async Task<IActionResult> GetAgents([FromQuery]DateRangeSieveModel model)
        {
            var agents = await DataContext.Store.GetAllAsync<User>(u => u.MajorRole == UserRoles.Agent);
             
            return Ok(Mapper.Map<IEnumerable<AgentViewModel>>(agents));
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAgentStatus([FromBody]AgentValidationStatusViewModel model)
        {
            User agent = await UserManager.FindByIdAsync(model.Id.ToString());
            agent.ValidationStatus = model.Status;
            agent.InspectionDate = model.InspectionDate;

            if (!await DataContext.Store.UpdateOneAsync(agent))
                return BadRequest("An unexpected error occured while updating agent values");

            return Ok();
        }
        #endregion
    }
}

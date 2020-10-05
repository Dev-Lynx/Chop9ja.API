using AutoMapper;
using Chop9ja.API.Data;
using Chop9ja.API.Extensions.Attributes;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.ViewModels.Agent;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Controllers.Agent
{
    [AutoBuild]
    [ApiController]
    [Route("api/agent/[controller]")]
    [AuthorizeRoles(UserRoles.Agent)]
    public class WalletController : ControllerBase
    {
        #region Properties

        #region Services

        [DeepDependency]
        ILogger Logger { get; }

        [DeepDependency]
        UserManager<User> UserManager { get; }

        [DeepDependency]
        IMapper Mapper { get; }

        [DeepDependency]
        MongoDataContext DataContext { get; }
        #endregion

        #endregion

        #region Methods
        /*
        [HttpPost("deposit")]
        public async Task<IActionResult> FundWallet([FromBody]AgentDepositViewModel model)
        {
            string id = User.FindFirst("id").Value;

            User user = await DataContext.Store.GetOneAsync<User>(u => u.Id == model.User);

            
        }
        */

        #endregion
    }
}

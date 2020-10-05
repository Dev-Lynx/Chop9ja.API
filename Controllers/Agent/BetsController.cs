using Chop9ja.API.Data;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Controllers.Agent
{
    /// <summary>
    /// Manages Agent Betting
    /// </summary>
    [AutoBuild]
    [ApiController]
    [Route("api/agent/[controller]")]
    public class BetsController : ControllerBase
    {
        #region Properties

        #region Services
        [DeepDependency]
        UserManager<User> UserManager { get; }

        [DeepDependency]
        MongoDataContext DataContext { get; }
        #endregion

        #endregion

        #region Methods

        public async Task<IActionResult> InsureBet([FromBody]BetViewModel model)
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);


            return Ok();
        }

        #endregion
    }
}

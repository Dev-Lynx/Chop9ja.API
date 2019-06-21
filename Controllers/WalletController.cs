using AutoMapper;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Controllers
{
    /// <summary>
    /// Manages user wallet details
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
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
        IMapper Mapper { get; }
        #endregion

        #endregion

        #region Methods

        #region RESTful API Calls
        [HttpGet]
        public Task<IActionResult> GetWallet()
        {
            return null;
        }

        /// <summary>
        /// Makes a deposit with interswitch
        /// </summary>
        /// <returns></returns>
        [HttpPost("deposit/interswitch")]
        public Task<IActionResult> MakeInterswitchDeposit()
        {
            return null;
        }


        [HttpPost("withdraw")]
        public Task<IActionResult> WithdrawFunds()
        {
            return null;
        }
        #endregion

        #endregion
    }
}

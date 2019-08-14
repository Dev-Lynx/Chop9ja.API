using AutoMapper;
using Chop9ja.API.Data;
using Chop9ja.API.Extensions.Attributes;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Entities;
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
    [AuthorizeRoles(UserRoles.Agent, UserRoles.Administrator)]
    public class PaymentRequestsController : ControllerBase
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
        public async Task<IActionResult> GetPaymentRequests()
        {
            await Task.Delay(10);
            return Ok();
        }
        #endregion

        #endregion
    }
}

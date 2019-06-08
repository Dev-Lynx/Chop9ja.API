using AutoMapper;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.ViewModels;
using Google.Apis.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Chop9ja.API.Controllers
{
    /// <summary>
    /// Handles Manages User Account.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [AutoBuild]
    public class AccountController : ControllerBase
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
        /// <summary>
        /// Gets the current user.
        /// </summary>
        [HttpGet("user")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(UserViewModel), Description = "Current User")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(UnauthorizedResult), Description = "User was not found or the user credentials were invalid.")]
        public async Task<IActionResult> GetUser()
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);

            if (user == null) return Unauthorized();

            return Ok(Mapper.Map<UserViewModel>(user));
        }
        #endregion

        #endregion
    }
}

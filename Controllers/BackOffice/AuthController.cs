using Chop9ja.API.Data;
using Chop9ja.API.Extensions.Attributes;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.ViewModels;
using Chop9ja.API.Services.Interfaces;
using Google.Apis.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Chop9ja.API.Controllers.BackOffice
{
    /// <summary>
    /// Handles BackOffice Authentication.
    /// </summary>
    [Route("api/backOffice/[controller]")]
    [ApiController]
    [AutoBuild]
    public class AuthController : ControllerBase
    {
        #region Properties

        #region Services
        [DeepDependency]
        UserManager<User> UserManager { get; }

        [DeepDependency]
        MongoDataContext DataContext { get; }

        [DeepDependency]
        IEmailService EmailService { get; }

        [DeepDependency]
        ISmsService SmsService { get; }

        [DeepDependency]
        IJwtFactory JwtFactory { get; }

        [DeepDependency]
        ILogger Logger { get; }
        #endregion

        #endregion

        #region Methods

        #region RESTful API Calls

        /// <summary>
        /// Log in a user into the back office
        /// </summary>
        /// <param name="model">User login details.</param>
        /// <returns>A JWT access token or a collection of the errors found.</returns>
        [HttpPost("login")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(AccessTokenModel), Description = "A JWT Access Token for user account access.")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(NotFoundResult), Description = "User does not exist on this platform.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(UnauthorizedResult), Description = "Invalid User Credentials.")]
        public async Task<IActionResult> Login([FromBody]BackOfficeLoginViewModel model)
        {
            User user = await DataContext.Store.GetOneAsync<User>(u => u.UserName.ToLower() == model.Username.ToLower());

            if (user == null) return NotFound();
            if (!(await UserManager.IsInRoleAsync(user, UserRoles.Administrator.ToString()))) return Unauthorized();

            bool valid = await UserManager.CheckPasswordAsync(user, model.Password);

            if (!valid) return Unauthorized();

            return Ok(new AccessTokenModel() { AccessToken = await JwtFactory.GenerateToken(user) });
        }

        #endregion

        #endregion
    }
}

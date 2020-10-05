using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AutoMapper;
using Chop9ja.API.Data;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.ViewModels;
using Chop9ja.API.Models.ViewModels.Agent;
using Chop9ja.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Chop9ja.API.Controllers.Agent
{
    /// <summary>
    /// Handles Agent Authentication.
    /// </summary>
    [Route("api/agent/[controller]")]
    [ApiController]
    [AutoBuild]
    public class AuthController : ControllerBase
    {
        #region Properties

        #region Services
        [DeepDependency]
        IAuthService Auth { get; }

        [DeepDependency]
        MongoDataContext DataContext { get; }

        [DeepDependency]
        IMongoRepository DataStore { get; }

        [DeepDependency]
        UserManager<User> UserManager { get; }

        [DeepDependency]
        IJwtFactory JwtFactory { get; }

        [DeepDependency]
        ILogger Logger { get; }

        [DeepDependency]
        IMapper Mapper { get; }
        #endregion

        #endregion

        #region Methods

        #region Reception
        /// <summary>
        /// Register a new user.
        /// </summary>
        /// <param name="model">User to register</param>
        /// <returns>A JWT access token or a collection of the errors found.</returns>
        [HttpPost("register")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(AccessTokenModel), Description = "A JWT Access Token for user account access.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(IEnumerable<IdentityError>), Description = "The input was invalid. Returns a collection of the errors found.")]
        public async Task<IActionResult> Register(AgentRegistrationViewModel model)
        {
            User user = Mapper.Map<User>(model);

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded) return new BadRequestObjectResult(result.Errors);

            result = await UserManager.AddToRoleAsync(user, UserRoles.Agent.ToString());
            if (!result.Succeeded) return new BadRequestObjectResult(result.Errors);

            result = await UserManager.AddPasswordAsync(user, model.Password);
            if (!result.Succeeded) return new BadRequestObjectResult(result.Errors);

            user.MajorRole = UserRoles.Agent;
            await user.InitializeAsync();

            return Ok(new AccessTokenModel() { AccessToken = await JwtFactory.GenerateToken(user) });
        }

        /// <summary>
        /// Log a user in.
        /// </summary>
        /// <param name="model">User login details.</param>
        /// <returns>A JWT access token or a collection of the errors found.</returns>
        [HttpPost("login")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(AccessTokenModel), Description = "A JWT Access Token for user account access.")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(NotFoundResult), Description = "User does not exist on this platform.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(UnauthorizedResult), Description = "Invalid User Credentials.")]
        public async Task<IActionResult> Login([FromBody]UserLoginViewModel model)
        {
            User user = await DataContext.Store.GetOneAsync<User>(u => u.UserName.ToLower() == model.Username.ToLower());

            if (user == null) return NotFound();

            bool valid = await UserManager.CheckPasswordAsync(user, model.Password);

            if (!valid) return Unauthorized();

            return Ok(new AccessTokenModel() { AccessToken = await JwtFactory.GenerateToken(user) });
        }
        #endregion  

        #endregion
    }
}

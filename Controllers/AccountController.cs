using AutoMapper;
using Chop9ja.API.Data;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        MongoDataContext DataContext { get; }

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

        [HttpPost("manage/changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody]PasswordChangeViewModel model)
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);

            if (user == null) return Unauthorized();

            await UserManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            return Ok();
        }

        [HttpGet("bankAccounts")]
        public async Task<IActionResult> GetBankAccounts()
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);

            if (user == null) return Unauthorized();

            var accounts = Mapper.Map<IEnumerable<BankAccountViewModel>>(user.BankAccounts);
            return Ok(accounts);
        }
        
        [HttpPost("manage/bankAccounts/add")]
        public async Task<IActionResult> AddBankAccount([FromBody]NewBankAccountViewModel model)
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);

            if (user == null) return Unauthorized();

            BankAccount account = Mapper.Map<BankAccount>(model);

            user.BankAccounts.Add(account);
            await DataContext.Store.UpdateOneAsync(user);

            return Ok();
        }

        [HttpPost("manage/bankAccounts/remove")]
        public async Task<IActionResult> RemoveBankAccount([FromBody]string accountId)
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);

            if (user == null) return Unauthorized();

            bool success = user.BankAccounts.RemoveAll(b => b.Id == Guid.Parse(accountId)) > 0;
            await DataContext.Store.UpdateOneAsync(user);

            if (!success) return NotFound($"No Bank Account exists with id {accountId}");
            return Ok();
        }
       
        #endregion

        #endregion
    }
}

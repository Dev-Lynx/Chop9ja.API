﻿using AutoMapper;
using Chop9ja.API.Data;
using Chop9ja.API.Extensions.Attributes;
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
    /// Manages User Account.
    /// </summary>
    [AutoBuild]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
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

        
        /// <summary>
        /// Get user bank accounts
        /// </summary>
        /// <returns>An OK Response</returns>
        [HttpGet("bankAccounts")]
        [AuthorizeRoles(UserRoles.Regular)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<UserBankAccountViewModel>), Description = "A list of the user's bank account.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(UnauthorizedResult), Description = "User was not found or the user credentials were invalid.")]
        public async Task<IActionResult> GetBankAccounts()
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);

            if (user == null) return Unauthorized();

            var accounts = Mapper.Map<IEnumerable<UserBankAccountViewModel>>(user.BankAccounts.Where(b => b.IsActive));
            return Ok(accounts);
        }


        /// <summary>
        /// Add a new bank account
        /// </summary>
        /// <param name="model">Banks and IDs are modelled after 
        /// (https://github.com/tomiiide/nigerian-banks/blob/master/banks.json).
        /// </param>
        /// <returns>An OK Response</returns>
        [AuthorizeRoles(UserRoles.Regular)]
        [HttpPost("manage/bankAccounts/add")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OkResult), Description = "Bank account was successfully added")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(UnauthorizedResult), Description = "User was not found or the user credentials were invalid.")]
        public async Task<IActionResult> AddBankAccount([FromBody]BankAccountViewModel model)
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);

            if (user == null) return Unauthorized();

            BankAccount account = Mapper.Map<BankAccount>(model);

            BankAccount clone = user.BankAccounts.FirstOrDefault(b =>
            {
                return b.BankId == model.BankId
                && b.AccountNumber == model.AccountNumber;
            });

            if (clone != null)
            {
                int index = user.BankAccounts.IndexOf(clone);
                clone.IsActive = true;
                user.BankAccounts[index] = account;
                await DataContext.Store.UpdateOneAsync(user);
                return Ok();
            }

            if (!user.BankAccounts.Any(b => b.IsDefault))
                account.IsDefault = true;

            user.BankAccounts.Add(account);
            await DataContext.Store.UpdateOneAsync(user);

            return Ok();
        }

        /// <summary>
        /// Set a bank account as default
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("manage/bankAccounts/default")]
        [AuthorizeRoles(UserRoles.Regular)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OkResult), Description = "The account was successfully set as default")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(NotFoundResult), Description = "The bank account was not found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(UnauthorizedResult), Description = "User was not found or the user credentials were invalid.")]
        public async Task<IActionResult> SetBankAccountAsDefault([FromBody]UserDefaultBankAccountViewModel model)
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);

            if (user == null) return Unauthorized();

            BankAccount account = user.BankAccounts.FirstOrDefault(b => b.IsActive && b.Id == model.Id);

            if (account == null) return NotFound();

            if (account.IsDefault != model.IsDefault)
            {
                account.IsDefault = model.IsDefault;
                await DataContext.Store.UpdateOneAsync(user);
            }

            return Ok();
        }

        /// <summary>
        /// Delete a bank account
        /// </summary>
        /// <returns>An OK Response</returns>
        [AuthorizeRoles(UserRoles.Regular)]
        [HttpPost("manage/bankAccounts/remove")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OkResult), Description = "Bank account was successfully removed")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(UnauthorizedResult), Description = "User was not found or the user credentials were invalid.")]
        public async Task<IActionResult> RemoveBankAccount([FromBody]BankAccountIdViewModel model)
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);

            if (user == null) return Unauthorized();

            BankAccount account = user.BankAccounts.FirstOrDefault(b => b.Id == Guid.Parse(model.Id));

            if (account == null) return NotFound($"No Bank Account exists with id {model.Id}");

            account.IsActive = false;

            if (account.IsDefault)
            {
                account.IsDefault = false;
                BankAccount nextAccount = user.BankAccounts.FirstOrDefault(b => b.IsActive);
                if (nextAccount != null) nextAccount.IsDefault = true;
            }

            bool success = await DataContext.Store.UpdateOneAsync(user);
            return Ok();
        }
       
        #endregion

        #endregion
    }
}

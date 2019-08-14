using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AutoMapper;
using Chop9ja.API.Data;
using Chop9ja.API.Extensions;
using Chop9ja.API.Extensions.RedocExtensions;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.Options;
using Chop9ja.API.Models.ViewModels;
using Chop9ja.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSwag.Annotations;
using OtpNet;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Chop9ja.API.Controllers
{
    /// <summary>
    /// Handles User Authentication.
    /// </summary>
    [Route("api/[controller]")]
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
        IMapper Mapper { get; }

        [DeepDependency]
        PhoneNumberUtil Phone { get; }

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

        #region Reception
        /// <summary>
        /// Register a new user.
        /// </summary>
        /// <param name="model">User to register</param>
        /// <returns>A JWT access token or a collection of the errors found.</returns>
        [HttpPost("register")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(AccessTokenModel), Description = "A JWT Access Token for user account access.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(IEnumerable<IdentityError>), Description = "The input was invalid. Returns a collection of the errors found.")]
        public async Task<IActionResult> Register([FromBody]UserRegistrationViewModel model)
        {
            User user = Mapper.Map<User>(model);

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded) return new BadRequestObjectResult(result.Errors);

            result = await UserManager.AddToRoleAsync(user, UserRoles.Regular.ToString());
            if (!result.Succeeded) return new BadRequestObjectResult(result.Errors);

            result = await UserManager.AddPasswordAsync(user, model.Password);
            if (!result.Succeeded) return new BadRequestObjectResult(result.Errors);

            await user.InitializeAsync();

            try
            {
                string template = await EmailService.GetTemplateAsync(Core.EmailTemplates.Verification);
                template = template.Replace("|USERNAME|", user.FirstName);
                await EmailService.SendEmailAsync(user.Email, "Welcome To Chop9ja", template);
            }
            catch (Exception ex)
            {
                Logger.LogError($"An error occured while sending an welcome email\n{ex}");
            }


            string message = string.Format("Hello {0}\n Welcome to Chop9ja. Click on the link below to validate your account", user.FirstName);
            await SmsService.SendMessage(user.FormattedPhoneNumber, message);

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

        /// <summary>
        /// Check if an email or username is already taken.
        /// </summary>
        /// <param name="model">Data sent via query. Use either email or username.</param>
        [HttpGet("user/exists")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(bool))]
        public async Task<IActionResult> UserExists([FromQuery]UserExistsModel model)
        {
            bool exists = false;
            bool useMail = !string.IsNullOrWhiteSpace(model.Email);
            bool useName = !string.IsNullOrWhiteSpace(model.UserName);
            bool mailExists = false;
            bool nameExists = false;

            if (useMail)
                mailExists = await DataContext.Store.AnyAsync<User>(u => u.Email.ToLower() == model.Email.ToLower());
            if (useName)
                nameExists = await DataContext.Store.AnyAsync<User>(u => u.UserName.ToLower() == model.UserName.ToLower());

            if (useMail && useName) exists = nameExists && mailExists;
            else if (useMail) exists = mailExists;
            else if (useName) exists = nameExists;

            return Ok(exists);
        }
        #endregion

        #region Password Management
        /// <summary>
        /// Change account password
        /// </summary>
        /// <param name="model">Password change model</param>
        /// <returns>An OK Response</returns>
        [Authorize]
        [HttpPost("changePassword")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OkResult), Description = "Password was successfully changed")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(UnauthorizedResult), Description = "User was not found or the user credentials were invalid.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(IEnumerable<IdentityError>), Description = "The input was invalid. Returns a collection of the errors found.")]
        public async Task<IActionResult> ChangePassword([FromBody]PasswordChangeViewModel model)
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);

            if (user == null) return Unauthorized();

            IdentityResult result = await UserManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok();
        }

        [HttpGet("password/reset/verify/token")]
        public async Task<IActionResult> VerifyPasswordResetToken([FromQuery]PasswordResetTokenVerificationModel model)
        {
            User user = await UserManager.FindByNameAsync(model.Username);

            if (user == null) return NotFound();

            bool valid = false;
            try
            {
                valid = await UserManager.VerifyUserTokenAsync(user, UserManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", model.Token);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occured while verifying user token.");
            }
            

            return Ok(valid);
        }

        [HttpGet("password/reset/verify/otp")]
        public async Task<IActionResult> VerifyOneTimePasswordResetToken([FromQuery]PasswordResetTokenVerificationModel model)
        {
            User user = await UserManager.FindByNameAsync(model.Username);

            if (user == null) return NotFound();

            bool valid = await Auth.VerifyOneTimePassword(user, OnePasswordType.Phone, model.Token);

            return Ok(valid);
        }

        /// <summary>
        /// Send a password reset email.
        /// </summary>
        [HttpPost("password/reset/mail")]
        public async Task<IActionResult> GeneratePasswordResetToken([FromBody]CustomEmailViewModel model)
        {
            User user = await UserManager.FindByEmailAsync(model.To);

            if (user == null) return NotFound($"No email ({model.To}) could be found on this platform");

            string body = model.Body;

            try
            {
                string token = await UserManager.GeneratePasswordResetTokenAsync(user);
                UserTokenViewModel context = Mapper.Map<UserTokenViewModel>(user);
                context.Token = HttpUtility.UrlEncode(token);

                body = body.BindTo(context);
                Logger.LogDebug(body);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occured while formatting input body.\n{0}", body);
                return BadRequest("The format of the email body is invalid.");
            }

            bool success = await EmailService.
                SendEmailAsync(user.Email.ToLower(), model.Subject, body);

            if (!success) return BadRequest("An error occured while sending the password reset email");

            return Ok();
        }

        /// <summary>
        /// Send a password reset sms.
        /// </summary>
        [HttpPost("password/reset/sms")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OneTimePasswordViewModel), Description = "One Time Password Metadata.")]
        public async Task<IActionResult> GeneratePasswordResetSms([FromBody]CustomSMSViewModel model)
        {
            User user = await DataContext.Store.GetOneAsync<User>(u => u.PhoneNumber == model.Phone);

            if (user == null) return NotFound($"No User with that phone number could be found on this platform.");

            string message = model.Message;

            OneTimePassword password = null;
            try
            {
                password = await Auth.GenerateOneTimePassword(user, OnePasswordType.Phone);
                UserOneTimePasswordModel context = Mapper.Map<UserOneTimePasswordModel>(user);
                context.OneTimePassword = password.Code;

                message = message.BindTo(context);
                Logger.LogWarning(user.FormattedPhoneNumber);
                Logger.LogWarning(message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occured while formatting a text message\n{0}", message);
                return BadRequest("The format of the text message is invalid");
            }

            bool success = await SmsService.SendMessage(user.FormattedPhoneNumber, message);

            if (!success) return BadRequest("An error occured while sending the password reset text message");

            return Ok(Mapper.Map<OneTimePasswordViewModel>(password));
        }

        /// <summary>
        /// Reset User's Password using token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("password/reset")]
        public async Task<IActionResult> ResetPassword(PasswordResetViewModel model)
        {
            User user = await UserManager.FindByNameAsync(model.Username);

            if (user == null) return NotFound($"No username ({model.Username}) could be found on this platform");

            bool success = (await UserManager.
                ResetPasswordAsync(user, model.Token, model.NewPassword)).Succeeded;

            if (!success) success = (await UserManager.
                ResetPasswordAsync(user, HttpUtility.UrlDecode(model.Token),
                model.NewPassword)).Succeeded;

            if (!success) return BadRequest("The provided token is expired or invalid");

            return Ok(new AccessTokenModel() { AccessToken = await JwtFactory.GenerateToken(user) });
        }

        /// <summary>
        /// Reset User's Password using One Time Password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("password/otpReset")]
        public async Task<IActionResult> ResetPassword(OneTimePasswordResetViewModel model)
        {
            User user = await UserManager.FindByNameAsync(model.Username);

            if (user == null) return NotFound($"No username ({model.Username}) could be found on this platform");

            bool success = await Auth.ValidateOneTimePassword(user, OnePasswordType.Phone, model.OneTimePassword);

            if (!success) return BadRequest("The provided one time password is expired or invalid");

            string token = await UserManager.GeneratePasswordResetTokenAsync(user);
            await UserManager.ResetPasswordAsync(user, token, model.NewPassword);

            return Ok(new AccessTokenModel() { AccessToken = await JwtFactory.GenerateToken(user) });
        }
        #endregion

        #region Verification
        /// <summary>
        /// Send an email for user verification.
        /// </summary>
        /// <param name="model">
        /// The body of the email should be formatted with 
        /// points (eg. {0}, {1}) where the token and other information
        /// can be inserted. Refer to the request sample in redoc for a
        /// valid sample of an email verification body.
        /// </param>
        [Authorize]
        [HttpPost("otp/email")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OneTimePasswordViewModel), Description = "One Time Password Metadata.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(UnauthorizedResult), Description = "User was not found or the user credentials were invalid.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(BadRequestObjectResult), Description = "Email body was not formatted properly.")]
        [SwaggerOperationProcessor(typeof(ReDocCodeSampleAppender), "Javascript", "var body = 'This is a sample verification email." +
            " click https://sitename.com/yada-yada?code={0} to verify your email.'")]
        public async Task<IActionResult> GenerateEmailOneTimePassword([FromBody]EmailViewModel model)
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);

            if (user == null) return Unauthorized();

            // TODO: Use inbuild tokens
            OneTimePassword password = await Auth.GenerateOneTimePassword(user, OnePasswordType.Email);
            var pvm = Mapper.Map<OneTimePasswordViewModel>(password);

            string body = model.Body;

            try
            {
                string code = Encoding.ASCII.ToBase64(password.Code);
                body = string.Format(body, code);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occured while formatting input body.\n{0}", body);
                return BadRequest("The format of the email body is invalid.");
            }

            await EmailService.SendEmailAsync(user.Email.ToLower(), model.Subject, body);

            return Ok(pvm);
        }


        /// <summary>
        /// Send an sms for user verification.
        /// </summary>
        /// <param name="model">
        /// The body of the sms should be formatted with 
        /// points (eg. {0}, {1}) where the token and other information
        /// can be inserted. Refer to the request sample in redoc for a
        /// valid sample of a verification text message.
        /// </param>
        [Authorize]
        [HttpPost("otp/phone")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OneTimePasswordViewModel), Description = "One Time Password Metadata.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(UnauthorizedResult), Description = "User was not found or the user token was invalid.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(BadRequestObjectResult), Description = "The message was not formatted properly.")]
        [SwaggerOperationProcessor(typeof(ReDocCodeSampleAppender), "Javascript", "var message = 'This is a sample verification text message." +
            " Your One Time Password is {0}. Please keep it safe and do not it share with anyone.'")]
        public async Task<IActionResult> GeneratePhoneOneTimePassword([FromBody]SMSViewModel model)
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);

            if (user == null) return Unauthorized();

            OneTimePassword password = await Auth.GenerateOneTimePassword(user, OnePasswordType.Phone);
            var pvm = Mapper.Map<OneTimePasswordViewModel>(password);

            string body = model.Message;
            try
            {
                body = string.Format(body, password.Code);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occured while formatting input body.\n{0}", body);
                return BadRequest("The format of the text message is invalid.");
            }

            string phone = user.PhoneNumber;

            if (!Phone.TryParse(phone, "NG", out PhoneNumber number)) return BadRequest($"User Phone Number ({user.PhoneNumber}) was not in a valid format");

            phone = Phone.Format(number, PhoneNumberFormat.E164);


            await SmsService.SendMessage(phone, body);
            return Ok(pvm);
        }

        /// <summary>
        /// Verify a user's email address.
        /// </summary>
        /// <param name="code">
        /// Validation code returned when user clicks the verification link.
        /// </param>
        [Authorize]
        [HttpGet("verify/email")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(string), Description = "Email Verification was successful.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(UnauthorizedResult), Description = "User was not found or the user token was invalid.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(BadRequestObjectResult), Description = "The One Time Password was either altered or invalid.")]
        public async Task<IActionResult> VerifyEmailOneTimePassword([FromQuery]string code)
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);

            if (user == null) return Unauthorized();

            bool valid = Encoding.ASCII.TryParseBase64(code, out code);

            if (!valid) return BadRequest("One Time Password was in an invalid format");

            valid = await Auth.ValidateOneTimePassword(user, OnePasswordType.Email, code);

            if (!valid) return BadRequest("One Time Password was invalid");

            return Ok();
        }

        /// <summary>
        /// Verify a user's phone number.
        /// </summary>
        /// <param name="code">
        /// One Time Password submitted by the user.
        /// </param>
        [Authorize]
        [HttpGet("verify/phone")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(string), Description = "Phone Verification was successful.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(UnauthorizedResult), Description = "User was not found or the user token was invalid.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(BadRequestObjectResult), Description = "The One Time Password was invalid.")]
        public async Task<IActionResult> VerifyPhoneOneTimePassword([FromQuery]string code)
        {
            string id = User.FindFirst("id").Value;
            User user = await UserManager.FindByIdAsync(id);

            if (user == null) return Unauthorized();

            bool valid = await Auth.ValidateOneTimePassword(user, OnePasswordType.Phone, code);

            if (!valid) return BadRequest("One Time Password was invalid");

            return Ok();
        }
        #endregion

        #endregion


        #endregion
    }
}

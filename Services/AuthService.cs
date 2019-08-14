using Chop9ja.API.Data;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.Options;
using Chop9ja.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Chop9ja.API.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Chop9ja.API.Extensions.Encryption;
using Unity;

namespace Chop9ja.API.Services
{
    [AutoBuild]
    public class AuthService : IAuthService
    {
        #region Properties

        #region Options
        public AuthSettings AuthSettings => OAuthSettings.Value;
        #endregion

        #region Services
        [DeepDependency]
        IOptions<AuthSettings> OAuthSettings { get; }

        [DeepDependency]
        MongoDataContext DataContext { get; }

        [DeepDependency]
        IMongoRepository DataStore { get; }

        [DeepDependency]
        UserManager<User> UserManager { get; }

        [DeepDependency]
        ILogger Logger { get; }

        [DeepDependency]
        IUnityContainer Container { get; }
        #endregion

        #region Internals
        ITokenGenerator Generator { get; set; }
        #endregion

        #endregion

        #region Methods

        #region IAuthService Implementation
        public async Task<OneTimePassword> GenerateOneTimePassword(User user, OnePasswordType kind)
        {
            Generator = NewGenerator(user);

            OneTimePassword password = user.OneTimePasswords
                .FirstOrDefault(p => p.IsActive && p.Kind == kind);

            if (password != null) return password;
            
            password = new OneTimePassword()
            {
                Kind = kind
            };

            user.OneTimePasswords.Add(password);

            password.Code = Generator.ComputeHOTP(user.OneTimePasswords.LongCount());
            await DataStore.UpdateOneAsync(user);

            return password;
        }

        public async Task<bool> ValidateOneTimePassword(User user, OnePasswordType kind, string code)
        {
            Generator = NewGenerator(user);

            // TODO: Stop Storing One Time Passwords, maybe just count them?
            OneTimePassword password = user.OneTimePasswords.
                FirstOrDefault(p => p.IsActive && p.Kind == kind);

            if (password == null) return false;

            if (!Generator.VerifyHotp(code, user.OneTimePasswords.LongCount())) return false;
            if (!password.Validate(code)) return false;

            switch (kind)
            {
                // TODO: Deprecate OnePasswordType
                case OnePasswordType.Email:
                    user.EmailConfirmed = true;
                    break;

                case OnePasswordType.Phone:
                    user.PhoneNumberConfirmed = true;
                    break;
            }

            await DataStore.UpdateOneAsync(user);
            return true;
        }

        public async Task<bool> VerifyOneTimePassword(User user, OnePasswordType kind, string code)
        {
            Generator = NewGenerator(user);

            // TODO: Stop Storing One Time Passwords, maybe just count them?
            OneTimePassword password = user.OneTimePasswords.
                FirstOrDefault(p => p.IsActive && p.Kind == kind);

            if (password == null) return false;

            if (!Generator.VerifyHotp(code, user.OneTimePasswords.LongCount())) return false;

            return password.Code == code;
        }
        #endregion

        ITokenGenerator NewGenerator(User user)
        {
            ITokenGenerator generator;
            try
            {
                string crypt = Crypt.XOR(user.UserName, AuthSettings.Key);
                Logger.LogWarning(crypt);
                byte[] key = Encoding.ASCII.GetBytes(crypt);
                generator = new TokenGenerator(key);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occured while attempting to create a user based token generator.");
                generator = Container.Resolve<ITokenGenerator>();
            }

            return generator;
        }

        #endregion
    }
}

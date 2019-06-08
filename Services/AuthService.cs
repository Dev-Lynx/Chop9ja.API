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
        Hotp Generator { get; set; }

        [DeepDependency]
        UserDataContext DataContext { get; }

        [DeepDependency]
        UserManager<User> UserManager { get; }

        [DeepDependency]
        ILogger Logger { get; }
        #endregion

        #endregion

        #region Methods

        #region IAuthService Implementation
        public async Task<OneTimePassword> GenerateOneTimePassword(User user, OnePasswordType kind)
        {
            await DataContext.Users.Attach(user).Collection(u => u.OneTimePasswords).LoadAsync();
            OneTimePassword password = user.OneTimePasswords.
                FirstOrDefault(p => p.IsActive && p.Kind == kind);

            if (password != null) return password;

            password = new OneTimePassword()
            {
                Kind = kind
            };

            user.OneTimePasswords.Add(password);

            long index = await DataContext.OneTimePasswords.LongCountAsync();
            password.Code = Generator.ComputeHOTP(index);

            await DataContext.SaveChangesAsync();
            return password;
        }

        public async Task<bool> ValidateOneTimePassword(User user, OnePasswordType kind, string code)
        {
            await DataContext.Attach(user).Collection(u => u.OneTimePasswords).LoadAsync();

            OneTimePassword password = user.OneTimePasswords.
                FirstOrDefault(p => p.IsActive && p.Kind == kind);

            if (password == null) return false;

            if (!password.Validate(code)) return false;

            switch (kind)
            {
                case OnePasswordType.Email:
                    user.EmailConfirmed = true;
                    break;

                case OnePasswordType.Phone:
                    user.PhoneNumberConfirmed = true;
                    break;
            }

            await DataContext.SaveChangesAsync();

            return true;
        }
        #endregion

        [DeepInjectionMethod]
        void Initialize()
        {
            Generator = new Hotp(Encoding.ASCII.GetBytes(AuthSettings.Key));
        }

        #endregion
    }
}

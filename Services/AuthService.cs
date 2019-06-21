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
        ITokenGenerator Generator { get; set; }

        [DeepDependency]
        MongoDataContext DataContext { get; }

        [DeepDependency]
        IMongoRepository DataStore { get; }

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
            /*
            var tracker = DataContext.Entry(user);
            var collection = tracker.Collection(u => u.OneTimePasswords);

            await collection.LoadAsync();
            */

            //user = DataContext.Users.FirstOrDefault(u => u.Id == user.Id);
            OneTimePassword password = user.OneTimePasswords.FirstOrDefault(p => p.IsActive && p.Kind == kind);

            if (password != null) return password;
            
            password = new OneTimePassword()
            {
                Kind = kind
            };

            await DataStore.AddOneAsync(password);
            user.OneTimePasswordIds.Add(password.Id);



            //user.OneTimePasswords.Add(password);

            
            //long index = (await DataContext.OneTimePasswords.ToListAsync()).LongCount();
            long index = await DataStore.CountAsync<OneTimePassword>(p => true);
            password.Code = Generator.ComputeHOTP(index);

            await DataStore.UpdateOneAsync(password);
            await DataStore.UpdateOneAsync(user);
            //await Task.Run(() => DataContext.Users.Update(user));
            //await DataContext.SaveChangesAsync();

            Logger.LogInformation("Successfully generated one time password for {0} - ({1})", user.UserName, password.Code);
            return password;
        }

        public async Task<bool> ValidateOneTimePassword(User user, OnePasswordType kind, string code)
        {
            /*
            await DataContext.Attach(user).Collection(u => u.OneTimePasswords).LoadAsync();
            */
            //user = DataContext.Users.FirstOrDefault(u => u.Id == user.Id);
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


            await DataStore.UpdateOneAsync(password);
            await DataStore.UpdateOneAsync(user);
            
            // await Task.Run(() => DataContext.Users.Update(user));
            // await Task.Run(() => DataContext.OneTimePasswords.Update(password));
            // await DataContext.SaveChangesAsync();

            Logger.LogInformation("Successfully verified user {1} one time password {0}", user.UserName, password.Kind);
            return true;
        }
        #endregion

        #endregion
    }
}

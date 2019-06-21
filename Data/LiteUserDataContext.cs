using Chop9ja.API.Extensions;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Entities;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Data
{
    /*
    [AutoBuild]
    public class LiteUserDataContext : LiteDbUserStore<User>, ILiteDbContext
    {
        #region Properties
        public LiteCollection<User> Users { get; }
        public LiteCollection<OneTimePassword> OneTimePasswords { get; }
        public LiteDatabase LiteDatabase { get; }

        [DeepDependency]
        ILogger Logger { get; }
        #endregion

        #region Constructors
        public LiteUserDataContext(LiteDatabase database) : base(new LiteDbContext(database))
        {
            LiteDatabase = database;

            LiteDatabase.Log.Level = 255;
            LiteDatabase.Log.Logging += (e) =>
            {
                Logger.LogInformation(e);
            };

            Users = LiteDatabase.Collection<User>();
            OneTimePasswords = LiteDatabase.Collection<OneTimePassword>();
        }
        #endregion
    }
    */
}

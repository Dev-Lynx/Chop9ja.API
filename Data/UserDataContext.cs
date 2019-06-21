using AspNetCore.Identity.Mongo.Model;
using Blueshift.EntityFrameworkCore.MongoDB.Annotations;
using Blueshift.EntityFrameworkCore.MongoDB.Infrastructure;
using Blueshift.Identity.MongoDB;
using Chop9ja.API.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Data
{
    [MongoDatabase("__identities")]
    public class UserDataContext : IdentityDbContext
    {
        #region Properties
        public DbSet<OneTimePassword> OneTimePasswords { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Bank> Banks { get; set; }
        
        /*
        public DbSet<User> Users { get; set; }
        public DbSet<IdentityUserClaim> UserClaims { get; set; }
        public DbSet<IdentityUserLogin> UserLogins { get; set; }
        public DbSet<IdentityUserToken> UserTokens { get; set; }
        */
        #endregion

        #region Construtctors
        public UserDataContext(DbContextOptions<UserDataContext> options) : base(options) { }
        #endregion

        #region Methods
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().Ignore(u => u.Roles);
        }
        #endregion
    }
}

using Chop9ja.API.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Data
{
    public class UserDataContext : IdentityDbContext<User>
    {
        #region Properties
        
        public DbSet<OneTimePassword> OneTimePasswords { get; set; }

        #endregion

        #region Construtctors
        public UserDataContext(DbContextOptions<UserDataContext> options) : base(options) { }
        #endregion

        #region Methods
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        #endregion
    }
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.Entities
{
    public enum UserRole
    {
        RegularUser,
        Administrator
    }

    public class User : IdentityUser
    {
        #region Properties

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string StateOfOrigin { get; set; }

        public ICollection<OneTimePassword> OneTimePasswords { get; set; } = new Collection<OneTimePassword>();

        #endregion
    }
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.Entities
{
    public class User : IdentityUser
    {
        #region Properties

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherNames { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string StateOfOrigin { get; set; }

        #endregion
    }
}

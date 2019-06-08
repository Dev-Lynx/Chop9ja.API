using Chop9ja.API.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels
{
    public class UserRegistrationViewModel
    {
        #region Properties
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Phone]
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string StateOfOrigin { get; set; }

        [Required]
        [MinLength(8)] [MaxLength(32)]
        public string Password { get; set; }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels
{
    public class UserViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string Initials => (string.IsNullOrWhiteSpace(LastName) && string.IsNullOrWhiteSpace(FirstName)) ? $"{LastName.FirstOrDefault()}{FirstName.FirstOrDefault()}" : "";

        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string StateOfOrigin { get; set; }

        [Phone]
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string PhoneNumberConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string EmailConfirmed { get; set; }

        public bool LockoutEnabled { get; set; }
    }

    public class UserTokenViewModel : UserViewModel
    {
        public string Token { get; set; }
    }

    public class UserOneTimePasswordModel : UserViewModel
    {
        public string OneTimePassword { get; set; }
    }

    public class AccountViewModel : UserViewModel
    {
        [Required]
        public Guid Id { get; set; }

        public DateTime Created { get; set; }
    }

    public class UserExistsModel
    {
        [Phone]
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}

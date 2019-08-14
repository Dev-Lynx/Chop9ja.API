using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels
{
    public class PasswordChangeViewModel
    {
        [Required]
        [MinLength(6)]
        [MaxLength(32)]
        public string CurrentPassword { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(32)]
        public string NewPassword { get; set; }
    }

    public class PasswordResetViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(32)]
        public string NewPassword { get; set; }
    }

    public class OneTimePasswordResetViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string OneTimePassword { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(32)]
        public string NewPassword { get; set; }
    }

    public class PasswordResetTokenVerificationModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Token { get; set; }
    }

    public class OneTimePasswordResetVerificationModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string OneTimePassword { get; set; }
    }

}

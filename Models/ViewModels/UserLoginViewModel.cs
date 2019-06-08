using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels
{
    /// <summary>
    /// Login Model for a user.
    /// </summary>
    public class UserLoginViewModel
    {
        /// <summary>
        /// Email of the user.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// User Password.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}

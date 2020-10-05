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
        /// Mobile Number of the user.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// User Password.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }

    /// <summary>
    /// Login Model for a user.
    /// </summary>
    public class BackOfficeLoginViewModel
    {

        public string Role { get; set; }

        /// <summary>
        /// Mobile Number of the user.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// User Password.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels.Agent
{
    public class AgentRegistrationViewModel
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
        public string Address { get; set; }

        [Required]
        public string Landmark { get; set; }

        /// <summary>
        /// State where the shop is located.
        /// </summary>
        [Required]
        public string StateOfOrigin { get; set; }

        [Required]
        public string LocalGovernment { get; set; }

        /// <summary>
        /// Password should not be less than 6 characters.
        /// </summary>
        [Required]
        [MinLength(6)]
        [MaxLength(32)]
        public string Password { get; set; }
        #endregion
    }
}

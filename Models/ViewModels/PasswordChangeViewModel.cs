using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels
{
    public class PasswordChangeViewModel
    {
        /// <summary>
        /// Password must contain a letter, number and symbol. 
        /// Password should not be less than 8 characters.
        /// </summary>
        [Required]
        [MinLength(8)]
        [MaxLength(32)]
        public string CurrentPassword { get; set; }
        /// <summary>
        /// Password must contain a letter, number and symbol. 
        /// Password should not be less than 8 characters.
        /// </summary>
        [Required]
        [MinLength(8)]
        [MaxLength(32)]
        public string NewPassword { get; set; }
    }
}

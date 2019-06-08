using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels
{
    public class SMSViewModel
    {

        /// <summary>
        /// SMS content to send. Should not be more than 160 characters.
        /// </summary>
        [Required]
        [StringLength(160, MinimumLength = 10)]
        public string Message { get; set; }
    }

    public class CustomSMSViewModel : SMSViewModel
    {
        /// <summary>
        /// User's phone number.
        /// </summary>
        [Required]
        [Phone]
        public string Phone { get; set; }
    }
}

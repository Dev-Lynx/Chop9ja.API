using Chop9ja.API.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels
{
    public class EmailViewModel
    {
        /// <summary>
        /// Subject of the email.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Body of the email. HTML is the preferred format.
        /// Should be less than 35MB.
        /// </summary>
        [Required]
        [DataType(DataType.Html)]
        public string Body { get; set; }
    }

    public class CustomEmailViewModel : EmailViewModel
    {
        /// <summary>
        /// Email of the user the email is addressed to.
        /// </summary>
        [Required]
        [EmailAddress]
        public string To { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels
{
    public class WithdrawalViewModel
    {
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public Guid BankAccountId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels
{
    public class WithdrawalViewModel
    {
        public decimal Amount { get; set; }
        public BankAccountViewModel BankAccount { get; set; }
    }
}

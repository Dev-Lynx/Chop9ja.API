using Chop9ja.API.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels
{
    public class DepositViewModel
    {
        public DateTime Date { get; set; } = DateTime.Now;
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public ChannelType PaymentChannel { get; set; }

        public Guid UserBankAccountId { get; set; }
        public Guid PlatformBankAccountId { get; set; }
        public string Description { get; set; }
    }
}

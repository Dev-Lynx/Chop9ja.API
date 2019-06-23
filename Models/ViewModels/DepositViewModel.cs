using Chop9ja.API.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels
{
    public class DepositViewModel
    {
        public decimal Amount { get; set; }
        public ChannelType PaymentChannel { get; set; }
    }
}

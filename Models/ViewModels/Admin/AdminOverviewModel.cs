using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels.Admin
{
    public class AdminOverviewModel
    {
        public DateRange DateRange { get; set; }

        public long NewUsers { get; set; }

        public long InsuredBets { get; set; }
        public long CashedOutBets { get; set; }

        public decimal TotalInsuredBetsAmount { get; set; }
        public decimal TotalCashedOutBetsAmount { get; set; }
        public decimal TotalDepositAmount { get; set; }
        public decimal TotalWithdrawalAmount { get; set; }
    }
}

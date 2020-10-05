using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels
{
    public class StatsViewModel
    {
        public double Value { get; set; }
        public double PreviousValue { get; set; }
        public double PercentageDifference { get; set; }

        public StatsViewModel() { }
        public StatsViewModel(double value, double previousValue, double difference)
        {
            Value = value;
            PreviousValue = previousValue;
            PercentageDifference = difference;
        }
    }

    public class TransactionStatsViewModel
    {
        public decimal Value { get; set; }
        public decimal PreviousValue { get; set; }
        public double PercentageDifference { get; set; }
        public List<BackOfficeTransactionViewModel> Transactions { get; set; }
    }
}

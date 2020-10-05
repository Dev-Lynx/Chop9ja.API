using Sieve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels
{
    public class DateSearchViewModel
    {
        public string SearchQuery { get; set; }
        public DateTime Start { get; set; } = DateRange.AllTime.Start;
        public DateTime End { get; set; } = DateRange.AllTime.End;
        //public DateRange DateRange { get; set; } = DateRange.AllTime;
    }

    public class SearchableSieveModel : SieveModel
    {
        public string SearchQuery { get; set; }
    }
    
    public class DateRangeSieveModel : SearchableSieveModel
    {
        public DateTime Start { get; set; } = DateRange.AllTime.Start;
        public DateTime End { get; set; } = DateRange.AllTime.End;
    }
}

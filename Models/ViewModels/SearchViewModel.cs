using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels
{
    public class SearchViewModel
    {
        public string SearchQuery { get; set; }
        public DateRange DateRange { get; set; } = DateRange.AllTime;
    }
}

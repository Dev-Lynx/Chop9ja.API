using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime TrySubtract(this DateTime date, TimeSpan value)
        {
            try { date = date.Subtract(value); }
            catch { date = DateTime.MinValue; }
            return date;
        }
    }
}

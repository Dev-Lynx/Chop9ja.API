using Chop9ja.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Extensions
{
    public static class TimespanExtensions
    {
        public static string ToHumanReadableString(this TimeSpan t)
        {
            if (t.TotalSeconds <= 1)
            {
                return $@"{t:s\.ff} seconds";
            }
            if (t.TotalMinutes <= 1)
            {
                return $@"{t:%s} seconds";
            }
            if (t.TotalHours <= 1)
            {
                return $@"{t:%m} minutes";
            }
            if (t.TotalDays <= 1)
            {
                return $@"{t:%h} hours";
            }

            return $@"{t:%d} days";
        }

        public static string ToHumanReadableString(this TimeRange range)
        {
            TimeSpan from = range.From;
            TimeSpan to = range.To;

            if (to.TotalSeconds <= 1) return $@"{from:s\.ff} - {to:s\.ff} seconds";
            if (to.TotalMinutes <= 1) return $@"{from:%s} - {to:%s} seconds";
            if (to.TotalHours <= 1) return $@"{from:%m} - {to:%m} minutes";
            if (to.TotalDays <= 1) return $@"{from:%h} - {to:%h} hours";

            return $@"{from:%d} - {to:%d} days";
        }
    }
}

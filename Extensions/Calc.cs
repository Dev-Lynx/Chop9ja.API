using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Extensions
{
    public static class Calc
    {
        public static double GetPercentageDifference(double current, double predicate)
        {
            double difference = current - predicate;
            if (current == 0) current = 1;
            return Math.Round((difference / ((current + predicate) / 2)) * 100, 2);
        }
    }
}

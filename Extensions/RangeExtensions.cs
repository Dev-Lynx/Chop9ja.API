using Chop9ja.API.Models;
using MongoDbGenericRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Chop9ja.API.Extensions
{
    public static class RangeExtensions
    {
        public static Expression<Func<TEntity, bool>> Includes<TEntity>(this DateRange range) where TEntity : Document
        {
            DateTime startUtc = range.Start.ToUniversalTime();
            DateTime endUtc = range.End.ToUniversalTime();

            return d => startUtc <= d.AddedAtUtc && endUtc >= d.AddedAtUtc;
        }
    }
}

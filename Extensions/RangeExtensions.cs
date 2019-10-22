using Chop9ja.API.Models;
using Chop9ja.API.Models.Entities;
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

        public static Expression<Func<User, bool>> IncludesUser(this DateRange range)
        {
            DateTime startUtc = range.Start.ToUniversalTime();
            DateTime endUtc = range.End.ToUniversalTime();

            return d => startUtc <= d.CreatedOn && endUtc >= d.CreatedOn;
        }

        public static Expression<Func<PaymentRequest, bool>> IncludesRequest(this DateRange range)
        {
            DateTime startUtc = range.Start.ToUniversalTime();
            DateTime endUtc = range.End.ToUniversalTime();

            return d => startUtc <= d.TransactionDate && endUtc >= d.TransactionDate;
        }
    }
}

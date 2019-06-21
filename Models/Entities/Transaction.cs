using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.Entities
{
    public enum TransactionType
    {
        Debit, Credit
    }

    public enum TransactionStatus
    {
        Idle, Success, Failed
    }

    [Owned]
    public class Transaction
    {
        public decimal Amount { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime CompletionTime { get; set; } = DateTime.MinValue;

        [BsonIgnoreIfDefault]
        public Guid PaymentMethodId { get; set; }

        PaymentMethod _paymentMethod;
        [BsonIgnore]
        public PaymentMethod PaymentMethod
        {
            get
            {
                if (_paymentMethod == null || _paymentMethod.Id == Guid.Empty)
                    _paymentMethod = Core.DataContext.Store.GetById<PaymentMethod>(PaymentMethodId);
                return _paymentMethod;
            }
            set
            {
                if (_paymentMethod != null)
                    PaymentMethodId = value.Id;
                _paymentMethod = value;
            }
        }

        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; }
    }

    
}

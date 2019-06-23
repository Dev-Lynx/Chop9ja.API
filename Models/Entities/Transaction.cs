using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using MongoDbGenericRepository.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.Entities
{

    #region Enumerations
    public enum TransactionType
    {
        Deposit, Withdrawal
    }
    #endregion

    #region Entity
    [Owned]
    [BsonIgnoreExtraElements]
    public class Transaction : Document
    {
        public decimal Amount { get; set; }

        [BsonIgnoreIfDefault]
        public Guid PaymentChannelId { get; set; }

        PaymentChannel _paymentChannel;
        [BsonIgnore]
        public PaymentChannel PaymentChannel
        {
            get
            {
                if (_paymentChannel == null || _paymentChannel.Id == Guid.Empty)
                    _paymentChannel = Core.DataContext.Store.GetById<PaymentChannel>(PaymentChannelId);
                return _paymentChannel;
            }
            set
            {
                if (value != null)
                    PaymentChannelId = value.Id;
                _paymentChannel = value;
            }
        }

        [BsonRepresentation(BsonType.String)]
        public TransactionType Type { get; set; }
    }
    #endregion
}


namespace Chop9ja.API.Models.ViewModels
{
    #region ViewModel

    public class TransactionViewModel
    {
        public decimal Amount { get; set; }
        public Entities.TransactionType Type { get; set; }
        public PaymentChannelViewModel PaymentMethod { get; set; }
    }

    #endregion
}

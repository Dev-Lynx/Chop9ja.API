using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using MongoDbGenericRepository.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.Entities
{
    public enum RequestStatus
    {
        Pending, 
        Approved,
        Declined
    }

    [ComplexType]
    [BsonIgnoreExtraElements]
    [CollectionName("PaymentRequests")]
    [BsonKnownTypes(typeof(DepositPaymentRequest))]
    [BsonDiscriminator(RootClass = true, Required = true)]
    public class PaymentRequest : Document
    {
        public decimal Amount { get; set; }

        [BsonIgnoreIfDefault]
        public Guid UserId { get; set; }
        [BsonIgnoreIfDefault]
        public Guid PaymentChannelId { get; set; }

        public Guid UserBankAccountId { get; set; }

        public TransactionType TransactionType { get; set; }
        public RequestStatus Status { get; set; }

        User _user;
        [BsonIgnore]
        public User User
        {
            get
            {
                if (_user == null)
                    _user = Core.DataContext.Store.GetById<User>(UserId);
                return _user;
            }
            set
            {
                if (value != null)
                    UserId = value.Id;
                _user = value;
            }
        }

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

        BankAccount _userBankAccount;
        [BsonIgnore]
        public BankAccount UserBankAccount
        {
            get
            {
                if (_userBankAccount == null || _userBankAccount.Id == Guid.Empty)
                    _userBankAccount = User.BankAccounts.FirstOrDefault(b => b.Id == UserBankAccountId);
                return _userBankAccount;
            }
        }
    }

    [CollectionName("PaymentRequests")]
    public class DepositPaymentRequest : PaymentRequest
    {
        [BsonIgnoreIfDefault]
        public Guid PlatformBankAccountId { get; set; }

        BankAccount _platformBankAccount;
        [BsonIgnore]
        public BankAccount PlatformBankAccount
        {
            get
            {
                if (_platformBankAccount == null || _platformBankAccount.Id == Guid.Empty)
                    _platformBankAccount = User.BankAccounts.FirstOrDefault(b => b.Id == PlatformBankAccountId);
                return _platformBankAccount;
            }
        }
    }
}

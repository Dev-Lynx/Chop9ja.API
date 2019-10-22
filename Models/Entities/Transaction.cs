using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using MongoDbGenericRepository.Models;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.Entities
{
    #region Entity
    [ComplexType]
    [BsonIgnoreExtraElements]
    [BsonKnownTypes(typeof(BankTransaction))]
    [BsonDiscriminator(Required = true, RootClass = true)]
    public class Transaction : Document
    {
        #region Properties
        public DateTime AddedAt => AddedAtUtc.ToLocalTime();
        public decimal Amount { get; set; }
        public bool Completed { get; set; }

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

        public Guid AuxilaryUserId { get; set; }

        User _auxilaryUser;
        [BsonIgnore]
        public User AuxilaryUser
        {
            get
            {
                if (_auxilaryUser == null || _auxilaryUser.Id == Guid.Empty)
                    _auxilaryUser = Core.DataContext.Store.GetById<User>(AuxilaryUserId);
                return _auxilaryUser;
            }
            set
            {
                if (value != null) AuxilaryUserId = value.Id;
                _auxilaryUser = value;
            }
        }
        #endregion

        #region Constructors
        public Transaction() { }
        public Transaction(Transaction transaction)
        {
            Id = transaction.Id;
            AddedAtUtc = transaction.AddedAtUtc;
            Amount = transaction.Amount;
            PaymentChannelId = transaction.PaymentChannelId;
            Type = transaction.Type;
            AuxilaryUserId = transaction.AuxilaryUserId;
        }
        #endregion
    }

    
    public class BankTransaction : Transaction
    {
        public Guid AuxilaryBankAccountId { get; set; }

        BankAccount _auxilaryBankAccount;
        [BsonIgnore]
        public BankAccount AuxilaryBankAccount
        {
            get
            {
                if (_auxilaryBankAccount == null || _auxilaryBankAccount.Id == Guid.Empty)
                    _auxilaryBankAccount = AuxilaryUser.BankAccounts.FirstOrDefault(b => b.Id == AuxilaryBankAccountId);
                return _auxilaryBankAccount;
            }
            set
            {
                if (value != null) AuxilaryBankAccountId = value.Id;
                _auxilaryBankAccount = value;
            }
        }
    }
    #endregion
}


#region ViewModel
namespace Chop9ja.API.Models.ViewModels
{
    #region BackOffice

    public class BackOfficeTransactionViewModel : TransactionViewModel
    {
        public Guid Id { get; set; }
        public UserViewModel AuxilaryUser { get; set; }
    }

    #endregion

    

    public class TransactionViewModel
    {
        public DateTime AddedAt { get; set; }
        public decimal Amount { get; set; }
        [Sieve(CanFilter = true)]
        public Entities.TransactionType Type { get; set; }
        public PaymentChannelViewModel PaymentChannel { get; set; }
    }
}

#endregion

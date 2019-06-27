using Blueshift.EntityFrameworkCore.MongoDB.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using MongoDbGenericRepository.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Unity;

namespace Chop9ja.API.Models.Entities
{
    [ComplexType]
    [BsonIgnoreExtraElements]
    [CollectionName("Wallets")]
    public class Wallet : Document
    {
        #region Properties
        public decimal Balance { get; set; }
        public decimal AvailableBalance { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new Collection<Transaction>();

        [BsonIgnoreIfDefault]
        public Guid UserId { get; set; }

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
        #region Payment Authorization
        [BsonIgnoreIfDefault]
        public string PaystackAuthorization { get; set; }
        #endregion


        #region Internals
        ILogger Logger { get; } = Core.Container.Resolve<ILogger<Wallet>>();
        #endregion

        #endregion

        #region Methods
        public async Task AddTransactionAsync(Transaction transaction)
        {
            if (transaction == null) throw new InvalidOperationException("Transaction cannot be null");

            if (Transactions.Any(t => t.Id == transaction.Id)) return;

            if (await EvaluateTransactionAsync(transaction))
                Transactions.Add(transaction);

            await Core.DataContext.Store.UpdateOneAsync(this);
        }

        async Task<bool> EvaluateTransactionAsync(Transaction transaction)
        { 
            switch (transaction.Type)
            {
                case TransactionType.Deposit:
                    Balance += transaction.Amount;
                    AvailableBalance += transaction.Amount;
                    break;

                case TransactionType.Withdrawal:
                    decimal balance = Balance - transaction.Amount;

                    if (balance < 0)
                    {
                        Logger.LogError("Attempted to withdraw a higher value " +
                            "({1}) than the current wallet ({0}) balance.", Id, transaction.Amount);
                        return false;
                    }
                    Balance = balance;
                    break;
            }

            await Core.DataContext.Store.UpdateOneAsync(this);

            return true;
        }
        #endregion

    }
}

namespace Chop9ja.API.Models.ViewModels
{
    public class WalletViewModel
    {
        public decimal Balance { get; set; }
        public decimal AvailableBalance { get; set; }

        public List<TransactionViewModel> Transactions { get; set; } = new List<TransactionViewModel>();
    }
}
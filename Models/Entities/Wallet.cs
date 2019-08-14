using Blueshift.EntityFrameworkCore.MongoDB.Annotations;
using Chop9ja.API.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDbGenericRepository.Attributes;
using MongoDbGenericRepository.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
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

        public List<Guid> TransactionIds { get; set; } = new List<Guid>();

        IEnumerable<Transaction> _transactions = Enumerable.Empty<Transaction>();
        public IEnumerable<Transaction> Transactions
        {
            get
            {
                if (_transactions.LongCount() != TransactionIds.LongCount())
                    _transactions = GetTransactions().Result;
                return _transactions;
            }
        }

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

        bool? _isPlatform = null;
        public bool? IsPlatform
        {
            get
            {
                if (_isPlatform == null)
                    _isPlatform = User != null ? User.IsPlatform : false;
                return _isPlatform;
            }
            set => _isPlatform = value;
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
            
            if (await EvaluateTransactionAsync(transaction))
            {
                TransactionIds.Add(transaction.Id);

                if (!(bool)IsPlatform)
                    await Core.DataContext.Store.AddOneAsync(transaction);
            }
            else
            {
                Logger.LogError($"An attempt to make an invalid transaction was made. " +
                    $"Transaction ({transaction.Id}) is now orphaned.");
                throw new InvalidOperationException($"Attempted to make an invalid transaction. Transaction Id is {transaction.Id}");
            }

            await Core.DataContext.Store.UpdateOneAsync(this);
        }

        async Task<bool> EvaluateTransactionAsync(Transaction transaction)
        {
            if (transaction.Completed) return false;
            if (TransactionIds.Any(id => id == transaction.Id)) return false;

            if (transaction.Amount < 0)
                return false;

            decimal balance = 0M;
            switch (transaction.Type)
            {
                case TransactionType.Deposit:
                    Balance += transaction.Amount;
                    AvailableBalance += transaction.Amount;
                    break;

                case TransactionType.Withdrawal:
                    balance = Balance - transaction.Amount;

                    if (balance < 0)
                    {
                        Logger.LogError("Attempted to withdraw a higher value " +
                            "({1}) than the current wallet ({0}) balance.", Id, transaction.Amount);
                        return false;
                    }
                    Balance = balance;
                    AvailableBalance -= transaction.Amount;
                    break;

                case TransactionType.Credit:
                    if ((bool)IsPlatform) break;
                    Balance += transaction.Amount;
                    AvailableBalance += transaction.Amount;
                    break;

                case TransactionType.Debit:
                    if ((bool)IsPlatform) break;
                    balance = Balance - transaction.Amount;

                    if (balance < 0)
                    {
                        Logger.LogError("Attempted to withdraw a higher value " +
                            "({1}) than the current wallet ({0}) balance.", Id, transaction.Amount);
                        return false;
                    }
                    Balance = balance;
                    AvailableBalance -= transaction.Amount;
                    break;
            }

            transaction.Completed = true;
            await Core.DataContext.Store.UpdateOneAsync(this);

            return true;
        }

        public async Task<IEnumerable<Transaction>> GetTransactions(Expression<Func<Transaction, bool>> filter = null)
        {
            if (filter == null) filter = t => TransactionIds.Contains(t.Id);
            else filter = filter.CombineWithAndAlso(t => TransactionIds.Contains(t.Id));

            // var transactions = await Core.DataContext.Transactions.FindAsync(filter);

            return await Core.DataContext.Store.GetAllAsync(filter);
        }

        public async Task<long> CountTransactions(Expression<Func<Transaction, bool>> filter = null)
        {
            if (filter == null) filter = t => TransactionIds.Contains(t.Id);
            else filter = filter.CombineWithAndAlso(t => TransactionIds.Contains(t.Id));

            return await Core.DataContext.Store.CountAsync(filter);
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
    }
}
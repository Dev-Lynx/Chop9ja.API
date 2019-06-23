using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDbGenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Data
{
    [AutoBuild]
    public class MongoDataContext : MongoDbContext
    {
        #region Properties

        #region Collections
        public IMongoCollection<User> Users => GetCollection<User>();
        public IMongoCollection<Wallet> Wallets => GetCollection<Wallet>();
        public IMongoCollection<Transaction> Transactions => GetCollection<Transaction>();
        public IMongoCollection<Bank> Banks => GetCollection<Bank>();
        #endregion

        #region Services
        [DeepDependency]
        ILogger Logger { get; }
        [DeepDependency]
        public IMongoRepository Store { get; }
        #endregion

        #region Statics
        public static MongoDataContext Current { get; private set; }
        #endregion

        #endregion


        #region Constructors
        public MongoDataContext(MongoClient client, string databaseName) : base(client, databaseName) { Current = this; }
        public MongoDataContext(string connectionString, string databaseName) : base(connectionString, databaseName) { Current = this; }
        #endregion
    }
}

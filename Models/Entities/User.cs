using AspNetCore.Identity.Mongo.Model;
using AspNetCore.Identity.MongoDbCore.Models;
using Blueshift.EntityFrameworkCore.MongoDB.Annotations;
using Blueshift.Identity.MongoDB;
using Microsoft.AspNetCore.Identity;
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

namespace Chop9ja.API.Models.Entities
{
    public enum UserRoles
    {
        RegularUser,
        Administrator
    }

    [BsonIgnoreExtraElements]
    [CollectionName("Users")]
    public class User : MongoIdentityUser, IDocument
    {
        #region Properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string StateOfOrigin { get; set; }

        [BsonIgnoreIfDefault]
        public Guid WalletId { get; set; }

        Wallet _wallet;
        [BsonIgnore]
        public Wallet Wallet
        {
            get
            {
                if (_wallet == null)
                    _wallet = Core.DataContext.Store.GetById<Wallet>(WalletId);

                return _wallet;
            }
            set
            {
                if (value != null)
                    WalletId = value.Id;
                _wallet = value;
            }
        }


        public List<OneTimePassword> OneTimePasswords { get; set; } = new List<OneTimePassword>();

        public List<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
        #endregion

        #region Methods
        public async Task InitializeAsync()
        {
            Wallet = new Wallet();
            Wallet.User = this;
            await Core.DataContext.Store.AddOneAsync(Wallet);

            WalletId = Wallet.Id;
            await Core.DataContext.Store.UpdateOneAsync(this);
        }
        #endregion
    }
}

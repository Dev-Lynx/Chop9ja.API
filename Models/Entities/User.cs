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
                if (_wallet == null || _wallet.Id == Guid.Empty)
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

        public List<Guid> OneTimePasswordIds { get; set; } = new List<Guid>();

        IEnumerable<OneTimePassword> _oneTimePasswords;
        [BsonIgnore]
        public IEnumerable<OneTimePassword> OneTimePasswords
        {
            get
            {
                if (_oneTimePasswords == null || _oneTimePasswords.Count() != OneTimePasswordIds.Count)
                    _oneTimePasswords = Core.DataStore.GetAll<OneTimePassword>(o => OneTimePasswordIds.Contains(o.Id));
                return _oneTimePasswords == null ? Enumerable.Empty<OneTimePassword>() : _oneTimePasswords;
            }
        }

        public List<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
        #endregion
    }
}

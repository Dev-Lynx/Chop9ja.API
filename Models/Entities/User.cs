using AspNetCore.Identity.Mongo.Model;
using AspNetCore.Identity.MongoDbCore.Models;
using Blueshift.EntityFrameworkCore.MongoDB.Annotations;
using Blueshift.Identity.MongoDB;
using Chop9ja.API.Extensions;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using MongoDbGenericRepository.Models;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.Entities
{
    public enum UserRoles
    {
        Regular,
        Administrator,
        Agent,
        Platform
    }

    [BsonIgnoreExtraElements]
    [CollectionName("Users")]
    public class User : MongoIdentityUser, IDocument
    {
        #region Properties
        public DateTime Created { get; set; } = DateTime.Now;
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

        public List<Guid> PaymentRequestIds { get; set; } = new List<Guid>();

        IEnumerable<PaymentRequest> _paymentRequests = Enumerable.Empty<PaymentRequest>();
        public IEnumerable<PaymentRequest> PaymentRequests
        {
            get
            {
                if (_paymentRequests.Count() != PaymentRequestIds.Count)
                    _paymentRequests = Core.DataContext.Store
                        .GetAll<PaymentRequest>(r => PaymentRequestIds.Contains(r.Id));
                return _paymentRequests;
            }
        }

        public List<Guid> BetIds { get; set; } = new List<Guid>();

        IEnumerable<Bet> _bets = Enumerable.Empty<Bet>();
        public IEnumerable<Bet> Bets
        {
            get
            {
                if (!BetsLoaded) GetBetsAsync().Wait();
                return _bets;
            }
        }

        public bool BetsLoaded => _bets.Count() == BetIds.Count;


        public List<OneTimePassword> OneTimePasswords { get; set; } = new List<OneTimePassword>();

        public List<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();

        public bool IsPlatform { get; set; }

        [NotMapped]
        public string FormattedPhoneNumber => Core.Phone.Format(Phone, PhoneNumberFormat.E164);
        [NotMapped]
        public string InternationalPhoneNumber => Core.Phone.Format(Phone, PhoneNumberFormat.INTERNATIONAL);

        #region Internals
        PhoneNumber Phone
        {
            get
            {
                try
                {
                    var phone = Core.Phone.Parse(PhoneNumber, "NG");
                    return phone;
                }
                catch { return new PhoneNumber(); }
            }
        }
        #endregion

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

        public async Task AddPaymentRequestAsync(PaymentRequest request)
        {
            request.User = this;
            await Core.DataContext.Store.AddOneAsync(request);

            PaymentRequestIds.Add(request.Id);
            await Core.DataContext.Store.UpdateOneAsync(this);
        }

        public async Task AddBetAsync(Bet bet)
        {
            bet.User = this;
            await Core.DataContext.Store.AddOneAsync(bet);

            BetIds.Add(bet.Id);
            await Core.DataContext.Store.UpdateOneAsync(this);
        }

        public async Task<IEnumerable<Bet>> GetBetsAsync(Expression<Func<Bet, bool>> filter = null)
        {
            if (filter == null) filter = b => BetIds.Contains(b.Id);
            else filter = filter.CombineWithAndAlso(b => BetIds.Contains(b.Id));

            _bets = await Core.DataContext.Store.GetAllAsync(filter);
            return _bets;
        }
        #endregion
    }
}

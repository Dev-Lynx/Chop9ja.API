using Blueshift.EntityFrameworkCore.MongoDB.Annotations;
using Microsoft.EntityFrameworkCore;
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
    [ComplexType]
    [CollectionName("Bets")]
    [MongoCollection("Bets")]
    public class Bet : Document
    {
        public DateTime AddedAt => AddedAtUtc.ToLocalTime();
        public DateTime CashedOutOn => CashedOutOnUtc.ToLocalTime();

        public int PlatformId { get; set; }

        BetPlatform _platform;
        [BsonIgnore]
        public BetPlatform Platform
        {
            get
            {
                if (_platform == null)
                    _platform = Core.DataContext.Store.GetById<BetPlatform, int>(PlatformId);
                return _platform;
            }
            set
            {
                if (value != null) PlatformId = _platform.Id;
                _platform = value;
            }
        }

        public Guid UserId { get; set; }
        public string Username { get; set; }

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
                {
                    UserId = value.Id;
                    Username = value.UserName;
                }
                _user = value;
            }
        }

        public string SlipNumber { get; set; }

        public double Odds { get; set; }
        public decimal Stake { get; set; }
        public decimal PotentialWinnings { get; set; }

        public bool CashOutRequested { get; set; }
        public DateTime CashedOutOnUtc { get; set; }

        public RequestStatus CashOutStatus { get; set; }
    }

    public class BetViewModel
    {
        [Required]
        [Sieve(CanSort = true)]
        public DateTime Date { get; set; }
        [Required]
        public int PlatformId { get; set; }

        [Required]
        public string SlipNumber { get; set; }

        [Required]
        public double Odds { get; set; }
        [Required]
        public decimal Stake { get; set; }
        [Required]
        public decimal PotentialWinnings { get; set; }
        [Required]
        [Sieve(CanFilter = true)]
        public bool CashedOut { get; set; }
    }

    public class StagedBetViewModel : BetViewModel
    {
        [Required]
        public RequestStatus Status { get; set; }
        [Required]
        public DateTime CashedOutOn { get; set; }
    }

    public class BackOfficeClaimViewModel : StagedBetViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }

    public class StatusUpdateViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public RequestStatus Status { get; set; }
    }

    public class CashOutViewModel
    {
        public string SlipNumber { get; set; }
    }
}

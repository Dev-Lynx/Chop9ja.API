using Blueshift.EntityFrameworkCore.MongoDB.Annotations;
using Microsoft.EntityFrameworkCore;
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
    [ComplexType]
    [BsonIgnoreExtraElements]
    [CollectionName("Wallets")]
    public class Wallet : Document
    {
        public decimal Balance { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new Collection<Transaction>();
    }
}

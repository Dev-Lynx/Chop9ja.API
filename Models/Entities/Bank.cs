using Blueshift.EntityFrameworkCore.MongoDB.Annotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using MongoDbGenericRepository.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.Entities
{
    [ComplexType]
    [CollectionName("Banks")]
    [MongoCollection("Banks")]
    public class Bank : IDocument<int>
    {
        [Key]
        [BsonId]
        public int Id { get; set; }
        public int Version { get; set; }
        public DateTime AddedAtUtc { get; set; } = DateTime.UtcNow;

        public string Name { get; set; }
        public string Code { get; set; }
    }
}

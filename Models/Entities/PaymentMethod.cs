using Blueshift.EntityFrameworkCore.MongoDB.Annotations;
using Microsoft.EntityFrameworkCore;
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
    [MongoCollection("PaymentMethods")]
    [CollectionName("PaymentMethods")]
    public class PaymentMethod : Document
    {
        public string Name { get; set; }
        public string Logo { get; set; }
        public string Description { get; set; }

        public bool ContainsFee { get; set; }
        public double FeePercentage { get; set; }

        public TimeRange PaymentRange { get; set; }
    }

    [Owned]
    public class TimeRange
    {
        public TimeRange(TimeSpan from, TimeSpan to)
        {
            From = from;
            To = to;
        }

        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
    }
}

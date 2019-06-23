using Blueshift.EntityFrameworkCore.MongoDB.Annotations;
using Chop9ja.API.Extensions;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using MongoDbGenericRepository.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models
{
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

        public override string ToString() => this.ToHumanReadableString();
    }
}

namespace Chop9ja.API.Models.Entities
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ChannelType
    {
        BankDeposit,
        Paystack
    }

    [ComplexType]
    [MongoCollection("PaymentChannels")]
    [CollectionName("PaymentChannels")]
    public class PaymentChannel : Document
    {
        public string Name { get; set; }
        public string Logo { get; set; }

        public string Currency { get; set; }
        public decimal ConversionRate { get; set; }

        public string Description { get; set; }

        [BsonRepresentation(BsonType.String)]
        public ChannelType Type { get; set; }

        public bool ContainsFee { get; set; }
        public string Fee { get; set; }

        public TimeRange PaymentRange { get; set; }
    }    
}

namespace Chop9ja.API.Models.ViewModels
{
    public class PaymentChannelViewModel
    {
        public string Name { get; set; }
        public string Logo { get; set; }
        public string Description { get; set; }

        public bool ContainsFee { get; set; }
        public string Fee { get; set; }

        public string PaymentRange { get; set; }
    }
}

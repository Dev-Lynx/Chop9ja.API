using Blueshift.EntityFrameworkCore.MongoDB.Annotations;
using Chop9ja.API.Extensions;
using Chop9ja.API.Models.Entities;
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
        Bank = 1,
        Paystack = 2
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

        public bool UsesFeePercentage { get; set; }
        public decimal FeePercentage { get; set; }

        public bool UsesFixedFee { get; set; }
        public decimal FixedFee { get; set; }

        public TimeRange PaymentRange { get; set; }
    }    
}

namespace Chop9ja.API.Models.ViewModels
{
    public class PaymentChannelViewModel
    {
        /// <summary>
        /// Name of payment channel
        /// </summary>
        [Required] public string Name { get; set; }

        /// <summary>
        /// Link to payment channel's logo
        /// </summary>
        [Required] public string Logo { get; set; }

        [Required] public string Description { get; set; }

        /// <summary>
        /// Indicates that the platform charges a percentage of the 
        /// original amount during payment.
        /// 
        /// Note: A platform can use both a fee percentage and a fixed fee.
        /// For a consistent fee, use the following formula:
        /// 
        /// fee = (amount * feePercentage) + fixedFee
        /// </summary>
        [Required] public bool UsesFeePercentage { get; set; }

        /// <summary>
        /// Percentage the platform charges during payment.
        /// </summary>
        [Required] public decimal FeePercentage { get; set; }

        /// <summary>
        /// Indicates that the platform charges a fixed fee
        /// during payment.
        /// 
        /// Note: A platform can use both a fee percentage and a fixed fee.
        /// For a consistent fee, use the following formula:
        /// 
        /// fee = (amount * feePercentage) + fixedFee
        /// </summary>
        [Required] public bool UsesFixedFee { get; set; }

        /// <summary>
        /// Fixed amount the platform charges during payment.
        /// </summary>
        [Required] public decimal FixedFee { get; set; }

        /// <summary>
        /// An estimation of the time this payment channel 
        /// would take to complete a payment.
        /// </summary>
        [Required] public string PaymentRange { get; set; }

        public ChannelType Type { get; set; }
    }
}

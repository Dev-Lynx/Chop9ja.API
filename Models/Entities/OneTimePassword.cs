using Blueshift.EntityFrameworkCore.MongoDB.Annotations;
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

namespace Chop9ja.API.Models.Entities
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OnePasswordType
    {
        /// <summary>
        /// One Time Password for email.
        /// </summary>
        Email,
        /// <summary>
        /// One Time Password for Phone\SMS.
        /// </summary>
        Phone
    };


    [ComplexType]
    [BsonIgnoreExtraElements]
    [CollectionName("OneTimePasswords")]
    [MongoCollection("OneTimePasswords")]
    public class OneTimePassword : Document
    {
        #region Properties

        #region Statics
        /// <summary>
        /// Life Span of an email one time password in minutes
        /// </summary>
        public const double EmailLifeSpan = 1440;

        /// <summary>
        /// Life Span of an SMS one time password in seconds.
        /// </summary>
        public const double SmsLifeSpan = 5;
        #endregion

        public string Code { get; set; }

        public TimeSpan LifeSpan
        {
            get
            {
                switch (Kind)
                {
                    default:
                        return TimeSpan.FromMinutes(SmsLifeSpan);

                    case OnePasswordType.Email:
                        return TimeSpan.FromMinutes(EmailLifeSpan);
                }
            }
        }
        public DateTime Created { get; set; } = DateTime.Now;

        [JsonConverter(typeof(StringEnumConverter))]
        public OnePasswordType Kind { get; set; }

        bool _isActive = true;
        public bool IsActive { get => !IsExpired && _isActive && !Validated; set => _isActive = value; }

        public bool Validated { get; set; }

        public bool IsExpired => Created.ToLocalTime().Add(LifeSpan) < DateTime.Now;
        #endregion

        #region Methods
        public bool Validate(string code)
        {
            Validated = Code == code;
            return Validated;
        }
        #endregion
    }
}

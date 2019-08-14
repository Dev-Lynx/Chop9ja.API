using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.Entities
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        Credit,
        Debit
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ChannelType
    {
        Chop9ja = 0,
        Bank = 1,
        Paystack = 2
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RequestStatus
    {
        Pending = 0,
        Approved = 1,
        Declined = 2
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.Responses
{
    public class PaystackTransactionResponse
    {
        public string Event { get; set; }
        public PaystackTransactionData Data { get; set; }
    }

    public class PaystackTransactionData
    {
        public string Id { get; set; }
        public string Domain { get; set; }
        public string Reference { get; set; }
        public string Amount { get; set; }
        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime PaidAt { get; set; }

        public string GateWayResponse { get; set; }
        public decimal Fees { get; set; }
        public Dictionary<string, object> Metadata { get; set; }

        public PaystackPaymentAuthorization Authorization { get; set; }
    }

    public class PaystackPaymentAuthorization
    {
        public string AuthorizationCode { get; set; }
        public string Bank { get; set; }
        public string Brand { get; set; }
        public bool Reusable { get; set; }
        public string Signature { get; set; }
    }
}

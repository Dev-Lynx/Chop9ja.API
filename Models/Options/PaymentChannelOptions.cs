using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.Options
{
    public class PaystackOptions
    {
        public const string ConfigKey = "Paystack";

        public string PublicKey { get; set; }
        public string SecretKey { get; set; }
        public string TransactionEvent { get; set; }
    }
}

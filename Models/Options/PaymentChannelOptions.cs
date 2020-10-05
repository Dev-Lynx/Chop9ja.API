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

        string _callbackUrl = string.Empty;
        public string CallbackUrl
        {
            get => _callbackUrl.Replace("{env}", Core.ONLINE_BASE_ADDRESS);
            set => _callbackUrl = value;
        }

        public PaystackOptions() { }
        public PaystackOptions(PaystackOptions options)
        {
            PublicKey = options.PublicKey;
            SecretKey = options.SecretKey;
            TransactionEvent = options.TransactionEvent;
            CallbackUrl = options.CallbackUrl;
        }
    }
}

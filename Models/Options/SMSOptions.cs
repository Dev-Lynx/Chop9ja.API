using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.Options
{
    public class SMSOptions
    {
        public const string ConfigKey = nameof(SMSOptions);

        public string SenderID { get; set; }
        public string SecretKey { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string MessageTemplate { get; set; }
        public string BalanceTemplate { get; set; }
    }
}

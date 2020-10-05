using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.Options
{
    public class EmailSettings
    {
        public const string ConfigKey = nameof(EmailSettings);

        public string Domain { get; set; }
        public int Port { get; set; }
        public string EmailAddress { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string RefreshToken { get; set; }
    }
}

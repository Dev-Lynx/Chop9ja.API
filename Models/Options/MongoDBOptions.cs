using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.Options
{
    public class MongoDBOptions
    {
        #region Properties
        public static string CONFIG_KEY = "MongoDB";

        public string Username { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
        #endregion
    }
}

using Chop9ja.API.Extensions.UnityExtensions;
using NLog;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Extensions
{
    public static class PhoneExtensions
    {
        #region Properties
        static ILogger Logger { get; } = LogManager.GetCurrentClassLogger();
        #endregion

        public static bool TryParse(this PhoneNumberUtil phoneUtil, string phoneNumber, string defaultRegion, out PhoneNumber phone)
        {
            phone = new PhoneNumber();
            try
            {
                phone = phoneUtil.Parse(phoneNumber, defaultRegion);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An error occured while parsing a phone number");
            }
            return false;
        }
    }
}

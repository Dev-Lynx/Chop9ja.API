using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Options;
using Chop9ja.API.Models.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chop9ja.API.Services.Interfaces
{
    [AutoBuild]
    public class SmsService : ISmsService
    {
        #region Properties

        public SMSOptions Options => SMSOptions.Value;

        #region Services
        [DeepDependency]
        IOptions<SMSOptions> SMSOptions { get; }
        [DeepDependency]
        ILogger Logger { get; }
        #endregion


        #endregion

        #region Methods
        public async Task<bool> CanSend()
        {
            double balance = await GetBalance();
            return !double.IsNaN(balance) && balance > .0;
        }

        public async Task<double> GetBalance()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpContent content = null;
                try
                {
                    Uri uri = new Uri(string.Format(Options.BalanceTemplate, Options.Username, Options.Password));
                    var response = await client.GetAsync(uri);

                    content = response.Content;
                    
                    string value = await content.ReadAsStringAsync();
                    
                    return double.Parse(value);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "An error occured while checking sms remaining balance. " +
                        "SMS API Response was:\n{0}", content);
                }
                return double.NaN;
            }
        }

        public async Task<bool> SendMessage(string phoneNumber, string message)
        {
            bool success = false;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    if (message.Length > 160)
                        message = message.Substring(0, 160);

                    Uri uri = new Uri(string.Format(Options.MessageTemplate, 
                        Options.Username, Options.Password, Options.SenderID, 
                        phoneNumber, message));
                    var response = await client.GetAsync(uri);

                    success = response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "An error occured while sending a message to ({0})", phoneNumber);
                }
                return success;
            }
        }
        #endregion
    }
}

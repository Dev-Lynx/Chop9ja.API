using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Options;
using Chop9ja.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Chop9ja.API.Services.Mail
{
    public class SendGridEmailService : IEmailService
    {
        #region Properties

        public EmailSettings Settings => EmailOptions.Value;

        #region Services
        [DeepDependency]
        IOptions<EmailSettings> EmailOptions { get; }
        [DeepDependency]
        ILogger Logger { get; }
        #endregion


        #endregion

        #region Methods

        #region IEmailService Implementation

        public async Task<bool> SendEmailAsync(string destinationMail, string subject, string body)
        {
            try
            {
                var client = new SendGridClient(Settings.ClientSecret);
                var from = new EmailAddress(Settings.EmailAddress, Settings.ClientID);
                var to = new EmailAddress(destinationMail);

                var mail = MailHelper.CreateSingleEmail(from, to, subject, body, body);

                CancellationTokenSource cancel = new CancellationTokenSource(TimeSpan.FromMinutes(3));
                var response = await client.SendEmailAsync(mail, cancel.Token);

                return response.StatusCode == HttpStatusCode.Accepted
                    || response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"An error occured while sending an email using {this}.");
                return false;
            }
        }

        #endregion

        #endregion
    }
}

using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Options;
using Chop9ja.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GMailV1 = Google.Apis.Gmail.v1;
using GMailMessage = Google.Apis.Gmail.v1.Data.Message;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.Extensions.Logging;
using Google.Apis.Services;
using Google.Apis.Gmail.v1;

namespace Chop9ja.API.Services.Mail
{
    [AutoBuild]
    public class GoogleMailService : IEmailService
    {
        public EmailSettings Settings => EmailOptions.Value;

        #region Services
        [DeepDependency]
        IOptions<EmailSettings> EmailOptions { get; }
        [DeepDependency]
        ILogger Logger { get; }
        #endregion

        #region Methods

        #region IEmailService Implementation
        public async Task<bool> SendEmailAsync(string destinationMail, string subject, string msg)
        {
            if (!InternetAddress.TryParse(destinationMail, out InternetAddress to))
                return false;
            if (!InternetAddress.TryParse(Settings.EmailAddress, out InternetAddress from))
                return false;

            try
            {
                MimeMessage message = new MimeMessage();
                message.Subject = subject;
                message.From.Add(from);
                message.To.Add(to);
                message.Body = new TextPart(TextFormat.Html) { Text = msg };
                message.ReplyTo.Add(from);

                var gmailService = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = await GetCredentials(),
                    ApplicationName = "chop9ja"
                });

                string raw = Encode(message.ToString());
                var gmailMessage = new GMailMessage { Raw = raw };
                
                var request = gmailService.Users.Messages.Send(gmailMessage, Settings.EmailAddress);

                await request.ExecuteAsync();

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occured while attempting to send a mail.");
            }

            return false;
        }
        #endregion

        async Task<UserCredential> GetCredentials()
        {
            try
            {
                CancellationTokenSource cancel = new CancellationTokenSource(TimeSpan.FromMinutes(3));

                var token = new TokenResponse() { RefreshToken = Settings.RefreshToken };

                UserCredential credentials = new UserCredential(new GoogleAuthorizationCodeFlow(
                    new GoogleAuthorizationCodeFlow.Initializer()
                    {
                        ClientSecrets = new ClientSecrets()
                        {
                            ClientId = Settings.ClientID,
                            ClientSecret = Settings.ClientSecret
                        }
                    }), Settings.EmailAddress, token);

                await credentials.RefreshTokenAsync(cancel.Token);

                return credentials;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occured while aquiring google credentials.");
                return null;
            }
        }

        static string Encode(string text)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);

            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }


        #endregion
    }
}

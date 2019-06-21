using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Options;
using Chop9ja.API.Services.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Gmail.v1;
using Google.Apis.Util;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using GAuth = Google.Apis.Auth.OAuth2.GoogleWebAuthorizationBroker;

namespace Chop9ja.API.Services
{
    [AutoBuild]
    public class EmailService : IEmailService
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

                var cred = await GetCredentials();
                
                using (SmtpClient client = new SmtpClient())
                {
                    await client.ConnectAsync(Settings.Domain, Settings.Port, 
                        SecureSocketOptions.StartTlsWhenAvailable);

                    var oauth = new SaslMechanismOAuth2(cred.UserId, cred.Token.AccessToken);
                    await client.AuthenticateAsync(oauth);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
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

            return System.Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }

        #endregion

    }
}

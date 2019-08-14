using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Options;
using Chop9ja.API.Services.Interfaces;
using FluentEmail.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMarkupMin.Core;

namespace Chop9ja.API.Services.Mail
{
    [AutoBuild]
    public class MailGunEmailService : IEmailService
    {
        #region Properties
        public EmailSettings Settings => EmailOptions.Value;

        #region Internals
        [DeepDependency]
        IOptions<EmailSettings> EmailOptions { get; }
        [DeepDependency]
        IFluentEmail FluentEmail { get; }
        [DeepDependency]
        ILogger Logger { get; }
        #endregion

        #endregion

        #region Methods

        #region IEmailService Implementation
        public async Task<bool> SendEmailAsync(string destinationMail, string subject, string message)
        {
            var response = await FluentEmail.To(destinationMail)
                .Subject(subject).Body(message, true).SendAsync();
            return response.Successful;
        }

        public async Task<string> GetTemplateAsync(string path)
        {
            string template = string.Empty;
            try
            {
                template = await System.IO.File.ReadAllTextAsync(path);

                HtmlMinifier minifier = new HtmlMinifier();
                template = minifier.Minify(template).MinifiedContent;
            }
            catch (Exception ex)
            {
                Logger.LogError($"An error occured while retriving an email template.\n{ex}");
            }

            return template;
        }
        #endregion

        #endregion
    }
}

using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Options;
using Chop9ja.API.Services.Interfaces;
using FluentEmail.Core;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        #endregion

        #endregion
    }
}

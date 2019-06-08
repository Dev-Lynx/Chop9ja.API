using Chop9ja.API.Models.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Services.Interfaces
{
    public interface IEmailService
    {
        EmailSettings Settings { get; }
        Task<bool> SendEmailAsync(string destinationMail, string subject, string message);
    }
}

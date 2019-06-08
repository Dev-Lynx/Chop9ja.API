using Chop9ja.API.Models.Options;
using Chop9ja.API.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Services.Interfaces
{
    public interface ISmsService 
    {
        SMSOptions Options { get; }
        Task<bool> CanSend();
        Task<double> GetBalance();
        Task<bool> SendMessage(string phoneNumber, string message);
    }
}

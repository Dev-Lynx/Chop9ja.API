using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Services.Interfaces
{
    public interface IAuthService
    {
        AuthSettings AuthSettings { get; }
        Task<bool> ValidateOneTimePassword(User user, OnePasswordType kind, string code);
        Task<OneTimePassword> GenerateOneTimePassword(User user, OnePasswordType kind);
    }
}

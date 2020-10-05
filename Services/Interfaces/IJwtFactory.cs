using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Services.Interfaces
{
    public interface IJwtFactory
    {
        JwtIssuerOptions Options { get; }
        Task<string> GenerateToken(User user);
    }
}

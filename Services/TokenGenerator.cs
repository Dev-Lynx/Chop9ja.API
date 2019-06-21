using Chop9ja.API.Services.Interfaces;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Services
{
    public class TokenGenerator : OtpNet.Hotp, ITokenGenerator
    {
        public TokenGenerator(byte[] secretKey, OtpHashMode mode = OtpHashMode.Sha1, int hotpSize = 6) : base(secretKey, mode, hotpSize)
        {

        }
    }
}

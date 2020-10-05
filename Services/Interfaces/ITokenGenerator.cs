using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Services.Interfaces
{
    public interface ITokenGenerator
    {
        string ComputeHOTP(long counter);
        bool VerifyHotp(string hotp, long counter);
    }
}

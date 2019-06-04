using Chop9ja.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Services
{
    public class AuthService : IAuthService
    {
        public void SayHello()
        {
            Core.Log.Debug("Well Hello, there!");
        }
    }
}

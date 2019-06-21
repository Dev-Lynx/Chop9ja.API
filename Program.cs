using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Chop9ja.API.Data;
using Chop9ja.API.Models.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Unity.Microsoft.DependencyInjection;

namespace Chop9ja.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Core.StartupArguments = args;
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                }).UseNLog()
                .UseUnityServiceProvider()
                .UseStartup<Startup>();
    }
}

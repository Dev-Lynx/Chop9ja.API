using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AspNetCore.Identity.MongoDbCore.Models;
using Chop9ja.API.Data;
using Chop9ja.API.Models.Entities;
using FluentScheduler;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using NLog.Conditions;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity;

namespace Chop9ja.API
{
    public static class Core
    {
        #region Properties
        public static Logger Log { get; } = LogManager.GetCurrentClassLogger();
        public static string[] StartupArguments { get; set; } = new string[0];
        public static IUnityContainer Container { get; private set; }
        public static IServiceProvider ServiceProvider { get; private set; }
        public static MongoDataContext DataContext => MongoDataContext.Current;
        public static IMongoRepository DataStore => MongoDataContext.Current.Store;
        static IWebHost Host { get; set; }

        #region Solids

        #region Names
        public static string PRODUCT_NAME = "Chop9ja API";
        public static string PRODUCT_VERSION = "v0-1";
        public static string FULL_PRODUCT_NAME => $"{Core.PRODUCT_NAME} {Core.PRODUCT_VERSION}";
        public static string AUTHOR = "Prince Owen";
        public static string COMPANY = "Dev-Lynx Technologies";
        public const string CONSOLE_LOG_NAME = "console-debugger";
        public const string LOG_LAYOUT = "${longdate}|${uppercase:${level}}| ${message} ${exception:format=tostring";
        public const string FULL_LOG_LAYOUT = "${longdate} | ${logger}\n${message} ${exception:format=tostring}\n";
        public static readonly string ERROR_LOG_NAME = $"Errors_{DateTime.Now.ToString("MM-yyyy")}";
        public static readonly string RUNTIME_LOG_NAME = $"Runtime_{DateTime.Now.ToString("MM-yyyy")}";
        public const string REGION = "NG";
        public const string SQL_SERVER_CONNECTION_STRING = "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;AttachDbFilename=|DataDirectory|\\Identity.mdf;Initial Catalog=Chop9ja";
        #endregion

        #region Directories
        public static readonly string BASE_DIR = Directory.GetCurrentDirectory();
        public static readonly string WORK_DIR = Path.Combine(BASE_DIR, "App");
        public static readonly string DATA_DIR = Path.Combine(WORK_DIR, "Data");
        public static readonly string INDEX_DIR = Path.Combine(WORK_DIR, "Indexes");
        public readonly static string LOG_DIR = Path.Combine(WORK_DIR, "Logs");
        #endregion

        #region Paths
        public static string RUNTIME_LOG_PATH => Path.Combine(LOG_DIR, RUNTIME_LOG_NAME + ".log");
        public static string ERROR_LOG_PATH => Path.Combine(LOG_DIR, ERROR_LOG_NAME + ".log");
        #endregion

        #region Routes
        public const string NGROK_SERVER = "http://c9dae43d.ngrok.io";
        public static string ONLINE_BASE_ADDRESS { get; set; }
        public const string DOCS_ROUTE = "/api/docs";
        #endregion

        #region JWT Claim Identifiers
        public const string JWT_CLAIM_ID = "id";
        public const string JWT_CLAIM_ROL = "rol";
        public const string JWT_CLAIM_VERIFIED = "ver";
        #endregion

        #region JWT Claims
        public const string JWT_CLAIM_API_USER = "api_user";
        public const string JWT_CLAIM_API_ACCESS = "api_access";
        #endregion

        #endregion

        #endregion

        #region Constructor
        static Core()
        {
            CreateDirectories(WORK_DIR, DATA_DIR, LOG_DIR);

            JobManager.Initialize(new Registry());

            ConfigureLogger();

            Core.Log.Info("\n\n");
            Core.Log.Info($"*** Application Started ***\nWelcome to the {PRODUCT_NAME}\nBuilt by {AUTHOR}" +
                $"\nUnder the supervision of {COMPANY}\nCopyright 2019.\nAll rights reserved.\n\n");
        }
        #endregion

        #region Methods

        #region Helpers
        /// <summary>
        /// Easy and safe way to create multiple directories. 
        /// </summary>
        /// <param name="directories">The set of directories to create</param>
        public static void CreateDirectories(params string[] directories)
        {
            if (directories == null || directories.Length <= 0) return;

            foreach (var directory in directories)
                try
                {
                    if (Directory.Exists(directory)) continue;

                    Directory.CreateDirectory(directory);
                    Log.Info("A new directory has been created ({0})", directory);
                }
                catch (Exception e)
                {
                    Log.Error("Error while creating directory {0} - {1}", directory, e);
                }
        }

        public static void ClearDirectory(string directory, bool removeDirectory = false)
        {
            if (string.IsNullOrWhiteSpace(directory)) return;

            foreach (var d in Directory.EnumerateDirectories(directory))
                ClearDirectory(d, true);

            foreach (var file in Directory.EnumerateFiles(directory, "*"))
                try { File.Delete(file); }
                catch (Exception e) { Log.Error("Failed to delete {0}\n", file, e); }

            if (removeDirectory)
                try { Directory.Delete(directory); }
                catch (Exception ex) { Log.Error("An error occured while attempting to remove a directory ({0})\n{1}", directory, ex); }
        }
        #endregion

        static void ConfigureLogger()
        {
            var config = new LoggingConfiguration();

#if DEBUG
            var debugConsole = new ColoredConsoleTarget()
            {
                Name = Core.CONSOLE_LOG_NAME,
                Layout = Core.FULL_LOG_LAYOUT,
                Header = $"{PRODUCT_NAME} Debugger"
            };

            var debugRule = new LoggingRule("*", LogLevel.Debug, debugConsole);
            config.LoggingRules.Add(debugRule);
#endif

            var errorFileTarget = new FileTarget()
            {
                Name = Core.ERROR_LOG_NAME,
                FileName = Core.ERROR_LOG_PATH,
                Layout = Core.LOG_LAYOUT
            };

            config.AddTarget(errorFileTarget);

            var errorRule = new LoggingRule("*", LogLevel.Error, errorFileTarget);
            config.LoggingRules.Add(errorRule);

            var runtimeFileTarget = new FileTarget()
            {
                Name = Core.RUNTIME_LOG_NAME,
                FileName = Core.RUNTIME_LOG_PATH,
                Layout = Core.LOG_LAYOUT
            };
            config.AddTarget(runtimeFileTarget);

            var runtimeRule = new LoggingRule("*", LogLevel.Trace, runtimeFileTarget);
            config.LoggingRules.Add(runtimeRule);


            var stackDriverTarget = new Google.Cloud.Logging.NLog.GoogleStackdriverTarget()
            {
                Name = "Google StackDriver Logger",
                Layout = Core.LOG_LAYOUT,
                ProjectId = "chop9ja",
                CredentialFile = Path.Combine(BASE_DIR, "credentials.json")
            };

            config.AddTarget(stackDriverTarget);
            var stackDriverRule = new LoggingRule("*", LogLevel.Trace, stackDriverTarget);
            config.LoggingRules.Add(stackDriverRule);


            LogManager.Configuration = config;

            LogManager.ReconfigExistingLoggers();

            DateTime oneMonthLater = DateTime.Now.AddMonths(1);
            DateTime nextMonth = new DateTime(oneMonthLater.Year, oneMonthLater.Month, 1);

            JobManager.AddJob(() =>
            {
                Core.Log.Debug("*** Monthly Session Ended ***");
                ConfigureLogger();
            }, s => s.ToRunOnceAt(nextMonth));
        }

        public static void RegisterServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Core.Log.Debug("Successfully registered the service provider");
        }

        public static void ConfigureCoreServices(IUnityContainer container)
        {
            Container = container;
        }



        public static Task Initialize(IWebHost host)
        {
            Host = host;
            var env = Container.Resolve<IHostingEnvironment>();

            if (env.IsDevelopment()) ONLINE_BASE_ADDRESS = Core.NGROK_SERVER;
            else ONLINE_BASE_ADDRESS = Host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.FirstOrDefault();

            InitializeData().Wait();

            if (StartupArguments.Length > 0)
                if (StartupArguments[0] == "--seed")
                    SeedUsers().Wait();

            Core.Log.Debug($"{PRODUCT_NAME} has successfully been initialized.");
            return Task.CompletedTask;
        }

        static async Task InitializeData()
        {
            var roleManager = Container.Resolve<RoleManager<UserRole>>();
            var userManager = Container.Resolve<UserManager<User>>();
            var dataContext = Container.Resolve<MongoDataContext>();

            string[] roles = Enum.GetValues(typeof(UserRoles)).
                OfType<UserRoles>().Select(u => u.ToString()).ToArray();

            foreach (var role in roles)
            {
                if (await roleManager.RoleExistsAsync(role)) continue;
                await roleManager.CreateAsync(new UserRole(role));
            }

            await ConfigureBanks();
            await ConfigurePaymentChannels();

            if (await userManager.FindByEmailAsync("devlynx.official@gmail.com") == null)
            {
                User platformAccount = new User()
                {
                    Email = "devlynx.official@gmail.com",
                    UserName = "Chop9ja",
                    FirstName = "Chop9ja",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsPlatform = true,
                    BankAccounts = new List<BankAccount>()
                    {
                        new BankAccount()
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            AccountName = "Chop9ja INC.",
                            AccountNumber = "123456789",
                            BankId = 5
                        },

                        new BankAccount()
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            AccountName = "Chop9ja International",
                            AccountNumber = "987654321",
                            BankId = 2
                        }
                    }
                };

                await userManager.CreateAsync(platformAccount);
                await userManager.AddToRoleAsync(platformAccount, UserRoles.Platform.ToString());
                await userManager.AddPasswordAsync(platformAccount, "Password@Password321");
                await platformAccount.InitializeAsync();
            }


            
        }

        static async Task ConfigurePaymentChannels()
        {
            string json = await File.ReadAllTextAsync(Path.Combine(Core.DATA_DIR, "paymentChannels.json"));
            var channels = JsonConvert.DeserializeObject<List<PaymentChannel>>(json);

            foreach (var channel in channels)
                if (!await DataContext.Store.AnyAsync<PaymentChannel>(c => c.Type == channel.Type))
                    await DataContext.Store.AddOneAsync(channel);
        }

        static async Task ConfigureBanks()
        {
            string bankData = Path.Combine(DATA_DIR, "banks.json");
            string bankJson = await File.ReadAllTextAsync(bankData);
            var banks = JsonConvert.DeserializeObject<List<Bank>>(bankJson);

            foreach (var bank in banks)
            {
                bool exists = await DataContext.Store.AnyAsync<Bank, int>(b => b.Id == bank.Id);
                if (!exists) await DataContext.Store.AddOneAsync<Bank, int>(bank);
            }
        }

        public static Task SeedUsers()
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}

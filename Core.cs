using FluentScheduler;
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

namespace Chop9ja.API
{
    public static class Core
    {
        #region Properties
        public static Logger Log { get; } = LogManager.GetCurrentClassLogger();
        public static string[] StartupArguments { get; set; } = new string[0];
        #region Solids

        #region Names
        static string PRODUCT_NAME = "Chop9ja API";
        static string AUTHOR = "Prince Owen";
        static string COMPANY = "Dev-Lynx Technologies";
        public const string CONSOLE_LOG_NAME = "console-debugger";
        public const string LOG_LAYOUT = "${longdate}|${uppercase:${level}}| ${message} ${exception:format=tostring";
        public const string FULL_LOG_LAYOUT = "${longdate} | ${logger}\n${message} ${exception:format=tostring}\n";
        public static readonly string ERROR_LOG_NAME = $"Errors_{DateTime.Now.ToString("MM-yyyy")}";
        public static readonly string RUNTIME_LOG_NAME = $"Runtime_{DateTime.Now.ToString("MM-yyyy")}";
        #endregion

        #region Directories
        public static readonly string BASE_DIR = Directory.GetCurrentDirectory();
        public static readonly string WORK_DIR = Path.Combine(BASE_DIR, "ApplicationBase");
        public static readonly string DATA_DIR = Path.Combine(WORK_DIR, "Data");
        public static readonly string INDEX_DIR = Path.Combine(WORK_DIR, "Indexes");
        public readonly static string LOG_DIR = Path.Combine(WORK_DIR, "Logs");
        #endregion

        #region Paths
        public static string RUNTIME_LOG_PATH => Path.Combine(LOG_DIR, RUNTIME_LOG_NAME + ".log");
        public static string ERROR_LOG_PATH => Path.Combine(LOG_DIR, ERROR_LOG_NAME + ".log");
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

        #endregion
    }
}

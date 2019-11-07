using NLog;
using NLog.Config;
using NLog.Targets;

namespace Honeybee.Core
{
    public static class NLogUtils
    {
        /// <summary>
        /// Creates a new NLog configuration. By default NLog will be configured to
        /// write to a file in the same directory as the application. There is also
        /// an optional REST API argument that would allow writing to a DB.
        /// </summary>
        public static void CreateConfiguration()
        {
            var config = new LoggingConfiguration();

            // (Konrad) File Log Setup
            var fileTarget = new FileTarget
            {
                Name = "default",
                FileName = @"${specialfolder:folder=ApplicationData}/Honeybee/logs/Debug.log",
                Layout = @"${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=ToString}",
                KeepFileOpen = false,
                ArchiveFileName = @"${specialfolder:folder=ApplicationData}/Honeybee/logs/Debug_${shortdate}.{##}.log",
                ArchiveNumbering = ArchiveNumberingMode.Sequence,
                ArchiveEvery = FileArchivePeriod.Day,
                MaxArchiveFiles = 30
            };
            config.AddTarget("logfile", fileTarget);

            var rule1 = new LoggingRule("*", LogLevel.Trace, fileTarget);
            config.LoggingRules.Add(rule1);

            LogManager.Configuration = config;
        }
    }
}

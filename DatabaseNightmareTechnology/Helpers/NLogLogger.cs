using NLog;
using NLog.Config;
using Prism.Logging;

namespace DatabaseNightmareTechnology
{
    /// <summary>
    /// ログ設定
    /// </summary>
    public class NLogLogger : ILoggerFacade
    {
        private ILogger Logger { get; }

        public NLogLogger()
        {
            LogManager.Configuration = new XmlLoggingConfiguration("./nlog.config");
            Logger = LogManager.GetCurrentClassLogger();
        }

        private LogLevel CategoryToLogLevel(Category category)
        {
            switch (category)
            {
                case Category.Debug:
                    return LogLevel.Debug;
                case Category.Exception:
                    return LogLevel.Error;
                case Category.Info:
                    return LogLevel.Info;
                case Category.Warn:
                    return LogLevel.Warn;
                default:
                    return LogLevel.Off;
            }
        }
        public void Log(string message, Category category, Priority priority)
        {
            Logger.Log(CategoryToLogLevel(category), message);
        }
    }
}

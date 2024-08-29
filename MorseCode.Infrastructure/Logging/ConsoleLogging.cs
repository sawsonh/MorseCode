using MorseCode.Core.Infrastructure.Logging;

namespace MorseCode.Infrastructure.Logging
{
    public class ConsoleLogging : ILogging
    {
        private LogLevel LogLevel { get; set; } = LogLevel.Info;

        public void SetLogLevel(LogLevel logLevel)
        {
            this.LogLevel = logLevel;
        }

        public void LazyTrace(Func<string> messageFunc) => LazyLog(LogLevel.Trace, "TRACE", messageFunc);

        public void LazyDebug(Func<string> messageFunc) => LazyLog(LogLevel.Debug, "DEBUG", messageFunc);

        public void Info(string message) => Log(LogLevel.Info, "INFO", message);

        public void Warn(string message) => Log(LogLevel.Warn, "WARN", message);

        public void Error(string message) => Log(LogLevel.Error, "ERROR", message);

        public void Fatal(string message) => Log(LogLevel.Fatal, "FATAL", message);

        public void Error(string message, Exception exception)
            => Log(LogLevel.Error, "ERROR", $"{message} - Exception: {exception.Message}\nStackTrace: {exception.StackTrace}");

        public void Fatal(string message, Exception exception)
            => Log(LogLevel.Fatal, "FATAL", $"{message} - Exception: {exception.Message}\nStackTrace: {exception.StackTrace}");

        private void LazyLog(LogLevel messageLevel, string level, Func<string> messageFunc)
        {
            // Log only if the message level is greater than or equal to the current log level
            if (messageLevel >= LogLevel)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {messageFunc()}");
            }
        }

        private void Log(LogLevel messageLevel, string level, string message)
        {
            // Log only if the message level is greater than or equal to the current log level
            if (messageLevel >= LogLevel)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");
            }
        }
    }
}


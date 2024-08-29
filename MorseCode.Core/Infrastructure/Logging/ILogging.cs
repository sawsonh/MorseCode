namespace MorseCode.Core.Infrastructure.Logging
{
    public interface ILogging
    {
        /// <summary>
        /// Set log level
        /// </summary>
        /// <param name="logLevel">log level</param>
        void SetLogLevel(LogLevel logLevel);

        /// <summary>
        /// Logs a message at the Trace level.
        /// </summary>
        /// <param name="messageFunc">Lazy evaluation, if log level suffices, of message to log</param>
        void LazyTrace(Func<string> messageFunc);

        /// <summary>
        /// Logs a message at the Debug level.
        /// </summary>
        /// <param name="messageFunc">Lazy evaluation, if log level suffices, of message to log</param>
        void LazyDebug(Func<string> messageFunc);

        /// <summary>
        /// Logs a message at the Information level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Info(string message);

        /// <summary>
        /// Logs a message at the Warning level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Warn(string message);

        /// <summary>
        /// Logs a message at the Error level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Error(string message);

        /// <summary>
        /// Logs a message at the Fatal level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Fatal(string message);

        /// <summary>
        /// Logs a message with an associated exception at the Error level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        void Error(string message, Exception exception);

        /// <summary>
        /// Logs a message with an associated exception at the Fatal level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        void Fatal(string message, Exception exception);
    }

}


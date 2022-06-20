using System;
using System.Globalization;
using VTQT.Core.Domain;
using VTQT.Core.Logging;

namespace VTQT.Services.Logging
{
    // TODO-XBase-Log
    public partial class SeriLogger : ILogger
    {
        #region Constants



        #endregion

        #region Fields

        private readonly LogLevel _logLevel;
        public LogLevel LogLevel => _logLevel;

        #endregion

        #region Ctor

        public SeriLogger(
            )
        {
            //var logLevelValue = CommonHelper.GetAppSetting<string>("Logging:LogLevel:Default");
            //_logLevel = Enum.Parse<LogLevel>(logLevelValue);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if this logger is enabled for a given <see cref="LogLevel"/> passed as parameter. 
        /// </summary>
        /// <param name="level">true if this logger is enabled for level, otherwise false</param>
        /// <returns>Result</returns>
        public virtual bool IsEnabledFor(LogLevel level)
        {
            return level >= LogLevel;
        }

        /// <summary>
        /// Generates a logging event for the specified level using the message and exception
        /// </summary>
        /// <param name="level">The level of the message to be logged</param>
        /// <param name="exception">The exception to log, including its stack trace. Pass null to not log an exception</param>
        /// <param name="message">The message object to log</param>
        /// <param name="args">An Object array containing zero or more objects to format. Can be null.</param>
        public virtual void Log(LogLevel level, Exception exception, string message, object[] args)
        {
            if (exception == null && string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException($"{nameof(exception)}, {nameof(message)}");

            message ??= exception.Message;

            if (args != null && args.Length > 0)
            {
                message = string.Format(CultureInfo.InvariantCulture, message, args);
            }

            
        }

        #endregion
    }
}

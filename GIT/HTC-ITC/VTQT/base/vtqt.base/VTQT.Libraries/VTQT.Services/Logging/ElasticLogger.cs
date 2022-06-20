using Nest;
using System;
using System.Globalization;
using VTQT.Core;
using VTQT.Core.Infrastructure;
using VTQT.Core.Logging;
using VTQT.Elastic.Documents.Common;
using VTQT.Elastic.Helpers;
using LogLevel = VTQT.Core.Domain.LogLevel;

namespace VTQT.Services.Logging
{
    public partial class ElasticLogger : ILogger
    {
        #region Constants



        #endregion

        #region Fields

        private readonly LogLevel _logLevel;
        public LogLevel LogLevel => _logLevel;

        private readonly ElasticClient _elasticClient;

        #endregion

        #region Ctor

        public ElasticLogger(
            //ElasticClient elasticClient,
            Func<string, ElasticClient> elasticClient,
            IWorkContext workContext)
        {
            var logLevelValue = CommonHelper.GetAppSetting<string>("Logging:LogLevel:Default");
            _logLevel = Enum.Parse<LogLevel>(logLevelValue);

            //_elasticClient = elasticClient;
            _elasticClient = elasticClient(ElasticHelper.ConnectionNames.Default);
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

            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var logDoc = new LogDoc
            {
                AppType = CommonHelper.XBaseConfig.AppType,
                LogLevel = level,
                LogLevelText = level.ToString(),
                ShortMessage = message,
                FullMessage = exception?.ToString(),
                //Action = 
                //Data = 
                UserId = workContext.UserId,
                UserName = workContext.UserName,
                //PageUrl = 
                //ReferrerUrl = 
                //HttpMethod = 
                //HttpStatusCode = 
                MachineName = Environment.MachineName,
                //IpAddress = 
            };

            var res = _elasticClient.IndexDocument(logDoc);
        }

        #endregion
    }
}

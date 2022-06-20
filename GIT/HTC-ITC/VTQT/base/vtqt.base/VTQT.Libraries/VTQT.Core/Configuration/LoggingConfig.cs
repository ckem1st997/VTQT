namespace VTQT.Core.Configuration
{
    public partial class LoggingConfig
    {
        public const string Logging = "Logging";

        public string LoggerType { get; set; }

        public LoggingConfig()
        {
            LoggerType = LoggingHelper.LoggerTypes.SeriLogger;
        }
    }

    public class LoggingHelper
    {
        public static class LoggerTypes
        {
            /// <summary>
            /// Serilog
            /// </summary>
            public static readonly string SeriLogger = "SeriLogger";
            /// <summary>
            /// Elastic
            /// </summary>
            public static readonly string ElasticLogger = "ElasticLogger";
        }
    }
}

using System;
using VTQT.Core.Domain;

namespace VTQT.Elastic.Documents.Common
{
    [Serializable]
    public class LogDoc : IElasticDoc
    {
        public string Id { get; set; }

        public string AppType { get; set; }

        public LogLevel LogLevel { get; set; }

        public string LogLevelText { get; set; }

        public string ShortMessage { get; set; }

        public string FullMessage { get; set; }

        public string Action { get; set; }

        public object Data { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string PageUrl { get; set; }

        public string ReferrerUrl { get; set; }

        public string HttpMethod { get; set; }

        public int HttpStatusCode { get; set; }

        public string MachineName { get; set; }

        public string IpAddress { get; set; }

        public DateTime Timestamp { get; set; }

        public LogDoc()
        {
            Id = Guid.NewGuid().ToString();
            Timestamp = DateTime.UtcNow;
        }
    }
}

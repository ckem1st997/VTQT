using System.Collections.Generic;

namespace VTQT.Core.Configuration
{
    public partial class ElasticConfig
    {
        public const string Elastic = "Elastic";

        public IList<ElasticConnection> Connections { get; set; }

        public string EnvironmentName { get; set; }

        public bool UseElasticApm { get; set; }

        public ElasticConfig()
        {
            Connections = new List<ElasticConnection>();
            UseElasticApm = true;
        }
    }

    public class ElasticConnection
    {
        public string Name { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public IList<string> Uris { get; set; }

        public ElasticConnection()
        {
            Uris = new List<string>();
        }
    }
}

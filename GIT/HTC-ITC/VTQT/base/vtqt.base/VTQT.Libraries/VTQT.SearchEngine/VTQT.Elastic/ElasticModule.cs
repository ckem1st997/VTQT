using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Elasticsearch.Net;
using Nest;
using VTQT.Core;
using VTQT.Elastic.Documents.Common;
using VTQT.Elastic.Helpers;
using VTQT.Elastic.Mapping;

namespace VTQT.Elastic
{
    public class ElasticModule : Module
    {
        public IEnumerable<string> ConnectionNames { get; }

        public ElasticModule(
            IEnumerable<string> connectionNames = null)
        {
            ConnectionNames = connectionNames;
            if (ConnectionNames == null || !ConnectionNames.Any())
                ConnectionNames = new[] { ElasticHelper.ConnectionNames.Default };
        }

        protected override void Load(ContainerBuilder builder)
        {
            var config = CommonHelper.ElasticConfig;
            if (config.Connections == null || !config.Connections.Any())
                throw new ArgumentNullException(nameof(config.Connections));

            var conns = config.Connections.Where(w => ConnectionNames.Contains(w.Name));
            if (conns == null || !conns.Any())
                throw new ArgumentNullException(nameof(conns));
            foreach (var conn in conns)
            {
                if (conn.Uris == null || !conn.Uris.Any())
                    throw new ArgumentNullException($"Connection [{nameof(conn.Name)}]: {nameof(conn.Uris)}");

                IConnectionPool connPool;
                if (conn.Uris.Count == 1)
                {
                    var uri = conn.Uris[0];
                    connPool = new SingleNodeConnectionPool(new Uri(uri));
                }
                else
                {
                    var uris = conn.Uris.Select(s => new Uri(s));
                    connPool = new SniffingConnectionPool(uris);
                }

                var settings = new ConnectionSettings(connPool)
                    .DefaultFieldNameInferrer(x => x);
                if (!string.IsNullOrWhiteSpace(conn.UserName) && !string.IsNullOrWhiteSpace(conn.Password))
                    settings.BasicAuthentication(conn.UserName, conn.Password);

                if (conn.Name == ElasticHelper.ConnectionNames.Default)
                {
                    AddCommonMappings(settings);

                    var client = new ElasticClient(settings);

                    client.MapCommon();

                    //builder.Register(x => client)
                    //    .Named<ElasticClient>(ElasticHelper.ConnectionNames.Default)
                    //    .SingleInstance();
                    builder.RegisterInstance(client);
                    builder.RegisterInstance(client)
                        .Named<ElasticClient>(ElasticHelper.ConnectionNames.Default);

                    // Register resolving delegate
                    builder.Register<Func<string, ElasticClient>>(c =>
                    {
                        var cc = c.Resolve<IComponentContext>();
                        return connectionName => cc.ResolveNamed<ElasticClient>(connectionName);
                    });

                    builder.Register<Func<string, Lazy<ElasticClient>>>(c =>
                    {
                        var cc = c.Resolve<IComponentContext>();
                        return connectionName => cc.ResolveNamed<Lazy<ElasticClient>>(connectionName);
                    });
                }
            }
        }

        #region DefaultMappings

        private static void AddCommonMappings(ConnectionSettings settings)
        {
            settings
                .DefaultMappingFor<LogDoc>(m => m
                    .IndexName(ElasticIndexHelper.GetIndexName<LogDoc>())
                    .IdProperty(x => x.Id)
                );
        }

        #endregion
    }
}

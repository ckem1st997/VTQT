using Autofac;
using Autofac.Core;
using LinqToDB.Data;
using VTQT.Core.Configuration;
using VTQT.Core.Domain.Dashboard;
using VTQT.Core.Infrastructure;
using VTQT.Core.Infrastructure.DependencyManagement;
using VTQT.Data;
using VTQT.Web.Framework.Infrastructure.Extensions;
using VTQT.Web.Framework.Routing;

namespace VTQT.Web.Dashboard.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, XBaseConfig config)
        {
            // Data Layer
            builder.RegisterType<DashboardDataConnection>()
                .Named<DataConnection>(DataConnectionHelper.ConnectionStringNames.Dashboard)
                .InstancePerDependency();
            builder.RegisterDataConnection(new[]
            {
                DataConnectionHelper.ConnectionStringNames.Master
            });

            // Repositories
            builder.RegisterGeneric(typeof(EntityRepository<>))
                .Named(DataConnectionHelper.ConnectionStringNames.Dashboard, typeof(IRepository<>))
                .WithParameter(new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(DataConnection) && pi.Name == DataConnectionHelper.ParameterName,
                    (pi, ctx) => EngineContext.Current.Resolve<DataConnection>(DataConnectionHelper.ConnectionStringNames.Dashboard)))
                .InstancePerLifetimeScope();
            builder.RegisterRepository(new[]
            {
                DataConnectionHelper.ConnectionStringNames.Master
            });

            // Cache
            builder.RegisterCacheManager();

            // Services


            //slug route transformer
            builder.RegisterType<SlugRouteTransformer>().InstancePerLifetimeScope();
        }

        /// <summary>
        /// Gets order of this dependency registrar implementation
        /// </summary>
        public int Order => 2;
    }
}

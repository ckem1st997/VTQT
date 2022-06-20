using Autofac;
using VTQT.Core.Configuration;
using VTQT.Core.Infrastructure;
using VTQT.Core.Infrastructure.DependencyManagement;
using VTQT.Data;
using VTQT.Services.File;
using VTQT.Services.Master;
using VTQT.SharedMvc.Helpers;
using VTQT.Web.Framework.Infrastructure.Extensions;


namespace VTQT.Api.File.Infrastructure
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
            builder.RegisterDataConnection(new[]
            {
                DataConnectionHelper.ConnectionStringNames.Master,
                DataConnectionHelper.ConnectionStringNames.File
            });

            // Repositories
            builder.RegisterRepository(new[]
            {
                DataConnectionHelper.ConnectionStringNames.Master,
                DataConnectionHelper.ConnectionStringNames.File
            });

            // Cache
            builder.RegisterCacheManager();

            // Services
            builder.RegisterType<FilesService>().As<IFilesService>().InstancePerLifetimeScope();
        }

        /// <summary>
        /// Gets order of this dependency registrar implementation
        /// </summary>
        public int Order => 2;
    }
}

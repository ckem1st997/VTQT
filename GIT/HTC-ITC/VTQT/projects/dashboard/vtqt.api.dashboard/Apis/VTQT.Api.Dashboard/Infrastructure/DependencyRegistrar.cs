using Autofac;
using Autofac.Core;
using LinqToDB.Data;
using VTQT.Api.Dashboard.Helper;
using VTQT.Core.Configuration;
using VTQT.Core.Domain.Dashboard;
using VTQT.Core.Infrastructure;
using VTQT.Core.Infrastructure.DependencyManagement;
using VTQT.Data;
using VTQT.Services.Dashboard;
using VTQT.Services.Master;
using VTQT.SharedMvc.Helpers;
using VTQT.Web.Framework.Infrastructure.Extensions;


namespace VTQT.Api.Dashboard.Infrastructure
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
                DataConnectionHelper.ConnectionStringNames.Master,
                DataConnectionHelper.ConnectionStringNames.Dashboard,
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
                DataConnectionHelper.ConnectionStringNames.Master,
                DataConnectionHelper.ConnectionStringNames.Dashboard
            });

            // Cache
            builder.RegisterCacheManager();

            // Services
            

            builder.RegisterType<OrganizationService>().As<IOrganizationService>().InstancePerLifetimeScope();

            builder.RegisterType<StationService>().As<IStationService>().InstancePerLifetimeScope();

            builder.RegisterType<ProjectService>().As<IProjectService>().InstancePerLifetimeScope();

            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerLifetimeScope();

            builder.RegisterType<UserModelHelper>().As<IUserModelHelper>().InstancePerLifetimeScope();
            
            builder.RegisterType<StorageValueService>().As<IStorageValueService>().InstancePerLifetimeScope();
            
            builder.RegisterType<TypeValueService>().As<ITypeValueService>().InstancePerLifetimeScope();
            
            builder.RegisterType<TypeValueModelHelper>().As<ITypeValueModelHelper>().InstancePerLifetimeScope();
            
            builder.RegisterType<MasterVTCNTTService>().As<IMasterVTCNTTService>().InstancePerLifetimeScope();
            
            builder.RegisterType<ExcampleService>().As<IExcampleService>().InstancePerLifetimeScope();
            builder.RegisterType<FTTHService>().As<IFTTHService>().InstancePerLifetimeScope();
            builder.RegisterType<FTTHService2022>().As<IFTTHService2022>().InstancePerLifetimeScope();
            builder.RegisterType<AuthorizeToRoleService>().As<IAuthorizeToRoleService>().InstancePerLifetimeScope();
            builder.RegisterType<DynamicService>().As<IDynamicService>().InstancePerLifetimeScope();
            builder.RegisterType<NameTableService>().As<INameTableService>().InstancePerLifetimeScope();
            builder.RegisterType<SelectTableService>().As<ISelectTableService>().InstancePerLifetimeScope();
            builder.RegisterType<MasterFileKH2022Service>().As<IMasterFileKH2022Service>().InstancePerLifetimeScope();
            builder.RegisterType<ExtensionGetValue>().As<IExtensionGetValue>().InstancePerLifetimeScope();
            builder.RegisterType<DashboardUserService>().As<IDashboardUserService>().InstancePerLifetimeScope();
            builder.RegisterType<FTTHMB3132022Service>().As<IFTTHMB3132022Service>().InstancePerLifetimeScope();

        }

        /// <summary>
        /// Gets order of this dependency registrar implementation
        /// </summary>
        public int Order => 2;
    }
}

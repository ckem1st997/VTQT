using Autofac;
using VTQT.Core.Configuration;
using VTQT.Core.Infrastructure;
using VTQT.Core.Infrastructure.DependencyManagement;
using VTQT.Data;
using VTQT.Services.Master;
using VTQT.Services.Master.UserRole;
using VTQT.Services.Security;
using VTQT.SharedMvc.Helpers;
using VTQT.Web.Framework.Infrastructure.Extensions;

namespace VTQT.Api.Master.Infrastructure
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
                DataConnectionHelper.ConnectionStringNames.Qlsc,
                DataConnectionHelper.ConnectionStringNames.FbmOrganization,
                DataConnectionHelper.ConnectionStringNames.FbmCrm,
                DataConnectionHelper.ConnectionStringNames.FbmContract,
                DataConnectionHelper.ConnectionStringNames.Warehouse,
                DataConnectionHelper.ConnectionStringNames.Asset

            });

            // Repositories
            builder.RegisterRepository(new[]
            {
                DataConnectionHelper.ConnectionStringNames.Master,
                DataConnectionHelper.ConnectionStringNames.Qlsc,
                DataConnectionHelper.ConnectionStringNames.FbmOrganization,
                DataConnectionHelper.ConnectionStringNames.FbmCrm,
                DataConnectionHelper.ConnectionStringNames.FbmContract,
                DataConnectionHelper.ConnectionStringNames.Warehouse,
                DataConnectionHelper.ConnectionStringNames.Asset
            });

            // Cache
            builder.RegisterCacheManager();

            // Services
            
            builder.RegisterType<AppModelHelper>().As<IAppModelHelper>().InstancePerLifetimeScope();

            builder.RegisterType<AppActionModelHelper>().As<IAppActionModelHelper>().InstancePerLifetimeScope();
            
            builder.RegisterType<AutoCodeService>().As<IAutoCodeService>().InstancePerLifetimeScope();

            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerLifetimeScope();

            builder.RegisterType<OrganizationService>().As<IOrganizationService>().InstancePerLifetimeScope();

            builder.RegisterType<ProjectService>().As<IProjectService>().InstancePerLifetimeScope();

            builder.RegisterType<ReportService>().As<IReportService>().InstancePerLifetimeScope();

            builder.RegisterType<StationService>().As<IStationService>().InstancePerLifetimeScope();
            
            builder.RegisterType<UserModelHelper>().As<IUserModelHelper>().InstancePerLifetimeScope();

            builder.RegisterType<VendorBillingService>().As<IVendorBillingService>().InstancePerLifetimeScope();
            
            builder.RegisterType<UserRoleService>().As<IUserRoleService>().InstancePerLifetimeScope();
        }

        /// <summary>
        /// Gets order of this dependency registrar implementation
        /// </summary>
        public int Order => 2;
    }
}

using Autofac;
using VTQT.Core.Configuration;
using VTQT.Core.Infrastructure;
using VTQT.Core.Infrastructure.DependencyManagement;
using VTQT.Data;
using VTQT.Services.Asset;
using VTQT.Services.Warehouse;
using VTQT.SharedMvc.Helpers;
using VTQT.Web.Framework.Infrastructure.Extensions;

namespace VTQT.Api.Asset.Infrastructure
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
                DataConnectionHelper.ConnectionStringNames.Asset,
                DataConnectionHelper.ConnectionStringNames.Qlsc,
                DataConnectionHelper.ConnectionStringNames.Warehouse,
                DataConnectionHelper.ConnectionStringNames.FbmOrganization,
                DataConnectionHelper.ConnectionStringNames.FbmContract
            });

            // Repositories
            builder.RegisterRepository(new[]
            {
                DataConnectionHelper.ConnectionStringNames.Master,
                DataConnectionHelper.ConnectionStringNames.Asset,
                DataConnectionHelper.ConnectionStringNames.Qlsc,
                DataConnectionHelper.ConnectionStringNames.Warehouse,
                DataConnectionHelper.ConnectionStringNames.FbmOrganization,
                DataConnectionHelper.ConnectionStringNames.FbmContract
            });

            // Cache
            builder.RegisterCacheManager();

            // Services
            builder.RegisterType<AssetService>().As<IAssetService>().InstancePerLifetimeScope();

            builder.RegisterType<AssetCategoryService>().As<IAssetCategoryService>().InstancePerLifetimeScope();

            builder.RegisterType<AssetDecreasedService>().As<IAssetDecreasedService>().InstancePerLifetimeScope();

            builder.RegisterType<AssetDetailService>().As<IAssetDetailService>().InstancePerLifetimeScope();
            
            builder.RegisterType<AssetTransferenceService>().As<IAssetTransferenceService>().InstancePerLifetimeScope();

            builder.RegisterType<AssetAttachmentService>().As<IAssetAttachmentService>().InstancePerLifetimeScope();

            builder.RegisterType<DecreaseReasonService>().As<IDecreaseReasonService>().InstancePerLifetimeScope();

            builder.RegisterType<HistoryService>().As<IHistoryService>().InstancePerLifetimeScope();

            builder.RegisterType<MaintenanceService>().As<IMaintenanceService>().InstancePerLifetimeScope();

            builder.RegisterType<MaintenanceDetailService>().As<IMaintenanceDetailService>().InstancePerLifetimeScope();

            builder.RegisterType<UserModelHelper>().As<IUserModelHelper>().InstancePerLifetimeScope();

            builder.RegisterType<UsageStatusService>().As<IUsageStatusService>().InstancePerLifetimeScope();

            builder.RegisterType<StationService>().As<IStationService>().InstancePerLifetimeScope();

            builder.RegisterType<WareHouseService>().As<IWareHouseService>().InstancePerLifetimeScope();

            builder.RegisterType<ChartService>().As<IChartService>().InstancePerLifetimeScope();

            builder.RegisterType<Services.Asset.AuditService>().As<Services.Asset.IAuditService>().InstancePerLifetimeScope();

            builder.RegisterType<Services.Asset.AuditDetailService>().As<Services.Asset.IAuditDetailService>().InstancePerLifetimeScope();

            builder.RegisterType<Services.Asset.AuditCouncilService>().As<Services.Asset.IAuditCouncilService>().InstancePerLifetimeScope();

            builder.RegisterType<Services.Asset.Print>().As<Services.Asset.IPrint>().InstancePerLifetimeScope();
        }

        /// <summary>
        /// Gets order of this dependency registrar implementation
        /// </summary>
        public int Order => 2;
    }
}

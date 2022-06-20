using Autofac;
using VTQT.Api.Warehouse.Helper;
using VTQT.Core.Configuration;
using VTQT.Core.Infrastructure;
using VTQT.Core.Infrastructure.DependencyManagement;
using VTQT.Data;
using VTQT.Services.Master;
using VTQT.Services.Warehouse;
using VTQT.SharedMvc.Helpers;
using VTQT.Web.Framework.Infrastructure.Extensions;
using IReportService = VTQT.Services.Warehouse.IReportService;
using ReportService = VTQT.Services.Warehouse.ReportService;


namespace VTQT.Api.Warehouse.Infrastructure
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
                DataConnectionHelper.ConnectionStringNames.Warehouse,
                DataConnectionHelper.ConnectionStringNames.Qlsc,
                DataConnectionHelper.ConnectionStringNames.FbmContract,
                DataConnectionHelper.ConnectionStringNames.FbmCrm,
                DataConnectionHelper.ConnectionStringNames.FbmOrganization
            });

            // Repositories
            builder.RegisterRepository(new[]
            {
                DataConnectionHelper.ConnectionStringNames.Master,
                DataConnectionHelper.ConnectionStringNames.Warehouse,
                DataConnectionHelper.ConnectionStringNames.Qlsc,
                DataConnectionHelper.ConnectionStringNames.FbmContract,
                DataConnectionHelper.ConnectionStringNames.FbmCrm,
                DataConnectionHelper.ConnectionStringNames.FbmOrganization
            });

            // Cache
            builder.RegisterCacheManager();

            // Services
            builder.RegisterType<AuditCouncilService>().As<IAuditCouncilService>().InstancePerLifetimeScope();

            builder.RegisterType<AuditDetailSerialService>().As<IAuditDetailSerialService>().InstancePerLifetimeScope();

            builder.RegisterType<AuditDetailService>().As<IAuditDetailService>().InstancePerLifetimeScope();

            builder.RegisterType<AuditService>().As<IAuditService>().InstancePerLifetimeScope();

            builder.RegisterType<AutoCodeService>().As<IAutoCodeService>().InstancePerLifetimeScope();

            builder.RegisterType<BeginningWareHouservice>().As<IBeginningWareHouseService>().InstancePerLifetimeScope();

            builder.RegisterType<InwardDetailService>().As<IInwardDetailService>().InstancePerLifetimeScope();

            builder.RegisterType<InwardService>().As<IInwardService>().InstancePerLifetimeScope();

            builder.RegisterType<OutwardDetailService>().As<IOutwardDetailService>().InstancePerLifetimeScope();

            builder.RegisterType<OutWardService>().As<IOutWardService>().InstancePerLifetimeScope();

            builder.RegisterType<SerialWareHouseService>().As<ISerialWareHouseService>().InstancePerLifetimeScope();

            builder.RegisterType<UnitService>().As<IUnitService>().InstancePerLifetimeScope();

            builder.RegisterType<VendorService>().As<IVendorService>().InstancePerLifetimeScope();

            builder.RegisterType<VoucherWareHouseService>().As<IVoucherWareHouseService>().InstancePerLifetimeScope();

            builder.RegisterType<WareHouseService>().As<IWareHouseService>().InstancePerLifetimeScope();

            builder.RegisterType<WarehouseBalanceService>().As<IWarehouseBalanceService>().InstancePerLifetimeScope();

            builder.RegisterType<WareHouseItemCategoryService>().As<IWareHouseItemCategoryService>().InstancePerLifetimeScope();

            builder.RegisterType<WareHouseItemService>().As<IWareHouseItemService>().InstancePerLifetimeScope();

            builder.RegisterType<WareHouseModelHelper>().As<IWareHouseModelHelper>().InstancePerLifetimeScope();

            builder.RegisterType<WareHouseLimitService>().As<IWareHouseLimitService>().InstancePerLifetimeScope();

            builder.RegisterType<OrganizationService>().As<IOrganizationService>().InstancePerLifetimeScope();

            builder.RegisterType<StationService>().As<IStationService>().InstancePerLifetimeScope();

            builder.RegisterType<ProjectService>().As<IProjectService>().InstancePerLifetimeScope();

            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerLifetimeScope();

            builder.RegisterType<ReportService>().As<IReportService>().InstancePerLifetimeScope();

            builder.RegisterType<Print>().As<IPrint>().InstancePerLifetimeScope();

            builder.RegisterType<UserModelHelper>().As<IUserModelHelper>().InstancePerLifetimeScope();

            builder.RegisterType<WareHouseItemUnitService>().As<IWareHouseItemUnitService>().InstancePerLifetimeScope();

            builder.RegisterType<WareHouseUserService>().As<IWareHouseUserService>().InstancePerLifetimeScope();

            builder.RegisterType<AdminRoleWareHouseService>().As<IAdminRoleWareHouseService>().InstancePerLifetimeScope();

            builder.RegisterType<CategoryService>().As<ICategoryService>().InstancePerLifetimeScope();

            builder.RegisterType<AccObjectService>().As<IAccObjectService>().InstancePerLifetimeScope();
        }

        /// <summary>
        /// Gets order of this dependency registrar implementation
        /// </summary>
        public int Order => 2;
    }
}

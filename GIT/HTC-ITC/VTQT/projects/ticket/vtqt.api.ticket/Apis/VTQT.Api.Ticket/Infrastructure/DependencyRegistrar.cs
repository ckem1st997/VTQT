using Autofac;
using VTQT.Core.Configuration;
using VTQT.Core.Infrastructure;
using VTQT.Core.Infrastructure.DependencyManagement;
using VTQT.Data;
using VTQT.Services.Ticket;
using VTQT.Services.Warehouse;
using VTQT.SharedMvc.Helpers;
using VTQT.Web.Framework.Infrastructure.Extensions;


namespace VTQT.Api.Ticket.Infrastructure
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
                DataConnectionHelper.ConnectionStringNames.Ticket,
                DataConnectionHelper.ConnectionStringNames.Qlsc,
                DataConnectionHelper.ConnectionStringNames.FbmContract,
                DataConnectionHelper.ConnectionStringNames.FbmCrm,
                DataConnectionHelper.ConnectionStringNames.FbmOrganization
            });

            // Repositories
            builder.RegisterRepository(new[]
            {
                DataConnectionHelper.ConnectionStringNames.Master,
                DataConnectionHelper.ConnectionStringNames.Ticket,
                DataConnectionHelper.ConnectionStringNames.Qlsc,
                DataConnectionHelper.ConnectionStringNames.FbmContract,
                DataConnectionHelper.ConnectionStringNames.FbmCrm,
                DataConnectionHelper.ConnectionStringNames.FbmOrganization
            });

            // Cache
            builder.RegisterCacheManager();

            // Services
            builder.RegisterType<ApprovalTicketService>().As<IApprovalTicketService>().InstancePerLifetimeScope();

            builder.RegisterType<ApprovalProgressService>().As<IApprovalProgressService>().InstancePerLifetimeScope();

            builder.RegisterType<AreaService>().As<IAreaService>().InstancePerLifetimeScope();

            builder.RegisterType<AssignmentService>().As<IAssignmentService>().InstancePerLifetimeScope();

            builder.RegisterType<Services.Master.AutoCodeService>().As<Services.Master.IAutoCodeService>().InstancePerLifetimeScope();

            builder.RegisterType<CableService>().As<ICableService>().InstancePerLifetimeScope();

            builder.RegisterType<ChannelService>().As<IChannelService>().InstancePerLifetimeScope();

            builder.RegisterType<ChannelAreaService>().As<IChannelAreaService>().InstancePerLifetimeScope();

            builder.RegisterType<ChannelCategoryService>().As<IChannelCategoryService>().InstancePerLifetimeScope();

            builder.RegisterType<ChannelStatusService>().As<IChannelStatusService>().InstancePerLifetimeScope();

            builder.RegisterType<ChannelTicketService>().As<IChannelTicketService>().InstancePerLifetimeScope();

            builder.RegisterType<CommentService>().As<ICommentService>().InstancePerLifetimeScope();

            builder.RegisterType<CustomerClassService>().As<ICustomerClassService>().InstancePerLifetimeScope();

            builder.RegisterType<DeviceService>().As<IDeviceService>().InstancePerLifetimeScope();

            builder.RegisterType<DeviceTicketService>().As<IDeviceTicketService>().InstancePerLifetimeScope();

            builder.RegisterType<FileService>().As<IFileService>().InstancePerLifetimeScope();

            builder.RegisterType<InfrastructorFeeService>().As<IInfrastructorFeeService>().InstancePerLifetimeScope();

            builder.RegisterType<LinkService>().As<ILinkService>().InstancePerLifetimeScope();

            builder.RegisterType<NetworkLinkService>().As<INetworkLinkService>().InstancePerLifetimeScope();

            builder.RegisterType<NetworkLinkTicketService>().As<INetworkLinkTicketService>().InstancePerLifetimeScope();

            builder.RegisterType<OrganizationUnitService>().As<IOrganizationUnitService>().InstancePerLifetimeScope();

            builder.RegisterType<PhenomenaService>().As<IPhenomenaService>().InstancePerLifetimeScope();

            builder.RegisterType<PriorityService>().As<IPriorityService>().InstancePerLifetimeScope();

            builder.RegisterType<ProblemTicketService>().As<IProblemTicketService>().InstancePerLifetimeScope();

            builder.RegisterType<ProjectService>().As<IProjectService>().InstancePerLifetimeScope();

            builder.RegisterType<RatingTicketService>().As<IRatingTicketService>().InstancePerLifetimeScope();

            builder.RegisterType<StationService>().As<IStationService>().InstancePerLifetimeScope();            

            builder.RegisterType<StationService>().As<IStationService>().InstancePerLifetimeScope();

            builder.RegisterType<StationCategoryService>().As<IStationCategoryService>().InstancePerLifetimeScope();

            builder.RegisterType<StationLevelService>().As<IStationLevelService>().InstancePerLifetimeScope();

            builder.RegisterType<StationTicketService>().As<IStationTicketService>().InstancePerLifetimeScope();

            builder.RegisterType<StatusService>().As<IStatusSerive>().InstancePerLifetimeScope();

            builder.RegisterType<StatusCategoryService>().As<IStatusCategoryService>().InstancePerLifetimeScope();

            builder.RegisterType<TicketCategoryService>().As<ITicketCategoryService>().InstancePerLifetimeScope();

            builder.RegisterType<TicketService>().As<ITicketService>().InstancePerLifetimeScope();

            builder.RegisterType<TicketAreaService>().As<ITicketAreaService>().InstancePerLifetimeScope();

            builder.RegisterType<TicketProvinceService>().As<ITicketProvinceService>().InstancePerLifetimeScope();

            builder.RegisterType<TicketProgressService>().As<ITicketProgressService>().InstancePerLifetimeScope();

            builder.RegisterType<TicketReasonService>().As<ITicketReasonService>().InstancePerLifetimeScope();

            builder.RegisterType<TroubleTicketService>().As<ITroubleTicketService>().InstancePerLifetimeScope();

            builder.RegisterType<UserModelHelper>().As<IUserModelHelper>().InstancePerLifetimeScope();

            builder.RegisterType<NetworkLinkCategoryService>().As<INetworkLinkCategoryService>().InstancePerLifetimeScope();

            builder.RegisterType<DeviceCategoryService>().As<IDeviceCategoryService>().InstancePerLifetimeScope();

            builder.RegisterType<WareHouseItemService>().As<IWareHouseItemService>().InstancePerLifetimeScope();

            builder.RegisterType<CRService>().As<ICRService>().InstancePerLifetimeScope();

            builder.RegisterType<CRHTCService>().As<ICRHTCService>().InstancePerLifetimeScope();

            builder.RegisterType<CRCategoryService>().As<ICRCategoryService>().InstancePerLifetimeScope();

            builder.RegisterType<ConfirmCRService>().As<IConfirmCRService>().InstancePerLifetimeScope();

            builder.RegisterType<ApprovalCRService>().As<IApprovalCRService>().InstancePerLifetimeScope();

            builder.RegisterType<InfrastructorFeeCRService>().As<IInfrastructorFeeCRService>().InstancePerLifetimeScope();

            builder.RegisterType<CRMxService>().As<ICRMxService>().InstancePerLifetimeScope();

            builder.RegisterType<ConfirmCRMxService>().As<IConfirmCRMxService>().InstancePerLifetimeScope();

            builder.RegisterType<ApprovalCRMxService>().As<IApprovalCRMxService>().InstancePerLifetimeScope();

            builder.RegisterType<CRPartnerService>().As<ICRPartnerService>().InstancePerLifetimeScope();

            builder.RegisterType<Services.Ticket.Print>().As<Services.Ticket.IPrint>().InstancePerLifetimeScope();

            builder.RegisterType<FtthService>().As<IFtthService>().InstancePerLifetimeScope();

            builder.RegisterType<ParitcularFtthService>().As<IParitcularFtthService>().InstancePerLifetimeScope();

            builder.RegisterType<WideFtthService>().As<IWideFtthService>().InstancePerLifetimeScope();

            builder.RegisterType<RatingService>().As<IRatingService>().InstancePerLifetimeScope();

            builder.RegisterType<CsReportService>().As<ICsReportService>().InstancePerLifetimeScope();

            builder.RegisterType<TechnologyService>().As<ITechnologyService>().InstancePerLifetimeScope();
        }

        /// <summary>
        /// Gets order of this dependency registrar implementation
        /// </summary>
        public int Order => 2;
    }
}

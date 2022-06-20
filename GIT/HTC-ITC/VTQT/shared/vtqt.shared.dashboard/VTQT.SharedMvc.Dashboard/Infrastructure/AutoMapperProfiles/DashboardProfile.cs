using AutoMapper;
using VTQT.Core.Domain.Dashboard;
using VTQT.Core.Infrastructure.Mapper;
using VTQT.SharedMvc.Dashboard.Models;
using VTQT.Web.Framework.Infrastructure.AutoMapperProfiles;

namespace VTQT.SharedMvc.Dashboard.Infrastructure.AutoMapperProfiles
{
    public class DashboardProfile : Profile, IOrderedMapperProfile
    {
        public DashboardProfile()
        {
            //add some generic mapping rules
            ForAllMaps(CommonProfile.AllMapsAction);

            #region TypeValue

            CreateMap<TypeValueModel, TypeValue>()
                .ForMember(x => x.TypeValueId_StorageValues, opt => opt.Ignore());

            CreateMap<TypeValue, TypeValueModel>()
                .ForMember(x => x.IsAuthorize, opt => opt.Ignore())
                .ForMember(x => x.AvailableTypeValue, opt => opt.Ignore());
            #endregion

            #region StorageValue
            CreateMap<StorageValueModel, StorageValue>()
                .ForMember(x => x.FKTypeValueId, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.CreatedDate, opt => opt.Ignore());

            CreateMap<StorageValue, StorageValueModel>()
                .ForMember(x => x.VoucherDate, opt => opt.Ignore())
                .ForMember(x => x.FormFile, opt => opt.Ignore())
                .ForMember(x => x.AvailableTypeValue, opt => opt.Ignore())
                .ForMember(x => x.AvailableNameTable, opt => opt.Ignore())
                .ForMember(x => x.AvailableUsers, opt => opt.Ignore())
                .ForMember(x => x.TypeValueName, opt => opt.Ignore());


            #endregion

            #region MasterVTCNTT
            CreateMap<MasterVTCNTTModel, MasterVTCNTT>();

            CreateMap<MasterVTCNTT, MasterVTCNTTModel>();


            #endregion

            #region Example
            CreateMap<ExampleModel, Example>();

            CreateMap<Example, ExampleModel>();
            #endregion

            #region FTTH
            CreateMap<FTTHModel, FTTH>();

            CreateMap<FTTH, FTTHModel>();
            #endregion

            #region RosePrice
            CreateMap<RosePriceModel, RosePrice>();

            CreateMap<RosePrice, RosePriceModel>();
            #endregion

            #region NameTableExist
            CreateMap<NameTableExistModel, NameTableExist>();

            CreateMap<NameTableExist, NameTableExistModel>();
            #endregion

            #region SelectTable
            CreateMap<SelectTableModel, SelectTable>();

            CreateMap<SelectTable, SelectTableModel>();
            #endregion

            #region MasterFileKH2022
            CreateMap<MasterFileKH2022Model, MasterFileKH2022>();

            CreateMap<MasterFileKH2022, MasterFileKH2022Model>();
            #endregion

            #region Report
            CreateMap<ReportModel, Report>()
                .ForMember(x => x.TableRefs, opt => opt.Ignore())
                .ForMember(x => x.ExcelData, opt => opt.Ignore())
                .ForMember(x => x.ReportGroupId, opt => opt.Ignore())
                .ForMember(x => x.ReportGroup, opt => opt.Ignore());

            CreateMap<Report, ReportModel>()
                .ForMember(x => x.Path, opt => opt.Ignore())
                .ForMember(x => x.ParentId, opt => opt.Ignore());
            #endregion

            #region ReportGroup
            CreateMap<ReportGroupModel, ReportGroup>()
                .ForMember(x => x.Path, opt => opt.Ignore())
                .ForMember(x => x.ParentId, opt => opt.Ignore())
                .ForMember(x => x.Reports, opt => opt.Ignore())
                .ForMember(x => x.ReportGroups, opt => opt.Ignore())
                .ForMember(x => x.Parent, opt => opt.Ignore());

            CreateMap<ReportGroup, ReportGroupModel>();
            #endregion

            #region FTTH2022
            CreateMap<FTTH2022Model, FTTH2022>();

            CreateMap<FTTH2022, FTTH2022Model>();
            #endregion

            #region AuthorizeToRole
            CreateMap<AuthorizeToRoleModel, AuthorizeToRole>();

            CreateMap<AuthorizeToRole, AuthorizeToRoleModel>()
                .ForMember(x => x.AvailableFiles, opt => opt.Ignore())
                .ForMember(x => x.AvailableTypeValues, opt => opt.Ignore())
                .ForMember(x => x.AvailableUsers, opt => opt.Ignore());
            #endregion

            #region DashBoardUser
            CreateMap<DashBoardUserModel, DashBoardUser>();

            CreateMap<DashBoardUser, DashBoardUserModel>()
                 .ForMember(x => x.WarehouseName, opt => opt.Ignore())
             .ForMember(x => x.UserName, opt => opt.Ignore())
            .ForMember(x => x.AccountName, opt => opt.Ignore());
            #endregion

            #region AuthorizeFile
            CreateMap<AuthorizeFileModel, AuthorizeFile>();

            CreateMap<AuthorizeFile, AuthorizeFileModel>();
            #endregion

            #region FTTHMB3132022
            CreateMap<FTTHMB3132022Model, FTTHMB3132022>();

            CreateMap<FTTHMB3132022, FTTHMB3132022Model>();
            #endregion

        }

        public int Order => 1;
    }
}

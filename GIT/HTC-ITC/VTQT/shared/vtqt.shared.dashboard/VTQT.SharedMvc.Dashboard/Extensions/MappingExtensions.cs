using VTQT.Core.Domain.Dashboard;
using VTQT.Core.Infrastructure.Mapper;
using VTQT.SharedMvc.Dashboard.Models;

namespace VTQT.SharedMvc.Dashboard
{
    public static class MappingExtensions
    {
        #region TypeValue

        public static TypeValue ToEntity(this TypeValueModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<TypeValueModel, TypeValue>(model);
        }

        public static TypeValue ToEntity(this TypeValueModel model, TypeValue destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static TypeValueModel ToModel(this TypeValue entity)
        {
            return AutoMapperConfiguration.Mapper.Map<TypeValue, TypeValueModel>(entity);
        }

        #endregion TypeValue

        #region StorageValue

        public static StorageValue ToEntity(this StorageValueModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<StorageValueModel, StorageValue>(model);
        }

        public static StorageValue ToEntity(this StorageValueModel model, StorageValue destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static StorageValueModel ToModel(this StorageValue entity)
        {
            return AutoMapperConfiguration.Mapper.Map<StorageValue, StorageValueModel>(entity);
        }

        #endregion StorageValue

        #region MasterVTCNTT

        public static MasterVTCNTT ToEntity(this MasterVTCNTTModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<MasterVTCNTTModel, MasterVTCNTT>(model);
        }

        public static MasterVTCNTT ToEntity(this MasterVTCNTTModel model, MasterVTCNTT destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static MasterVTCNTTModel ToModel(this MasterVTCNTT entity)
        {
            return AutoMapperConfiguration.Mapper.Map<MasterVTCNTT, MasterVTCNTTModel>(entity);
        }

        #endregion MasterVTCNTT

        #region Example

        public static Example ToEntity(this ExampleModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<ExampleModel, Example>(model);
        }

        public static Example ToEntity(this ExampleModel model, Example destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static ExampleModel ToModel(this Example entity)
        {
            return AutoMapperConfiguration.Mapper.Map<Example, ExampleModel>(entity);
        }

        #endregion Example

        #region FTTH

        public static FTTH ToEntity(this FTTHModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<FTTHModel, FTTH>(model);
        }

        public static FTTH ToEntity(this FTTHModel model, FTTH destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static FTTHModel ToModel(this FTTH entity)
        {
            return AutoMapperConfiguration.Mapper.Map<FTTH, FTTHModel>(entity);
        }

        #endregion FTTH

        #region RosePrice

        public static RosePrice ToEntity(this RosePriceModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<RosePriceModel, RosePrice>(model);
        }

        public static RosePrice ToEntity(this RosePriceModel model, RosePrice destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static RosePriceModel ToModel(this RosePrice entity)
        {
            return AutoMapperConfiguration.Mapper.Map<RosePrice, RosePriceModel>(entity);
        }

        #endregion RosePrice

        #region NameTableExist

        public static NameTableExist ToEntity(this NameTableExistModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<NameTableExistModel, NameTableExist>(model);
        }

        public static NameTableExist ToEntity(this NameTableExistModel model, NameTableExist destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static NameTableExistModel ToModel(this NameTableExist entity)
        {
            return AutoMapperConfiguration.Mapper.Map<NameTableExist, NameTableExistModel>(entity);
        }

        #endregion NameTableExist

        #region SelectTable

        public static SelectTable ToEntity(this SelectTableModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<SelectTableModel, SelectTable>(model);
        }

        public static SelectTable ToEntity(this SelectTableModel model, SelectTable destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static SelectTableModel ToModel(this SelectTable entity)
        {
            return AutoMapperConfiguration.Mapper.Map<SelectTable, SelectTableModel>(entity);
        }

        #endregion SelectTable

        #region MasterFileKH2022

        public static MasterFileKH2022 ToEntity(this MasterFileKH2022Model model)
        {
            return AutoMapperConfiguration.Mapper.Map<MasterFileKH2022Model, MasterFileKH2022>(model);
        }

        public static MasterFileKH2022 ToEntity(this MasterFileKH2022Model model, MasterFileKH2022 destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static MasterFileKH2022Model ToModel(this MasterFileKH2022 entity)
        {
            return AutoMapperConfiguration.Mapper.Map<MasterFileKH2022, MasterFileKH2022Model>(entity);
        }

        #endregion MasterFileKH2022

        #region Report

        public static Report ToEntity(this ReportModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<ReportModel, Report>(model);
        }

        public static Report ToEntity(this ReportModel model, Report destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static ReportModel ToModel(this Report entity)
        {
            return AutoMapperConfiguration.Mapper.Map<Report, ReportModel>(entity);
        }

        #endregion Report

        #region ReportGroup

        public static ReportGroup ToEntity(this ReportGroupModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<ReportGroupModel, ReportGroup>(model);
        }

        public static ReportGroup ToEntity(this ReportGroupModel model, ReportGroup destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static ReportGroupModel ToModel(this ReportGroup entity)
        {
            return AutoMapperConfiguration.Mapper.Map<ReportGroup, ReportGroupModel>(entity);
        }

        #endregion ReportGroup

        #region FTTH2022

        public static FTTH2022 ToEntity(this FTTH2022Model model)
        {
            return AutoMapperConfiguration.Mapper.Map<FTTH2022Model, FTTH2022>(model);
        }

        public static FTTH2022 ToEntity(this FTTH2022Model model, FTTH2022 destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static FTTH2022Model ToModel(this FTTH2022 entity)
        {
            return AutoMapperConfiguration.Mapper.Map<FTTH2022, FTTH2022Model>(entity);
        }

        #endregion FTTH2022

        #region AuthorizeToRole

        public static AuthorizeToRole ToEntity(this AuthorizeToRoleModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<AuthorizeToRoleModel, AuthorizeToRole>(model);
        }

        public static AuthorizeToRole ToEntity(this AuthorizeToRoleModel model, AuthorizeToRole destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static AuthorizeToRoleModel ToModel(this AuthorizeToRole entity)
        {
            return AutoMapperConfiguration.Mapper.Map<AuthorizeToRole, AuthorizeToRoleModel>(entity);
        }

        #endregion AuthorizeToRole

        #region DashBoardUser

        public static DashBoardUser ToEntity(this DashBoardUserModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<DashBoardUserModel, DashBoardUser>(model);
        }

        public static DashBoardUser ToEntity(this DashBoardUserModel model, DashBoardUser destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static DashBoardUserModel ToModel(this DashBoardUser entity)
        {
            return AutoMapperConfiguration.Mapper.Map<DashBoardUser, DashBoardUserModel>(entity);
        }

        #endregion DashBoardUser

        #region AuthorizeFile

        public static AuthorizeFile ToEntity(this AuthorizeFileModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<AuthorizeFileModel, AuthorizeFile>(model);
        }

        public static AuthorizeFile ToEntity(this AuthorizeFileModel model, AuthorizeFile destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static AuthorizeFileModel ToModel(this AuthorizeFile entity)
        {
            return AutoMapperConfiguration.Mapper.Map<AuthorizeFile, AuthorizeFileModel>(entity);
        }

        #endregion AuthorizeFile

        #region FTTHMB3132022

        public static FTTHMB3132022 ToEntity(this FTTHMB3132022Model model)
        {
            return AutoMapperConfiguration.Mapper.Map<FTTHMB3132022Model, FTTHMB3132022>(model);
        }

        public static FTTHMB3132022 ToEntity(this FTTHMB3132022Model model, FTTHMB3132022 destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static FTTHMB3132022Model ToModel(this FTTHMB3132022 entity)
        {
            return AutoMapperConfiguration.Mapper.Map<FTTHMB3132022, FTTHMB3132022Model>(entity);
        }

        #endregion FTTHMB3132022
    }
}
using AutoMapper;
using VTQT.Core.Domain.Warehouse;
using VTQT.Core.Infrastructure.Mapper;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Web.Framework.Infrastructure.AutoMapperProfiles;

namespace VTQT.SharedMvc.Warehouse.Infrastructure.AutoMapperProfiles
{
    public class WarehouseProfile : Profile, IOrderedMapperProfile
    {
        public WarehouseProfile()
        {
            //add some generic mapping rules
            ForAllMaps(CommonProfile.AllMapsAction);

            #region WareHouseUser
            CreateMap<WareHouseUser, WareHouseUserModel>()
                .ForMember(x => x.WarehouseName, opt => opt.Ignore())
             .ForMember(x => x.UserName, opt => opt.Ignore())
            .ForMember(x => x.AccountName, opt => opt.Ignore());

            CreateMap<WareHouseUserModel, WareHouseUser>();

            #endregion

            #region AccObject
            CreateMap<AccObject, AccObjectModel>()
                .ForMember(x => x.WareHouseInwards, opt => opt.Ignore())
             .ForMember(x => x.WareHouseInwardsDetails, opt => opt.Ignore())
             .ForMember(x => x.WareHouseOutwards, opt => opt.Ignore())
            .ForMember(x => x.WareHouseOutwardDetails, opt => opt.Ignore());

            CreateMap<AccObjectModel, AccObject>();

            #endregion


            #region WareHouse
            CreateMap<WareHouse, WareHouseModel>()
                .ForMember(x => x.GetListRole, opt => opt.Ignore())
                .ForMember(x => x.SetListRole, opt => opt.Ignore())
                .ForMember(x => x.AvailableWareHouses, opt => opt.Ignore())
                .ForMember(x => x.Locales, opt => opt.Ignore());

            CreateMap<WareHouseModel, WareHouse>()
                .ForMember(x => x.Code, opt => opt.Ignore())
                .ForMember(x => x.Audits, opt => opt.Ignore())
                .ForMember(x => x.BeginningWareHouses, opt => opt.Ignore())
                .ForMember(x => x.ToWareHouse_Outwards, opt => opt.Ignore())
                .ForMember(x => x.WareHouseInwards, opt => opt.Ignore())
                .ForMember(x => x.Warehouse_Outwards, opt => opt.Ignore())
                .ForMember(x => x.WareHouseLimits, opt => opt.Ignore());
            #endregion

            #region BeginningWareHouse
            CreateMap<BeginningWareHouse, BeginningWareHouseModel>()
                .ForMember(x => x.CreatedDate, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.ModifiedDate, opt => opt.Ignore())
                .ForMember(x => x.ModifiedBy, opt => opt.Ignore())
                .ForMember(x => x.UnitModel, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItemModel, opt => opt.Ignore())
                .ForMember(x => x.WareHouseModel, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItemName, opt => opt.Ignore());

            CreateMap<BeginningWareHouseModel, BeginningWareHouse>()
                .ForMember(x => x.Unit, opt => opt.Ignore())
                .ForMember(x => x.WareHouse, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItem, opt => opt.Ignore())
                .ForMember(x => x.CreatedDate, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                 .ForMember(x => x.ModifiedDate, opt => opt.Ignore())
                .ForMember(x => x.ModifiedBy, opt => opt.Ignore());
            #endregion

            #region Outward
            CreateMap<Outward, OutwardModel>()
                .ForMember(x => x.AvailableReasons, opt => opt.Ignore())
                //.ForMember(x => x.CreatedDate, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                //.ForMember(x => x.ModifiedDate, opt => opt.Ignore())
                .ForMember(x => x.ModifiedBy, opt => opt.Ignore())
                .ForMember(x => x.OutwardDetails, opt => opt.Ignore())
                .ForMember(x => x.References, opt => opt.Ignore())
                .ForMember(x => x.WareHouse, opt => opt.Ignore())
                .ForMember(x => x.ToWareHouse, opt => opt.Ignore())
                .ForMember(x => x.VoucherDate, opt => opt.Ignore())
                .ForMember(x => x.AvailableWareHouses, opt => opt.Ignore())
                .ForMember(x => x.AvailableAccObject, opt => opt.Ignore())
                .ForMember(x => x.AvailableToWareHouses, opt => opt.Ignore())
                .ForMember(x => x.AvailableCreatedBy, opt => opt.Ignore())
                .ForMember(x => x.DeleteDetailIds, opt => opt.Ignore());
            CreateMap<OutwardModel, Outward>()
                .ForMember(x => x.VoucherCode, opt => opt.Ignore())
                .ForMember(x => x.CreatedDate, opt => opt.Ignore())
                //.ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.ModifiedDate, opt => opt.Ignore())
                //.ForMember(x => x.ModifiedBy, opt => opt.Ignore())
                //.ForMember(x => x.Reference, opt => opt.Ignore())
                .ForMember(x => x.OutwardDetails, opt => opt.Ignore())
                .ForMember(x => x.ToWareHouse, opt => opt.Ignore())
                .ForMember(x => x.WareHouse, opt => opt.Ignore())
                .ForMember(x => x.AccObject, opt => opt.Ignore())
                .ForMember(x => x.VoucherDate, opt => opt.Ignore());
            #endregion

            #region OutwardDetail
            CreateMap<OutwardDetail, OutwardDetailModel>()
                .ForMember(x => x.AvailableAccountMores, opt => opt.Ignore())
                .ForMember(x => x.AvailableAccountYes, opt => opt.Ignore())
                .ForMember(x => x.Serial, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItem, opt => opt.Ignore())
                .ForMember(x => x.ItemName, opt => opt.Ignore())
                .ForMember(x => x.Unit, opt => opt.Ignore())
                .ForMember(x => x.UnitName, opt => opt.Ignore())
                .ForMember(x => x.Outward, opt => opt.Ignore())
                .ForMember(x => x.SerialWareHouses, opt => opt.Ignore())
                .ForMember(x => x.AvailableItems, opt => opt.Ignore())
                .ForMember(x => x.AvailableUnits, opt => opt.Ignore())
                 .ForMember(x => x.AvailableAccObject, opt => opt.Ignore())
                .ForMember(x => x.AvailableOrganizations, opt => opt.Ignore())
                .ForMember(x => x.AvailableUsers, opt => opt.Ignore())
                .ForMember(x => x.AvailableStations, opt => opt.Ignore())
                .ForMember(x => x.AvailableProjects, opt => opt.Ignore())
                .ForMember(x => x.AvailableCustomers, opt => opt.Ignore());
            CreateMap<OutwardDetailModel, OutwardDetail>()
                 .ForMember(x => x.Outward, opt => opt.Ignore())
                .ForMember(x => x.SerialWareHouses, opt => opt.Ignore())
                .ForMember(x => x.Unit, opt => opt.Ignore())
                                .ForMember(x => x.AccObject, opt => opt.Ignore())

                .ForMember(x => x.WareHouseItem, opt => opt.Ignore());
            #endregion

            #region SerialWareHouse
            CreateMap<SerialWareHouse, SerialWareHouseModel>();

            CreateMap<SerialWareHouseModel, SerialWareHouse>()
                .ForMember(x => x.InwardDetail, opt => opt.Ignore())
                .ForMember(x => x.OutwardDetail, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItem, opt => opt.Ignore());

            #endregion

            #region WarehouseBalance
            CreateMap<WarehouseBalance, WarehouseBalanceModel>()
                 .ForMember(x => x.UIQuantity, opt => opt.Ignore())
                .ForMember(x => x.WareHouseModel, opt => opt.Ignore());

            CreateMap<WarehouseBalanceModel, WarehouseBalance>();

            #endregion

            #region Unit
            CreateMap<Unit, UnitModel>()
                .ForMember(x => x.Locales, opt => opt.Ignore());

            CreateMap<UnitModel, Unit>()
                .ForMember(x => x.BeginningWareHouses, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItems, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItemUnits, opt => opt.Ignore())
                .ForMember(x => x.WareHouseLimits, opt => opt.Ignore())
                .ForMember(x => x.InwardDetails, opt => opt.Ignore())
                .ForMember(x => x.OutwardDetails, opt => opt.Ignore());
            #endregion

            #region Vendor
            CreateMap<Vendor, VendorModel>()
                .ForMember(x => x.Locales, opt => opt.Ignore());

            CreateMap<VendorModel, Vendor>()
                .ForMember(x => x.Code, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItems, opt => opt.Ignore())
                .ForMember(x => x.WareHouseInwards, opt => opt.Ignore());
            #endregion

            #region VoucherWareHouse

            CreateMap<Inward, VoucherWareHouseModel>()
                .ForMember(x => x.AvailableCreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UserModel, opt => opt.Ignore())
                .ForMember(x => x.VoucherDate, opt => opt.Ignore())
                .ForMember(x => x.VoucherType, opt => opt.Ignore())
                .ForMember(x => x.StrVoucherDate, opt => opt.Ignore())
                .ForMember(x => x.Voucher, opt => opt.Ignore())
                .ForMember(x => x.SelectedInwardReason, opt => opt.Ignore())
                .ForMember(x => x.SelectedOutwardReason, opt => opt.Ignore());

            CreateMap<Outward, VoucherWareHouseModel>()
                .ForMember(x => x.AvailableCreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UserModel, opt => opt.Ignore())
                .ForMember(x => x.VoucherDate, opt => opt.Ignore())
                .ForMember(x => x.VoucherType, opt => opt.Ignore())
                .ForMember(x => x.Voucher, opt => opt.Ignore())
                .ForMember(x => x.StrVoucherDate, opt => opt.Ignore())
                .ForMember(x => x.SelectedInwardReason, opt => opt.Ignore())
                .ForMember(x => x.SelectedOutwardReason, opt => opt.Ignore());
            #endregion

            #region WareHouseItemCategory
            CreateMap<WareHouseItemCategory, WareHouseItemCategoryModel>()
                .ForMember(x => x.AvailableCategories, opt => opt.Ignore())
                .ForMember(x => x.Locales, opt => opt.Ignore());

            CreateMap<WareHouseItemCategoryModel, WareHouseItemCategory>()
                .ForMember(x => x.Code, opt => opt.Ignore())
                .ForMember(x => x.Parent, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItems, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItemCategories, opt => opt.Ignore());
            #endregion  

            #region WareHouseItem
            CreateMap<WareHouseItem, WareHouseItemModel>()
                .ForMember(x => x.WareHouseItemUnits, opt => opt.Ignore())
                .ForMember(x => x.Locales, opt => opt.Ignore())
                .ForMember(x => x.AvailableUnits, opt => opt.Ignore())
                .ForMember(x => x.AvailableVendors, opt => opt.Ignore())
                .ForMember(x => x.AvailableWareHouseItemCategories, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItemCategoryModel, opt => opt.Ignore())
                .ForMember(x => x.VendorModel, opt => opt.Ignore())
                .ForMember(x => x.UnitModel, opt => opt.Ignore());

            CreateMap<WareHouseItemModel, WareHouseItem>()
                .ForMember(x => x.Code, opt => opt.Ignore())
                .ForMember(x => x.BeginningWareHouses, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItemCategory, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItemUnits, opt => opt.Ignore())
                .ForMember(x => x.WareHouseLimits, opt => opt.Ignore())
                .ForMember(x => x.SerialWareHouses, opt => opt.Ignore())
                .ForMember(x => x.AuditDetails, opt => opt.Ignore())
                .ForMember(x => x.AuditDetailSerials, opt => opt.Ignore())
                .ForMember(x => x.InwardDetails, opt => opt.Ignore())
                .ForMember(x => x.OutwardDetails, opt => opt.Ignore())
                .ForMember(x => x.Unit, opt => opt.Ignore())
                .ForMember(x => x.Vendor, opt => opt.Ignore());
            #endregion

            #region WareHouseItemUnit
            CreateMap<WareHouseItemUnit, WareHouseItemUnitModel>()
                 .ForMember(x => x.UnitName, opt => opt.Ignore())
                .ForMember(x => x.UnitModel, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItemModel, opt => opt.Ignore())
                .ForMember(x => x.Note, opt => opt.Ignore())
                .ForMember(x => x.Locales, opt => opt.Ignore())
                .ForMember(x => x.AvailableUnits, opt => opt.Ignore());

            CreateMap<WareHouseItemUnitModel, WareHouseItemUnit>()
                .ForMember(x => x.Unit, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItem, opt => opt.Ignore());
            #endregion

            #region WareHouseLimit

            CreateMap<WareHouseLimit, WareHouseLimitModel>()
                .ForMember(x => x.CreatedDate, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.ModifiedDate, opt => opt.Ignore())
                .ForMember(x => x.ModifiedBy, opt => opt.Ignore())
                .ForMember(x => x.UnitModel, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItemModel, opt => opt.Ignore())
                .ForMember(x => x.Quantity, opt => opt.Ignore())
                .ForMember(x => x.WareHouseModel, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItemName, opt => opt.Ignore());

            CreateMap<WareHouseLimitModel, WareHouseLimit>()
                .ForMember(x => x.Unit, opt => opt.Ignore())
                .ForMember(x => x.WareHouse, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItem, opt => opt.Ignore())
                .ForMember(x => x.CreatedDate, opt => opt.Ignore())
                .ForMember(x => x.ModifiedDate, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.ModifiedBy, opt => opt.Ignore());
            #endregion

            #region Inward
            CreateMap<InwardModel, Inward>()
                .ForMember(x => x.VoucherCode, opt => opt.Ignore())
                //.ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.CreatedDate, opt => opt.Ignore())
                .ForMember(x => x.InwardDetails, opt => opt.Ignore())
                //.ForMember(x => x.ModifiedBy, opt => opt.Ignore())
                .ForMember(x => x.ModifiedDate, opt => opt.Ignore())
                .ForMember(x => x.Vendor, opt => opt.Ignore())
                  .ForMember(x => x.AccObject, opt => opt.Ignore())
                .ForMember(x => x.WareHouse, opt => opt.Ignore())
                .ForMember(x => x.VoucherDate, opt => opt.Ignore());

            CreateMap<Inward, InwardModel>()
                .ForMember(x => x.AvailableReasons, opt => opt.Ignore())
                .ForMember(x => x.AvailableVendors, opt => opt.Ignore())
                //.ForMember(x => x.CreatedDate, opt => opt.Ignore())
                .ForMember(x => x.InwardDetails, opt => opt.Ignore())
                //.ForMember(x => x.ModifiedDate, opt => opt.Ignore())
                .ForMember(x => x.References, opt => opt.Ignore())
                .ForMember(x => x.Vendor, opt => opt.Ignore())

                .ForMember(x => x.WareHouse, opt => opt.Ignore())
                //.ForMember(x => x.VoucherDate, opt => opt.Ignore())
                .ForMember(x => x.AvailableWareHouses, opt => opt.Ignore())
                 .ForMember(x => x.AvailableAccObject, opt => opt.Ignore())
                .ForMember(x => x.AvailableCreatedBy, opt => opt.Ignore())
                .ForMember(x => x.DeleteDetailIds, opt => opt.Ignore());
            #endregion

            #region InwardDetail
            CreateMap<InwardDetail, InwardDetailModel>()
                .ForMember(x => x.AvailableAccountMores, opt => opt.Ignore())
                .ForMember(x => x.AvailableAccountYes, opt => opt.Ignore())
                .ForMember(x => x.Serial, opt => opt.Ignore())
                .ForMember(x => x.ItemName, opt => opt.Ignore())
                .ForMember(x => x.UnitName, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItem, opt => opt.Ignore())
                .ForMember(x => x.Unit, opt => opt.Ignore())
                .ForMember(x => x.Inward, opt => opt.Ignore())
                .ForMember(x => x.SerialWareHouses, opt => opt.Ignore())
                .ForMember(x => x.AvailableItems, opt => opt.Ignore())
                .ForMember(x => x.AvailableUnits, opt => opt.Ignore())
                .ForMember(x => x.AvailableOrganizations, opt => opt.Ignore())
                .ForMember(x => x.AvailableAccObject, opt => opt.Ignore())
                .ForMember(x => x.AvailableUsers, opt => opt.Ignore())
                .ForMember(x => x.AvailableStations, opt => opt.Ignore())
                .ForMember(x => x.AvailableProjects, opt => opt.Ignore())
                .ForMember(x => x.AvailableCustomers, opt => opt.Ignore());

            CreateMap<InwardDetailModel, InwardDetail>()
                .ForMember(x => x.Inward, opt => opt.Ignore())
                .ForMember(x => x.SerialWareHouses, opt => opt.Ignore())
                .ForMember(x => x.Unit, opt => opt.Ignore())
                 .ForMember(x => x.AccObject, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItem, opt => opt.Ignore());

            #endregion

            #region Audit
            CreateMap<Audit, AuditModel>()
                .ForMember(x => x.StringVoucherDate, opt => opt.Ignore())
                .ForMember(x => x.AuditCouncils, opt => opt.Ignore())
                .ForMember(x => x.AuditDetails, opt => opt.Ignore())
                .ForMember(x => x.AvailableAuditTimes, opt => opt.Ignore())
                .ForMember(x => x.AvailableWareHouses, opt => opt.Ignore())
                .ForMember(x => x.AvailableCreatedBy, opt => opt.Ignore())
                .ForMember(x => x.DeleteDetailIds, opt => opt.Ignore())
                //.ForMember(x => x.CreatedDate, opt => opt.Ignore())
                //.ForMember(x => x.ModifiedDate, opt => opt.Ignore())
                //.ForMember(x => x.VoucherDate, opt => opt.Ignore())
                .ForMember(x => x.WareHouse, opt => opt.Ignore());

            CreateMap<AuditModel, Audit>()
                .ForMember(x => x.VoucherCode, opt => opt.Ignore())
                .ForMember(x => x.WareHouse, opt => opt.Ignore())
                .ForMember(x => x.VoucherDate, opt => opt.Ignore())
                .ForMember(x => x.ModifiedDate, opt => opt.Ignore())
                //.ForMember(x => x.ModifiedBy, opt => opt.Ignore())
                .ForMember(x => x.CreatedDate, opt => opt.Ignore())
                //.ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.AuditDetails, opt => opt.Ignore())
                .ForMember(x => x.AuditCouncils, opt => opt.Ignore());
            #endregion

            #region AuditDetail
            CreateMap<AuditDetail, AuditDetailModel>()
                .ForMember(x => x.AvailableUnits, opt => opt.Ignore())
                .ForMember(x => x.UnitId, opt => opt.Ignore())
                .ForMember(x => x.Serial, opt => opt.Ignore())
                .ForMember(x => x.AuditDetailSerials, opt => opt.Ignore())
                .ForMember(x => x.AvailableUsers, opt => opt.Ignore())
                .ForMember(x => x.AvailableItems, opt => opt.Ignore())
                .ForMember(x => x.ItemName, opt => opt.Ignore())
                .ForMember(x => x.Audit, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItem, opt => opt.Ignore());

            CreateMap<AuditDetailModel, AuditDetail>()
                .ForMember(x => x.Audit, opt => opt.Ignore())
                .ForMember(x => x.AuditDetailSerials, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItem, opt => opt.Ignore());
            #endregion

            #region AuditDetailSerial
            CreateMap<AuditDetailSerial, AuditDetailSerialModel>()
                .ForMember(x => x.AuditDetailModel, opt => opt.Ignore())
                .ForMember(x => x.WareHouseItemModel, opt => opt.Ignore());

            CreateMap<AuditDetailSerialModel, AuditDetailSerial>()
                .ForMember(x => x.WareHouseItem, opt => opt.Ignore())
                .ForMember(x => x.AuditDetail, opt => opt.Ignore());
            #endregion

            #region AuditCouncil
            CreateMap<AuditCouncil, AuditCouncilModel>()
                .ForMember(x => x.AvailableUsers, opt => opt.Ignore())
                .ForMember(x => x.AuditModel, opt => opt.Ignore());

            CreateMap<AuditCouncilModel, AuditCouncil>()
                .ForMember(x => x.Audit, opt => opt.Ignore());
            #endregion

            #region AdminRoleWareHouse
            CreateMap<AdminRoleWareHouse, AdminRoleWareHouseModel>()
                .ForMember(x => x.AvailableCreatedBy, opt => opt.Ignore());

            CreateMap<AdminRoleWareHouseModel, AdminRoleWareHouse>();
            #endregion
        }

        public int Order => 1;
    }
}

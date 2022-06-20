using VTQT.Core.Domain.Warehouse;
using VTQT.Core.Infrastructure.Mapper;
using VTQT.SharedMvc.Warehouse.Models;

namespace VTQT.SharedMvc.Warehouse
{
    public static class MappingExtensions
    {
        #region WareHouseUser

        public static WareHouseUserModel ToModel(this WareHouseUser entity)
        {
            return AutoMapperConfiguration.Mapper.Map<WareHouseUser, WareHouseUserModel>(entity);
        }

        public static WareHouseUser ToEntity(this WareHouseUserModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<WareHouseUserModel, WareHouseUser>(model);
        }

        public static WareHouseUser ToEntity(this WareHouseUserModel model, WareHouseUser destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        #endregion

        #region AccObject

        public static AccObjectModel ToModel(this AccObject entity)
        {
            return AutoMapperConfiguration.Mapper.Map<AccObject, AccObjectModel>(entity);
        }

        public static AccObject ToEntity(this AccObjectModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<AccObjectModel, AccObject>(model);
        }

        public static AccObject ToEntity(this AccObjectModel model, AccObject destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        #endregion

        #region WareHouse
        public static WareHouseModel ToModel(this WareHouse entity)
        {
            return AutoMapperConfiguration.Mapper.Map<WareHouse, WareHouseModel>(entity);
        }

        public static WareHouse ToEntity(this WareHouseModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<WareHouseModel, WareHouse>(model);
        }

        public static WareHouse ToEntity(this WareHouseModel model, WareHouse destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }
        #endregion

        #region Vendor
        public static VendorModel ToModel(this Vendor entity)
        {
            return AutoMapperConfiguration.Mapper.Map<Vendor, VendorModel>(entity);
        }

        public static Vendor ToEntity(this VendorModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<VendorModel, Vendor>(model);
        }

        public static Vendor ToEntity(this VendorModel model, Vendor destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }
        #endregion

        #region VoucherWareHouse
        public static VoucherWareHouseModel InwardToVoucherModel(this Inward entity)
        {
            return AutoMapperConfiguration.Mapper.Map<Inward, VoucherWareHouseModel>(entity);
        }

        public static VoucherWareHouseModel OutwardToVoucherModel(this Outward entity)
        {
            return AutoMapperConfiguration.Mapper.Map<Outward, VoucherWareHouseModel>(entity);
        }
        #endregion

        #region Unit
        public static Unit ToEntity(this UnitModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<UnitModel, Unit>(model);
        }

        public static UnitModel ToModel(this Unit unit)
        {
            return AutoMapperConfiguration.Mapper.Map<Unit, UnitModel>(unit);
        }

        public static Unit ToEntity(this UnitModel model, Unit destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }
        #endregion

        #region WareHouseItemCategory
        public static WareHouseItemCategory ToEntity(this WareHouseItemCategoryModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<WareHouseItemCategoryModel, WareHouseItemCategory>(model);
        }

        public static WareHouseItemCategory ToEntity(this WareHouseItemCategoryModel model, WareHouseItemCategory destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static WareHouseItemCategoryModel ToModel(this WareHouseItemCategory entity)
        {
            return AutoMapperConfiguration.Mapper.Map<WareHouseItemCategory, WareHouseItemCategoryModel>(entity);
        }
        #endregion

        #region WareHouseItem
        public static WareHouseItem ToEntity(this WareHouseItemModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<WareHouseItemModel, WareHouseItem>(model);
        }

        public static WareHouseItem ToEntity(this WareHouseItemModel model, WareHouseItem destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static WareHouseItemModel ToModel(this WareHouseItem entity)
        {
            return AutoMapperConfiguration.Mapper.Map<WareHouseItem, WareHouseItemModel>(entity);
        }
        #endregion

        #region WareHouseItemUnit
        public static WareHouseItemUnit ToEntity(this WareHouseItemUnitModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<WareHouseItemUnitModel, WareHouseItemUnit>(model);
        }

        public static WareHouseItemUnit ToEntity(this WareHouseItemUnitModel model, WareHouseItemUnit destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static WareHouseItemUnitModel ToModel(this WareHouseItemUnit entity)
        {
            return AutoMapperConfiguration.Mapper.Map<WareHouseItemUnit, WareHouseItemUnitModel>(entity);
        }
        #endregion

        #region WareHouseLimit
        public static WareHouseLimit ToEntity(this WareHouseLimitModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<WareHouseLimitModel, WareHouseLimit>(model);
        }

        public static WareHouseLimit ToEntity(this WareHouseLimitModel model, WareHouseLimit destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static WareHouseLimitModel ToModel(this WareHouseLimit entity)
        {
            return AutoMapperConfiguration.Mapper.Map<WareHouseLimit, WareHouseLimitModel>(entity);
        }
        #endregion

        #region Inward
        public static Inward ToEntity(this InwardModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<InwardModel, Inward>(model);
        }

        public static Inward ToEntity(this InwardModel model, Inward destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static InwardModel ToModel(this Inward entity)
        {
            return AutoMapperConfiguration.Mapper.Map<Inward, InwardModel>(entity);
        }
        #endregion

        #region InwardDetail
        public static InwardDetail ToEntity(this InwardDetailModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<InwardDetailModel, InwardDetail>(model);
        }

        public static InwardDetail ToEntity(this InwardDetailModel model, InwardDetail destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static InwardDetailModel ToModel(this InwardDetail entity)
        {
            return AutoMapperConfiguration.Mapper.Map<InwardDetail, InwardDetailModel>(entity);
        }
        #endregion

        #region Audit
        public static Audit ToEntity(this AuditModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<AuditModel, Audit>(model);
        }

        public static Audit ToEntity(this AuditModel model, Audit destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static AuditModel ToModel(this Audit entity)
        {
            return AutoMapperConfiguration.Mapper.Map<Audit, AuditModel>(entity);
        }
        #endregion

        #region AuditDetail
        public static AuditDetail ToEntity(this AuditDetailModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<AuditDetailModel, AuditDetail>(model);
        }

        public static AuditDetail ToEntity(this AuditDetailModel model, AuditDetail destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static AuditDetailModel ToModel(this AuditDetail entity)
        {
            return AutoMapperConfiguration.Mapper.Map<AuditDetail, AuditDetailModel>(entity);
        }
        #endregion

        #region AuditDetailSerial
        public static AuditDetailSerial ToEntity(this AuditDetailSerialModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<AuditDetailSerialModel, AuditDetailSerial>(model);
        }

        public static AuditDetailSerial ToEntity(this AuditDetailSerialModel model, AuditDetailSerial destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static AuditDetailSerialModel ToModel(this AuditDetailSerial entity)
        {
            return AutoMapperConfiguration.Mapper.Map<AuditDetailSerial, AuditDetailSerialModel>(entity);
        }
        #endregion

        #region AuditCouncil
        public static AuditCouncil ToEntity(this AuditCouncilModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<AuditCouncilModel, AuditCouncil>(model);
        }

        public static AuditCouncil ToEntity(this AuditCouncilModel model, AuditCouncil destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static AuditCouncilModel ToModel(this AuditCouncil entity)
        {
            return AutoMapperConfiguration.Mapper.Map<AuditCouncil, AuditCouncilModel>(entity);
        }
        #endregion

        #region BeginningWareHouse
        public static BeginningWareHouse ToEntity(this BeginningWareHouseModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<BeginningWareHouseModel, BeginningWareHouse>(model);
        }

        public static BeginningWareHouse ToEntity(this BeginningWareHouseModel model, BeginningWareHouse destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static BeginningWareHouseModel ToModel(this BeginningWareHouse entity)
        {
            return AutoMapperConfiguration.Mapper.Map<BeginningWareHouse, BeginningWareHouseModel>(entity);
        }
        #endregion

        #region Outward
        public static Outward ToEntity(this OutwardModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<OutwardModel, Outward>(model);
        }

        public static Outward ToEntity(this OutwardModel model, Outward destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static OutwardModel ToModel(this Outward entity)
        {
            return AutoMapperConfiguration.Mapper.Map<Outward, OutwardModel>(entity);
        }

        #endregion

        #region OutwardDetail
        public static OutwardDetail ToEntity(this OutwardDetailModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<OutwardDetailModel, OutwardDetail>(model);
        }

        public static OutwardDetail ToEntity(this OutwardDetailModel model, OutwardDetail destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static OutwardDetailModel ToModel(this OutwardDetail entity)
        {
            return AutoMapperConfiguration.Mapper.Map<OutwardDetail, OutwardDetailModel>(entity);
        }



        #endregion

        #region SerialWareHouse
        public static SerialWareHouse ToEntity(this SerialWareHouseModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<SerialWareHouseModel, SerialWareHouse>(model);
        }

        public static SerialWareHouse ToEntity(this SerialWareHouseModel model, SerialWareHouse destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static SerialWareHouseModel ToModel(this SerialWareHouse entity)
        {
            return AutoMapperConfiguration.Mapper.Map<SerialWareHouse, SerialWareHouseModel>(entity);
        }
        #endregion

        #region WarehouseBalance
        public static WarehouseBalance ToEntity(this WarehouseBalanceModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<WarehouseBalanceModel, WarehouseBalance>(model);
        }

        public static WarehouseBalance ToEntity(this WarehouseBalanceModel model, WarehouseBalance destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static WarehouseBalanceModel ToModel(this WarehouseBalance entity)
        {
            return AutoMapperConfiguration.Mapper.Map<WarehouseBalance, WarehouseBalanceModel>(entity);
        }
        #endregion

        #region AdminRoleWareHouse
        public static AdminRoleWareHouseModel ToModel(this AdminRoleWareHouse entity)
        {
            return AutoMapperConfiguration.Mapper.Map<AdminRoleWareHouse, AdminRoleWareHouseModel>(entity);
        }

        public static AdminRoleWareHouse ToEntity(this AdminRoleWareHouseModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<AdminRoleWareHouseModel, AdminRoleWareHouse>(model);
        }

        public static AdminRoleWareHouse ToEntity(this AdminRoleWareHouseModel model, AdminRoleWareHouse destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }
        #endregion
    }
}

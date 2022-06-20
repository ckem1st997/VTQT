using AutoMapper;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Infrastructure.Mapper;
using VTQT.SharedMvc.Asset.Models;
using VTQT.Web.Framework.Infrastructure.AutoMapperProfiles;

namespace VTQT.SharedMvc.Asset.Infrastructure.AutoMapperProfiles
{
    public class AssetProfile : Profile, IOrderedMapperProfile
    {
        public AssetProfile()
        {
            ForAllMaps(CommonProfile.AllMapsAction);

            #region Asset
            // VTQT.SharedMvc.Asset not a class
            CreateMap<Core.Domain.Asset.Asset, AssetModel>()
                .ForMember(x => x.Locales, opt => opt.Ignore())
                .ForMember(x => x.Reference, opt => opt.Ignore())
                .ForMember(x => x.CreatedDate, opt => opt.Ignore())
                .ForMember(x => x.ModifiedDate, opt => opt.Ignore())
                .ForMember(x => x.SelectedDepreciationUnit, opt => opt.Ignore())
                .ForMember(x => x.SelectedWarrantyUnit, opt => opt.Ignore())
                .ForMember(x => x.AvailableAssetStatus, opt => opt.Ignore())
                .ForMember(x => x.AvailableCategories, opt => opt.Ignore())
                .ForMember(x => x.AvailableCustomers, opt => opt.Ignore())
                .ForMember(x => x.AvailableDurations, opt => opt.Ignore())
                .ForMember(x => x.AvailableItems, opt => opt.Ignore())
                .ForMember(x => x.AvailableOrganizations, opt => opt.Ignore())
                .ForMember(x => x.AvailableProjects, opt => opt.Ignore())
                .ForMember(x => x.AvailableStations, opt => opt.Ignore())
                .ForMember(x => x.AvailableUsers, opt => opt.Ignore())
                .ForMember(x => x.MantainDate, opt => opt.Ignore())
                .ForMember(x => x.BalanceQuantity, opt => opt.Ignore())
                .ForMember(x => x.StationAddress, opt => opt.Ignore())
                .ForMember(x => x.StationArea, opt => opt.Ignore())
                .ForMember(x => x.StationCategory, opt => opt.Ignore())
                .ForMember(x => x.StationLatitude, opt => opt.Ignore())
                .ForMember(x => x.StationLongitude, opt => opt.Ignore())
                .ForMember(x => x.CustomerAddress, opt => opt.Ignore());
            CreateMap<AssetModel, Core.Domain.Asset.Asset>()
                .ForMember(x => x.Reference, opt => opt.Ignore())
                .ForMember(x => x.Asset_CategoryId, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.CreatedDate, opt => opt.Ignore())
                .ForMember(x => x.ModifiedBy, opt => opt.Ignore())
                .ForMember(x => x.ModifiedDate, opt => opt.Ignore())
                .ForMember(x => x.AuditDetails, opt => opt.Ignore())
                .ForMember(x => x.FK_History_AssetId_BackReferences, opt => opt.Ignore())
                .ForMember(x => x.FK_MaintenamceDetail_AssetId_BackReferences, opt => opt.Ignore())
                .ForMember(x => x.BrokenQuantity, opt => opt.Ignore())
                .ForMember(x => x.OriginQuantity, opt => opt.Ignore())
                .ForMember(x => x.SoldQuantity, opt => opt.Ignore())
                .ForMember(x => x.RecallQuantity, opt => opt.Ignore());
            #endregion

            #region AssetCategory
            CreateMap<AssetCategory, AssetCategoryModel>()
                .ForMember(x => x.Locales, opt => opt.Ignore())
                .ForMember(x => x.SelectedDepreciationUnit, opt => opt.Ignore())
                .ForMember(x => x.SelectedWarrantyUnit, opt => opt.Ignore());

            CreateMap<AssetCategoryModel, AssetCategory>()
                .ForMember(x => x.Code, opt => opt.Ignore())
                .ForMember(x => x.AssetCategory_ParentId, opt => opt.Ignore())
                .ForMember(x => x.FK_AssetCategory_ParentId_BackReferences, opt => opt.Ignore())
                .ForMember(x => x.FK_Asset_CategoryId_BackReferences, opt => opt.Ignore());
            #endregion

            #region AssetDecreased
            CreateMap<AssetDecreased, AssetDecreasedModel>()
                .ForMember(x => x.DecreaseDate, opt => opt.Ignore())
                .ForMember(x => x.CreatedDate, opt => opt.Ignore())
                .ForMember(x => x.ModifiedDate, opt => opt.Ignore())
                .ForMember(x => x.AvailableReasons, opt => opt.Ignore())
                .ForMember(x => x.AvailableAssets, opt => opt.Ignore())
                .ForMember(x => x.AvailableUsers, opt => opt.Ignore())
                .ForMember(x => x.AvailableWarehouses, opt => opt.Ignore());

            CreateMap<AssetDecreasedModel, AssetDecreased>()
                .ForMember(x => x.DecreaseDate, opt => opt.Ignore())
                .ForMember(x => x.CreatedDate, opt => opt.Ignore())
                .ForMember(x => x.ModifiedDate, opt => opt.Ignore());
            #endregion

            #region History
            CreateMap<History, HistoryModel>()
                .ForMember(x => x.TimeStamp, opt => opt.Ignore());
            CreateMap<HistoryModel, History>()
                .ForMember(x => x.TimeStamp, opt => opt.Ignore())
                .ForMember(x => x.History_AssetId, opt => opt.Ignore());
            #endregion

            #region Maintenance
            CreateMap<Maintenance, MaintenanceModel>()
                .ForMember(x => x.MaintenancedDate, opt => opt.Ignore())
                .ForMember(x => x.AvailableActions, opt => opt.Ignore())
                .ForMember(x => x.AvailableUsers, opt => opt.Ignore())
                .ForMember(x => x.MaintenanceDetails, opt => opt.Ignore());
            CreateMap<MaintenanceModel, Maintenance>()
                .ForMember(x => x.MaintenancedDate, opt => opt.Ignore())
                .ForMember(x => x.FK_MaintenamceDetail_MaintenanceId_BackReferences, opt => opt.Ignore());
            #endregion

            #region MaintenanceDetail
            CreateMap<MaintenanceDetailModel, MaintenanceDetail>()
                .ForMember(x => x.MaintenamceDetail_AssetId, opt => opt.Ignore())
                .ForMember(x => x.MaintenamceDetail_MaintenanceId, opt => opt.Ignore());
            CreateMap<MaintenanceDetail, MaintenanceDetailModel>()
                .ForMember(x => x.AssetCode, opt => opt.Ignore())
                .ForMember(x => x.AssetName, opt => opt.Ignore())
                .ForMember(x => x.AssetCategoryId, opt => opt.Ignore())
                .ForMember(x => x.AvailableAssets, opt => opt.Ignore());
            #endregion            
        }

        public int Order => 1;
    }
}

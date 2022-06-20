using VTQT.Core.Domain.Asset;
using VTQT.Core.Infrastructure.Mapper;
using VTQT.SharedMvc.Asset.Models;

namespace VTQT.SharedMvc.Asset.Extensions
{
    public static class MappingExtensions
    {
        #region Asset
        public static AssetModel ToModel(this Core.Domain.Asset.Asset entity)
        {
            return AutoMapperConfiguration.Mapper.Map<Core.Domain.Asset.Asset, AssetModel>(entity);
        }

        public static Core.Domain.Asset.Asset ToEntity(this AssetModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<AssetModel, Core.Domain.Asset.Asset>(model);
        }

        public static Core.Domain.Asset.Asset ToEntity(this AssetModel model, Core.Domain.Asset.Asset destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }
        #endregion

        #region AssetCategory
        public static AssetCategoryModel ToModel(this AssetCategory entity)
        {
            return AutoMapperConfiguration.Mapper.Map<AssetCategory, AssetCategoryModel>(entity);
        }

        public static AssetCategory ToEntity(this AssetCategoryModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<AssetCategoryModel, AssetCategory>(model);
        }

        public static AssetCategory ToEntity(this AssetCategoryModel model, AssetCategory destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }
        #endregion

        #region AssetDecreased
        public static AssetDecreasedModel ToModel(this AssetDecreased entity)
        {
            return AutoMapperConfiguration.Mapper.Map<AssetDecreased, AssetDecreasedModel>(entity);
        }

        public static AssetDecreased ToEntity(this AssetDecreasedModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<AssetDecreasedModel, AssetDecreased>(model);
        }

        public static AssetDecreased ToEntity(this AssetDecreasedModel model, AssetDecreased destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }
        #endregion

        #region History
        public static HistoryModel ToModel(this History entity)
        {
            return AutoMapperConfiguration.Mapper.Map<History, HistoryModel>(entity);
        }

        public static History ToEntity(this HistoryModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<HistoryModel, History>(model);
        }

        public static History ToEntity(this HistoryModel model, History destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }
        #endregion

        #region Maintenance
        public static MaintenanceModel ToModel(this Maintenance entity)
        {
            return AutoMapperConfiguration.Mapper.Map<Maintenance, MaintenanceModel>(entity);
        }

        public static Maintenance ToEntity(this MaintenanceModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<MaintenanceModel, Maintenance>(model);
        }

        public static Maintenance ToEntity(this MaintenanceModel model, Maintenance destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }
        #endregion

        #region MaintenanceDetail
        public static MaintenanceDetailModel ToModel(this MaintenanceDetail entity)
        {
            return AutoMapperConfiguration.Mapper.Map<MaintenanceDetail, MaintenanceDetailModel>(entity);
        }

        public static MaintenanceDetail ToEntity(this MaintenanceDetailModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<MaintenanceDetailModel, MaintenanceDetail>(model);
        }

        public static MaintenanceDetail ToEntity(this MaintenanceDetailModel model, MaintenanceDetail destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }
        #endregion        
    }
}

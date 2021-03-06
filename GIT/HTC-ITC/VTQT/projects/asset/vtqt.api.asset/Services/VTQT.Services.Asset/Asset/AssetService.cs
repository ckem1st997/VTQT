using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Master;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;
using LinqToDB;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Domain.FbmOrganization;
using VTQT.Core.Domain.Asset.Enum;
using VTQT.Core.Domain.FbmContract;

namespace VTQT.Services.Asset
{
    public partial class AssetService : IAssetService
    {
        #region Fields
        private readonly IRepository<Core.Domain.Asset.Asset> _assetRepository;
        private readonly IRepository<History> _historyRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IIntRepository<OrganizationUnit> _organizationRepository;
        private readonly IRepository<MaintenanceDetail> _maintenanceDetailRepository;
        private readonly IRepository<Station> _stationRepository;
        private readonly IIntRepository<Project> _projectRepository;
        private readonly IIntRepository<MarketArea> _marketAreaRepository;
        #endregion

        #region Ctor
        public AssetService()
        {
            _assetRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Asset.Asset>>(DataConnectionHelper.ConnectionStringNames.Asset);
            _historyRepository = EngineContext.Current.Resolve<IRepository<History>>(DataConnectionHelper.ConnectionStringNames.Asset);
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
            _organizationRepository = EngineContext.Current.Resolve<IIntRepository<OrganizationUnit>>(DataConnectionHelper.ConnectionStringNames.FbmOrganization);
            _maintenanceDetailRepository = EngineContext.Current.Resolve<IRepository<MaintenanceDetail>>(DataConnectionHelper.ConnectionStringNames.Asset);
            _stationRepository = EngineContext.Current.Resolve<IRepository<Station>>(DataConnectionHelper.ConnectionStringNames.Asset);
            _projectRepository = EngineContext.Current.Resolve<IIntRepository<Project>>(DataConnectionHelper.ConnectionStringNames.FbmContract);
            _marketAreaRepository =
                EngineContext.Current.Resolve<IIntRepository<MarketArea>>(DataConnectionHelper.ConnectionStringNames
                    .FbmContract);
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(Core.Domain.Asset.Asset entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            if (!string.IsNullOrEmpty(entity.ProjectCode))
            {
                var proj = _projectRepository.Table.FirstOrDefault(x => x.ProjectCode == entity.ProjectCode);
                if (proj != null)
                {
                    entity.ProjectName = $"[{proj.ProjectCode}] {proj.ProjectName}";
                    entity.ProjectAddress = proj.District + ", " + proj.City;
                    entity.ProjectProvince = proj.City;
                    var queryProjArea = from p in _projectRepository.Table
                        join a in _marketAreaRepository.Table on p.MarketAreaId equals a.Id into ap
                        from pa in ap.DefaultIfEmpty()
                        where p.ProjectCode == entity.ProjectCode
                        select pa.MarketName;
                    if (queryProjArea?.ToList()?.Count > 0)
                    {
                        entity.ProjectArea = queryProjArea.ToList()[0];
                    }
                }
            }

            var result = await _assetRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(Core.Domain.Asset.Asset entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            if (!string.IsNullOrEmpty(entity.ProjectCode))
            {
                var proj = _projectRepository.Table.FirstOrDefault(x => x.ProjectCode == entity.ProjectCode);
                if (proj != null)
                {
                    entity.ProjectName = proj.ProjectName;
                    entity.ProjectAddress = proj.District + ", " + proj.City;
                    entity.ProjectProvince = proj.City;
                    var queryProjArea = from p in _projectRepository.Table
                        join a in _marketAreaRepository.Table on p.MarketAreaId equals a.Id into ap
                        from pa in ap.DefaultIfEmpty()
                        where p.ProjectCode == entity.ProjectCode
                        select pa.MarketName;
                    if (queryProjArea?.ToList()?.Count > 0)
                    {
                        entity.ProjectArea = queryProjArea.ToList()[0];
                    }
                }
            }

            var result = await _assetRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeleteAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            if (ids.ToList().Count > 0)
            {
                ids.ToList().ForEach(async x =>
                {
                    var histories = _historyRepository.Table.Where(h => h.AssetId == x).Select(h => h.Id);
                    if (histories?.ToList()?.Count > 0)
                    {
                        await _historyRepository.DeleteAsync(histories.ToList());
                    }

                    var maintenanceDetails = _maintenanceDetailRepository.Table.Where(m => m.AssetId == x).Select(m => m.Id);
                    if (maintenanceDetails?.ToList()?.Count > 0)
                    {
                        await _maintenanceDetailRepository.DeleteAsync(maintenanceDetails.ToList());
                    }
                });
            }

            var result = await _assetRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<bool> ExistAsync(string code)
        {
            return await _assetRepository.Table
                .AnyAsync(x => x.Code != null
                          && x.Code.Equals(code));
        }

        public async Task<Core.Domain.Asset.Asset> GetByIdAsync(string id)
        {
            if (id.IsEmpty())
                throw new ArgumentException(nameof(id));

            return await _assetRepository.GetByIdAsync(id);
        }
        #endregion

        #region List
        public IPagedList<Core.Domain.Asset.Asset> GetMvcDropdownList(AssetSearchContext ctx)
        {
            var query = from p in _assetRepository.Table
                where p.AssetType == ctx.AssetType
                select p;

            if (!string.IsNullOrEmpty(ctx.Keywords))
            {
                query = from p in query
                    where p.Name.ToLower().Contains(ctx.Keywords.ToLower()) ||
                          p.Code.ToLower().Contains(ctx.Keywords.ToLower())
                    select p;

            }

            return new PagedList<Core.Domain.Asset.Asset>(query, ctx.PageIndex, 10);
        }
        
        public IList<Core.Domain.Asset.Asset> GetAll(int assetType)
        {
            var query = from p in _assetRepository.Table
                        where p.AssetType == assetType
                        select p;

            query = from p in query
                    orderby p.Code
                    select p;

            return query.ToList();
        }

        public IPagedList<AssetSvcEntity> Get(AssetSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _assetRepository.Table
                        where p.AssetType == ctx.AssetType
                        select new AssetSvcEntity
                        {
                            Id = p.Id,
                            Code = p.Code,
                            Name = p.Name,
                            OrganizationUnitId = p.OrganizationUnitId,
                            OrganizationUnitName = p.OrganizationUnitName,
                            EmployeeId = p.EmployeeId,
                            EmployeeName = p.EmployeeName,
                            ProjectCode = p.ProjectCode,
                            ProjectName = p.ProjectName,
                            CurrentUsageStatus = p.CurrentUsageStatus,
                            CustomerCode = p.CustomerCode,
                            CustomerName = p.CustomerName,
                            CategoryId = p.CategoryId,
                            AllocationDate = p.AllocationDate,
                            CreatedBy = p.CreatedBy,
                            CreatedDate = p.CreatedDate,
                            ModifiedBy = p.ModifiedBy,
                            ModifiedDate = p.ModifiedDate,
                            UnitId = p.UnitId,
                            UnitName = p.UnitName,
                            MaintenancedDate = p.MaintenancedDate,
                            Serial = p.Serial,
                            Description = p.Description,
                            ManufactureYear = p.ManufactureYear,
                            Status = p.Status,
                            Reference = p.Reference,
                            WareHouseItemCode = p.WareHouseItemCode,
                            WareHouseItemName = p.WareHouseItemName,
                            OriginQuantity = p.OriginQuantity,
                            RecallQuantity = p.RecallQuantity,
                            SoldQuantity = p.SoldQuantity,
                            BrokenQuantity = p.BrokenQuantity,
                            OriginUsageStatus = p.OriginUsageStatus,
                            OriginValue = p.OriginValue,
                            CurrentValue = p.CurrentValue,
                            DepreciationDuration = p.DepreciationDuration,
                            DepreciationUnit = p.DepreciationUnit,
                            WarrantyCondition = p.WarrantyCondition,
                            WarrantyDuration = p.WarrantyDuration,
                            WarrantyUnit = p.WarrantyUnit,
                            VendorName = p.VendorName,
                            Country = p.Country,
                            AssetType = p.AssetType,
                            StationCode = p.StationCode,
                            StationName = p.StationName,
                            ProjectAddress = p.ProjectAddress,
                            ProjectArea = p.ProjectArea,
                            ProjectCategory = p.ProjectCategory,
                            ProjectProvince = p.ProjectProvince,
                            IpAddress = p.IpAddress,
                            Owner = p.Owner,
                            BuildingNumber = p.BuildingNumber,
                            FloorNumber = p.FloorNumber
                        };

            if (ctx.AssetType == (int)AssetType.Infrastructure)
            {
                query = from p in query
                        join s in _stationRepository.Table on p.StationCode.Trim() equals s.Code into sp
                        from ps in sp.DefaultIfEmpty()
                        where p.StationCode != null                              
                        select new AssetSvcEntity
                        {
                            Id = p.Id,
                            Code = p.Code,
                            Name = p.Name,
                            OrganizationUnitId = p.OrganizationUnitId,
                            OrganizationUnitName = p.OrganizationUnitName,
                            EmployeeId = p.EmployeeId,
                            EmployeeName = p.EmployeeName,
                            ProjectCode = p.ProjectCode,
                            ProjectName = p.ProjectName,
                            CurrentUsageStatus = p.CurrentUsageStatus,
                            CustomerCode = p.CustomerCode,
                            CustomerName = p.CustomerName,
                            CategoryId = p.CategoryId,
                            AllocationDate = p.AllocationDate,
                            CreatedBy = p.CreatedBy,
                            CreatedDate = p.CreatedDate,
                            ModifiedBy = p.ModifiedBy,
                            ModifiedDate = p.ModifiedDate,
                            UnitId = p.UnitId,
                            UnitName = p.UnitName,
                            MaintenancedDate = p.MaintenancedDate,
                            Serial = p.Serial,
                            Description = p.Description,
                            ManufactureYear = p.ManufactureYear,
                            Status = p.Status,
                            Reference = p.Reference,
                            WareHouseItemCode = p.WareHouseItemCode,
                            WareHouseItemName = p.WareHouseItemName,
                            OriginQuantity = p.OriginQuantity,
                            RecallQuantity = p.RecallQuantity,
                            SoldQuantity = p.SoldQuantity,
                            BrokenQuantity = p.BrokenQuantity,
                            OriginUsageStatus = p.OriginUsageStatus,
                            OriginValue = p.OriginValue,
                            CurrentValue = p.CurrentValue,
                            DepreciationDuration = p.DepreciationDuration,
                            DepreciationUnit = p.DepreciationUnit,
                            WarrantyCondition = p.WarrantyCondition,
                            WarrantyDuration = p.WarrantyDuration,
                            WarrantyUnit = p.WarrantyUnit,
                            VendorName = p.VendorName,
                            Country = p.Country,
                            AssetType = p.AssetType,
                            StationCode = p.StationCode,
                            StationName = ps.Name,
                            StationArea = ps.Area,
                            StationAddress = ps.Address,
                            StationCategory = ps.Note,
                            StationLatitude = ps.Latitude ?? 0,
                            StationLongitude = ps.Longitude ?? 0,
                            ProjectAddress = p.ProjectAddress,
                            ProjectArea = p.ProjectArea,
                            ProjectCategory = p.ProjectCategory,
                            ProjectProvince = p.ProjectProvince,
                            IpAddress = p.IpAddress,
                            Owner = p.Owner,
                            BuildingNumber = p.BuildingNumber,
                            FloorNumber = p.FloorNumber
                        };
            }           

            if (ctx.Keywords != null && ctx.Keywords.HasValue())
            {
                query = query.LeftJoin(_localizedPropertyRepository.DataConnection.GetTable<LocalizedProperty>().DatabaseName(DbHelper.DbNames.Master),
                        (e, l) => e.Id == l.EntityId,
                        (e, l) => new { e, l })
                    .Where(
                        el =>
                             el.e.Code.Contains(ctx.Keywords) ||
                             el.e.Name.Contains(ctx.Keywords) ||
                            (el.l.LanguageId == ctx.LanguageId &&
                             el.l.LocaleKeyGroup == nameof(Core.Domain.Asset.Asset) &&
                             el.l.LocaleKey == nameof(Core.Domain.Asset.Asset.Name) &&
                             el.l.LocaleValue.Contains(ctx.Keywords)))
                    .Select(el => el.e).Distinct();
            }

            if (!string.IsNullOrEmpty(ctx.OrganizationId))
            {
                var department = _organizationRepository.Table.FirstOrDefault(x => x.Id.ToString() == ctx.OrganizationId);

                if (!string.IsNullOrEmpty(department.TreePath))
                {
                    var departmentIds = _organizationRepository.Table.Where(x => !string.IsNullOrEmpty(x.TreePath) &&
                                                                            x.TreePath.Contains(department.TreePath))
                                                                      .Select(x => x.Id.ToString());
                    if (departmentIds?.ToList().Count > 0)
                    {
                        var listDepartmentIds = departmentIds.ToList();
                        query = query.Where(x => listDepartmentIds.Contains(x.OrganizationUnitId));
                    }
                }               
            }

            if (!string.IsNullOrEmpty(ctx.EmployeeId))
            {
                query = from p in query
                        where p.EmployeeId == ctx.EmployeeId
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.CustomerCode))
            {
                query = from p in query
                        where p.CustomerCode == ctx.CustomerCode
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.ProjectCode))
            {
                query = from p in query
                        where p.ProjectCode == ctx.ProjectCode
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.StationCode))
            {
                query = from p in query
                        where p.StationCode == ctx.StationCode
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.CategoryId))
            {
                query = from p in query
                        where p.CategoryId == ctx.CategoryId
                        select p;
            }

            if (ctx.FromDate.HasValue)
            {
                query = from p in query
                        where p.CreatedDate >= ctx.FromDate.Value
                        select p;
            }

            if (ctx.ToDate.HasValue)
            {
                query = from p in query
                        where p.CreatedDate <= ctx.ToDate.Value
                        select p;
            }

            query = from p in query orderby p.Code select p;

            return new PagedList<AssetSvcEntity>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<SelectListItem> GetMvcListItems(int assetType)
        {
            var results = new List<SelectListItem>();

            var query = from p in _assetRepository.Table
                        where p.AssetType == assetType
                        select p;

            if (query?.ToList()?.Count > 0)
            {
                query.ToList().ForEach(x =>
                {
                    var m = new SelectListItem
                    {
                        Value = x.Id,
                        Text = $"[{x.Code}] {x.Name}"
                    };

                    results.Add(m);
                });
            }

            return results;
        }

        public IList<Core.Domain.Asset.Asset> GetListDropDownd()
        {
            var query = from p in _assetRepository.Table
                        select new Core.Domain.Asset.Asset
                        {
                            Id = p.Id,
                            Code = p.Code,
                            Name = p.Name
                        };

            return query.ToList();
        }

        public IList<Core.Domain.Asset.Asset> GetByIdsAsync(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var entities = _assetRepository.Table.Where(x => ids.Contains(x.Id));

            return entities.ToList();
        }

        public IPagedList<AssetSvcEntity> GetExcelData(AssetSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _assetRepository.Table
                        where p.AssetType == ctx.AssetType
                        select new AssetSvcEntity
                        {
                            Id = p.Id,
                            Code = p.Code,
                            Name = p.Name,
                            OrganizationUnitId = p.OrganizationUnitId,
                            OrganizationUnitName = p.OrganizationUnitName,
                            EmployeeId = p.EmployeeId,
                            EmployeeName = p.EmployeeName,
                            ProjectCode = p.ProjectCode,
                            ProjectName = p.ProjectName,
                            CurrentUsageStatus = p.CurrentUsageStatus,
                            CustomerCode = p.CustomerCode,
                            CustomerName = p.CustomerName,
                            CategoryId = p.CategoryId,
                            AllocationDate = p.AllocationDate,
                            CreatedBy = p.CreatedBy,
                            CreatedDate = p.CreatedDate,
                            ModifiedBy = p.ModifiedBy,
                            ModifiedDate = p.ModifiedDate,
                            UnitId = p.UnitId,
                            UnitName = p.UnitName,
                            MaintenancedDate = p.MaintenancedDate,
                            Serial = p.Serial,
                            Description = p.Description,
                            ManufactureYear = p.ManufactureYear,
                            Status = p.Status,
                            Reference = p.Reference,
                            WareHouseItemCode = p.WareHouseItemCode,
                            WareHouseItemName = p.WareHouseItemName,
                            OriginQuantity = p.OriginQuantity,
                            RecallQuantity = p.RecallQuantity,
                            SoldQuantity = p.SoldQuantity,
                            BrokenQuantity = p.BrokenQuantity,
                            OriginUsageStatus = p.OriginUsageStatus,
                            OriginValue = p.OriginValue,
                            CurrentValue = p.CurrentValue,
                            DepreciationDuration = p.DepreciationDuration,
                            DepreciationUnit = p.DepreciationUnit,
                            WarrantyCondition = p.WarrantyCondition,
                            WarrantyDuration = p.WarrantyDuration,
                            WarrantyUnit = p.WarrantyUnit,
                            VendorName = p.VendorName,
                            Country = p.Country,
                            AssetType = p.AssetType,
                            StationCode = p.StationCode,
                            StationName = p.StationName,
                            ProjectAddress = p.ProjectAddress,
                            ProjectArea = p.ProjectArea,
                            ProjectCategory = p.ProjectCategory,
                            ProjectProvince = p.ProjectProvince,
                            IpAddress = p.IpAddress,
                            Owner = p.Owner,
                            BuildingNumber = p.BuildingNumber,
                            FloorNumber = p.FloorNumber
                        };

            if (ctx.AssetType == (int)AssetType.Infrastructure)
            {
                query = from p in query
                        join s in _stationRepository.Table on p.StationCode.Trim() equals s.Code into sp
                        from ps in sp.DefaultIfEmpty()
                        where p.StationCode != null
                        select new AssetSvcEntity
                        {
                            Id = p.Id,
                            Code = p.Code,
                            Name = p.Name,
                            OrganizationUnitId = p.OrganizationUnitId,
                            OrganizationUnitName = p.OrganizationUnitName,
                            EmployeeId = p.EmployeeId,
                            EmployeeName = p.EmployeeName,
                            ProjectCode = p.ProjectCode,
                            ProjectName = p.ProjectName,
                            CurrentUsageStatus = p.CurrentUsageStatus,
                            CustomerCode = p.CustomerCode,
                            CustomerName = p.CustomerName,
                            CategoryId = p.CategoryId,
                            AllocationDate = p.AllocationDate,
                            CreatedBy = p.CreatedBy,
                            CreatedDate = p.CreatedDate,
                            ModifiedBy = p.ModifiedBy,
                            ModifiedDate = p.ModifiedDate,
                            UnitId = p.UnitId,
                            UnitName = p.UnitName,
                            MaintenancedDate = p.MaintenancedDate,
                            Serial = p.Serial,
                            Description = p.Description,
                            ManufactureYear = p.ManufactureYear,
                            Status = p.Status,
                            Reference = p.Reference,
                            WareHouseItemCode = p.WareHouseItemCode,
                            WareHouseItemName = p.WareHouseItemName,
                            OriginQuantity = p.OriginQuantity,
                            RecallQuantity = p.RecallQuantity,
                            SoldQuantity = p.SoldQuantity,
                            BrokenQuantity = p.BrokenQuantity,
                            OriginUsageStatus = p.OriginUsageStatus,
                            OriginValue = p.OriginValue,
                            CurrentValue = p.CurrentValue,
                            DepreciationDuration = p.DepreciationDuration,
                            DepreciationUnit = p.DepreciationUnit,
                            WarrantyCondition = p.WarrantyCondition,
                            WarrantyDuration = p.WarrantyDuration,
                            WarrantyUnit = p.WarrantyUnit,
                            VendorName = p.VendorName,
                            Country = p.Country,
                            AssetType = p.AssetType,
                            StationCode = p.StationCode,
                            StationName = ps.Name,
                            StationArea = ps.Area,
                            StationAddress = ps.Address,
                            StationCategory = ps.Note,
                            StationLatitude = ps.Latitude ?? 0,
                            StationLongitude = ps.Longitude ?? 0,
                            ProjectAddress = p.ProjectAddress,
                            ProjectArea = p.ProjectArea,
                            ProjectCategory = p.ProjectCategory,
                            ProjectProvince = p.ProjectProvince,
                            IpAddress = p.IpAddress,
                            Owner = p.Owner,
                            BuildingNumber = p.BuildingNumber,
                            FloorNumber = p.FloorNumber
                        };
            }

            if (ctx.Keywords != null && ctx.Keywords.HasValue())
            {
                query = query.LeftJoin(_localizedPropertyRepository.DataConnection.GetTable<LocalizedProperty>().DatabaseName(DbHelper.DbNames.Master),
                        (e, l) => e.Id == l.EntityId,
                        (e, l) => new { e, l })
                    .Where(
                        el =>
                             el.e.Code.Contains(ctx.Keywords) ||
                             el.e.Name.Contains(ctx.Keywords) ||
                            (el.l.LanguageId == ctx.LanguageId &&
                             el.l.LocaleKeyGroup == nameof(Core.Domain.Asset.Asset) &&
                             el.l.LocaleKey == nameof(Core.Domain.Asset.Asset.Name) &&
                             el.l.LocaleValue.Contains(ctx.Keywords)))
                    .Select(el => el.e).Distinct();
            }

            if (!string.IsNullOrEmpty(ctx.OrganizationId))
            {
                var department = _organizationRepository.Table.FirstOrDefault(x => x.Id.ToString() == ctx.OrganizationId);

                if (!string.IsNullOrEmpty(department.TreePath))
                {
                    var departmentIds = _organizationRepository.Table.Where(x => !string.IsNullOrEmpty(x.TreePath) &&
                                                                            x.TreePath.Contains(department.TreePath))
                                                                      .Select(x => x.Id.ToString());
                    if (departmentIds?.ToList().Count > 0)
                    {
                        var listDepartmentIds = departmentIds.ToList();
                        query = query.Where(x => listDepartmentIds.Contains(x.OrganizationUnitId));
                    }
                }
            }

            if (!string.IsNullOrEmpty(ctx.EmployeeId))
            {
                query = from p in query
                        where p.EmployeeId == ctx.EmployeeId
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.CustomerCode))
            {
                query = from p in query
                        where p.CustomerCode == ctx.CustomerCode
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.ProjectCode))
            {
                query = from p in query
                        where p.ProjectCode == ctx.ProjectCode
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.StationCode))
            {
                query = from p in query
                        where p.StationCode == ctx.StationCode
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.CategoryId))
            {
                query = from p in query
                        where p.CategoryId == ctx.CategoryId
                        select p;
            }

            if (ctx.FromDate.HasValue)
            {
                query = from p in query
                        where p.CreatedDate >= ctx.FromDate.Value
                        select p;
            }

            if (ctx.ToDate.HasValue)
            {
                query = from p in query
                        where p.CreatedDate <= ctx.ToDate.Value
                        select p;
            }

            query = from p in query orderby p.Code select p;

            return new PagedList<AssetSvcEntity>(query, ctx.PageIndex, ctx.PageSize);
        }
        #endregion

        #region Utilities

        #endregion
    }
}

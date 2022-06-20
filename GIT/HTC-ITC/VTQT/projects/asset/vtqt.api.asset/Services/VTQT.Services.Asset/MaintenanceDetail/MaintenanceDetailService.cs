using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;
using VTQT.Core.Domain.FbmOrganization;

namespace VTQT.Services.Asset
{
    public partial class MaintenanceDetailService : IMaintenanceDetailService
    {
        #region Fields
        private readonly IRepository<MaintenanceDetail> _maintenanceDetailRepository;
        private readonly IRepository<Core.Domain.Asset.Asset> _assetRepository;
        private readonly IIntRepository<OrganizationUnit> _organizationRepository;
        private readonly IRepository<AssetAttachment> _assetAttachmentRepository;
        #endregion

        #region Ctor
        public MaintenanceDetailService()
        {
            _maintenanceDetailRepository = EngineContext.Current.Resolve<IRepository<MaintenanceDetail>>(DataConnectionHelper.ConnectionStringNames.Asset);
            _assetRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Asset.Asset>>(DataConnectionHelper.ConnectionStringNames.Asset);
            _organizationRepository = EngineContext.Current.Resolve<IIntRepository<OrganizationUnit>>(DataConnectionHelper.ConnectionStringNames.FbmOrganization);
            _assetAttachmentRepository = EngineContext.Current.Resolve<IRepository<AssetAttachment>>(DataConnectionHelper.ConnectionStringNames.Asset);
        }
        #endregion

        #region Methods
        public async Task<long> InsertAsync(List<MaintenanceDetail> entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _maintenanceDetailRepository.InsertAsync(entity);

            return result;
        }
        #endregion

        #region List
        public IPagedList<Core.Domain.Asset.Asset> Get(MaintenanceDetailSearchContext ctx)
        {
            ctx.Keywords = ctx?.Keywords?.Trim();

            var query = from p in _assetRepository.Table
                        where p.AssetType == ctx.AssetType
                        select p;

            if (ctx.Keywords.HasValue())
            {
                query = from p in query
                        where p.Name.Contains(ctx.Keywords) ||
                              p.Code.Contains(ctx.Keywords)
                        select p;
            }

            if (ctx.AssetCategoryId.HasValue())
            {
                query = from p in query
                        where p.CategoryId == ctx.AssetCategoryId
                        select p;
            }

            if (ctx.OrganizationId.HasValue())
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

            if (ctx.StationCode.HasValue())
            {
                query = from p in query
                        where p.StationCode == ctx.StationCode
                        select p;
            }

            if (ctx.ProjectCode.HasValue())
            {
                query = from p in query
                        where p.ProjectCode == ctx.ProjectCode
                        select p;
            }

            query = from p in query orderby p.Code select p;

            return new PagedList<Core.Domain.Asset.Asset>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IPagedList<AssetAttachment> GetAssetAttachments(MaintenanceDetailSearchContext ctx)
        {
            ctx.Keywords = ctx?.Keywords?.Trim();

            var query = from p in _assetAttachmentRepository.Table
                        where p.AssetType == ctx.AssetType
                        select p;

            if (ctx.Keywords.HasValue())
            {
                query = from p in query
                        where p.Name.Contains(ctx.Keywords) ||
                              p.Code.Contains(ctx.Keywords)
                        select p;
            }

            if (ctx.AssetCategoryId.HasValue())
            {
                query = from p in query
                        where p.CategoryId == ctx.AssetCategoryId
                        select p;
            }

            if (ctx.OrganizationId.HasValue())
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

            if (ctx.StationCode.HasValue())
            {
                query = from p in query
                        where p.StationCode == ctx.StationCode
                        select p;
            }

            if (ctx.ProjectCode.HasValue())
            {
                query = from p in query
                        where p.ProjectCode == ctx.ProjectCode
                        select p;
            }

            query = from p in query orderby p.Code select p;

            return new PagedList<AssetAttachment>(query, ctx.PageIndex, ctx.PageSize);
        }
        #endregion
    }
}

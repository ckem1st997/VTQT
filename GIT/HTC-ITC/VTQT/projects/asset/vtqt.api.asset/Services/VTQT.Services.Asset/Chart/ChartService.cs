using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Text;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using LinqToDB;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using VTQT.Core;
using VTQT.Core.Data;
using VTQT.Core.Domain.FbmOrganization;

namespace VTQT.Services.Asset
{
    public partial class ChartService : IChartService
    {

        private readonly IRepository<History> _historyRepository;
        private readonly IRepository<Core.Domain.Asset.Asset> _assetRepository;
        private readonly IIntRepository<OrganizationUnit> _organizationRepository;

        #region Ctor
        public ChartService()
        {
            _historyRepository = EngineContext.Current.Resolve<IRepository<History>>(DataConnectionHelper.ConnectionStringNames.Asset);
            _assetRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Asset.Asset>>(DataConnectionHelper.ConnectionStringNames.Asset);
            _organizationRepository = EngineContext.Current.Resolve<IIntRepository<OrganizationUnit>>(DataConnectionHelper.ConnectionStringNames.FbmOrganization);

        }

        public IEnumerable<Core.Domain.Asset.Asset> GetChartPie(int AssetType, string OrganizationId)
        {
            if (AssetType <= 0)
                throw new NotImplementedException(nameof(AssetType));


            var list = new StringBuilder();
            list.Append(" ( ");
            var department = _organizationRepository.Table.FirstOrDefault(x => x.Id.ToString() == OrganizationId);

            if (department !=null && !string.IsNullOrEmpty(department.TreePath))
            {
                var departmentIds = _organizationRepository.Table.Where(x => !string.IsNullOrEmpty(x.TreePath) &&
                                                                        x.TreePath.Contains(department.TreePath))
                                                                  .Select(x => x.Id.ToString());
                if (departmentIds?.ToList().Count > 0)
                {
                    var listDepartmentIds = departmentIds.ToList();
                    for (int i = 0; i < listDepartmentIds.Count; i++)
                    {
                        if (i == listDepartmentIds.Count - 1)
                            list.Append(listDepartmentIds[i]);
                        else
                            list.Append(listDepartmentIds[i] + ", ");

                    }
                }
            }
            list.Append(" ) ");

            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT us.Description, SUM(a.OriginQuantity-a.RecallQuantity-a.SoldQuantity-a.BrokenQuantity) AS OriginQuantity FROM Asset a INNER JOIN UsageStatus us ON a.CurrentUsageStatus =us.Id ");
            sb.Append("WHERE a.AssetType=@p1 ");
            if (department !=null &&!string.IsNullOrEmpty(department.TreePath))
            {
                sb.Append(" AND a.OrganizationUnitId in ");
                sb.Append(list);
            }

            sb.Append("GROUP BY us.Description ");
            

            DataParameter p1 = new DataParameter("p1", AssetType);
            DataParameter p2 = new DataParameter("p2", OrganizationId);

            return _historyRepository.DataConnection.Query<Core.Domain.Asset.Asset>(sb.ToString(), p1, p2);

        }


        public IEnumerable<Core.Domain.Asset.Asset> GetChartCoulunm(int AssetType, string OrganizationId)
        {
            if (AssetType <= 0)
                throw new NotImplementedException(nameof(AssetType));
            // BuildMyString.com generated code. Please enjoy your string responsibly.
            var list = new StringBuilder();
            list.Append(" ( ");
            var department = _organizationRepository.Table.FirstOrDefault(x => x.Id.ToString() == OrganizationId);

            if (department != null &&!string.IsNullOrEmpty(department.TreePath))
            {
                var departmentIds = _organizationRepository.Table.Where(x => !string.IsNullOrEmpty(x.TreePath) &&
                                                                        x.TreePath.Contains(department.TreePath))
                                                                  .Select(x => x.Id.ToString());
                if (departmentIds?.ToList().Count > 0)
                {
                    var listDepartmentIds = departmentIds.ToList();
                    for (int i = 0; i < listDepartmentIds.Count; i++)
                    {
                        if (i == listDepartmentIds.Count - 1)
                            list.Append(listDepartmentIds[i]);
                        else
                            list.Append(listDepartmentIds[i] + ", ");

                    }
                }
            }
            list.Append(" ) ");
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("  ac.Name, ");
            sb.Append("  SUM(a.OriginQuantity-a.RecallQuantity-a.SoldQuantity-a.BrokenQuantity) as  OriginQuantity ");
            sb.Append("FROM Asset a ");
            sb.Append("  INNER JOIN UsageStatus us ");
            sb.Append("    ON a.CurrentUsageStatus = us.Id ");
            sb.Append("  INNER JOIN AssetCategory ac ");
            sb.Append("    ON a.CategoryId = ac.Id ");
            sb.Append("WHERE a.AssetType = @p1 ");
            if (department != null && !string.IsNullOrEmpty(department.TreePath))
            {
                sb.Append(" AND a.OrganizationUnitId in ");
                sb.Append(list);
            }
            sb.Append("GROUP BY a.CategoryId ");
            sb.Append("ORDER BY OriginQuantity ");



            DataParameter p1 = new DataParameter("p1", AssetType);
            DataParameter p2 = new DataParameter("p2", OrganizationId);

            return _historyRepository.DataConnection.Query<Core.Domain.Asset.Asset>(sb.ToString(), p1,p2);

        }


        public async Task<IPagedList<Core.Domain.Asset.Asset>> GetWarrantyDuration(ChartSearchContext model)
        {
            if (model is null)
                throw new NotImplementedException(nameof(model));
            int number;
            //  var result = "SELECT a.Name,a.OrganizationUnitName,(CASE WHEN a.WarrantyUnit = 1 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration DAY) WHEN a.WarrantyUnit = 2 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration MONTH) ELSE DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration year) END) AS CreatedBy FROM Asset a WHERE a.AssetType = @p1 and CASE WHEN a.WarrantyUnit = 1 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration DAY) >= CURRENT_TIMESTAMP() WHEN a.WarrantyUnit = 2 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration MONTH) >= CURRENT_TIMESTAMP() ELSE DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration year) >= CURRENT_TIMESTAMP() END  LIMIT @p2 OFFSET @p3";
         
            
            
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("  a.Code, ");
            sb.Append("  a.Name, ");
            sb.Append("  ac.Name AS CategoryId, ");
            sb.Append("  a.OrganizationUnitName, ");
            sb.Append("  (CASE WHEN a.WarrantyUnit = 1 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration DAY) ");
            sb.Append("  WHEN a.WarrantyUnit = 2 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration MONTH) ");
            sb.Append("  ELSE DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration year) END) AS CreatedBy, ");
            sb.Append("  a.OriginQuantity - a.RecallQuantity - a.SoldQuantity - a.BrokenQuantity AS OriginQuantity ");
            sb.Append("FROM Asset a ");
            sb.Append("INNER JOIN AssetCategory ac ON a.CategoryId = ac.Id ");
            sb.Append("WHERE a.AssetType = @p1 ");
            sb.Append("AND (CASE WHEN a.WarrantyUnit = 1 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration DAY) ");
            sb.Append("WHEN a.WarrantyUnit = 2 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration MONTH) ");
            sb.Append("ELSE DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration year) END) <= DATE_ADD(CURRENT_TIMESTAMP(), INTERVAL @p4 DAY) ");
            sb.Append("ORDER BY CreatedBy DESC ");
            sb.Append("LIMIT @p2 OFFSET @p3 ");



            //  var count = "SELECT COUNT(*) FROM (  SELECT a.Name,a.OrganizationUnitName,(CASE WHEN a.WarrantyUnit = 1 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration DAY) WHEN a.WarrantyUnit = 2 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration MONTH) ELSE DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration year) END) AS CreatedBy FROM Asset a WHERE a.AssetType = @p1 and CASE WHEN a.WarrantyUnit = 1 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration DAY) >= CURRENT_TIMESTAMP() WHEN a.WarrantyUnit = 2 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration MONTH) >= CURRENT_TIMESTAMP() ELSE DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration year) >= CURRENT_TIMESTAMP() END         ) t";
            StringBuilder sbCount = new StringBuilder();

            sbCount.Append("SELECT COUNT(*) FROM ( SELECT ");
            sbCount.Append("  a.Code, ");
            sbCount.Append("  a.Name, ");
            sbCount.Append("  ac.Name AS CategoryId, ");
            sbCount.Append("  a.OrganizationUnitName, ");
            sbCount.Append("  (CASE WHEN a.WarrantyUnit = 1 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration DAY)  ");
            sbCount.Append("  WHEN a.WarrantyUnit = 2 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration MONTH)  ");
            sbCount.Append("  ELSE DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration year) END) AS CreatedBy, ");
            sbCount.Append("  a.OriginQuantity - a.RecallQuantity - a.SoldQuantity - a.BrokenQuantity AS OriginQuantity ");
            sbCount.Append("FROM Asset a ");
            sbCount.Append("INNER JOIN AssetCategory ac ON a.CategoryId = ac.Id ");
            sbCount.Append("WHERE a.AssetType = @p1 ");
            sbCount.Append("AND (CASE WHEN a.WarrantyUnit = 1 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration DAY)  ");
            sbCount.Append("WHEN a.WarrantyUnit = 2 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration MONTH)  ");
            sbCount.Append("ELSE DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration year) END) <= DATE_ADD(CURRENT_TIMESTAMP(), INTERVAL @p4 DAY)) d ");
            if (int.TryParse(model.Keywords, out number))
            {
                DataParameter p1 = new DataParameter("p1", int.Parse(model.Keywords));
                DataParameter p2 = new DataParameter("p2", model.PageSize);
                DataParameter p3 = new DataParameter("p3", model.PageIndex * model.PageSize);
                DataParameter p4 = new DataParameter("p4", model.Date);

                var res = await _assetRepository.DataConnection.QueryToListAsync<Core.Domain.Asset.Asset>(sb.ToString(), p1, p2, p3,p4);
                var resCount = await _assetRepository.DataConnection.QueryToListAsync<int>(sbCount.ToString(), p1,p4);
                return new PagedList<Core.Domain.Asset.Asset>(res, model.PageIndex, model.PageSize, resCount.FirstOrDefault());
            }
            return new PagedList<Core.Domain.Asset.Asset>(new List<Core.Domain.Asset.Asset>(), model.PageIndex, model.PageSize);


        }


        public async Task<IPagedList<Core.Domain.Asset.Asset>> GetProjectBase(ChartSearchContext model)
        {
            if (model is null)
                throw new NotImplementedException(nameof(model));
            int number;
            // var result = "SELECT a.OrganizationUnitName, SUM(a.OriginQuantity) AS OriginQuantity, SUM(a.BrokenQuantity) AS BrokenQuantity, COUNT(a.Id) AS Status FROM Asset a WHERE a.AssetType = @p1 AND CASE WHEN a.WarrantyUnit = 1 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration DAY) >= CURRENT_TIMESTAMP() WHEN a.WarrantyUnit = 2 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration MONTH) >= CURRENT_TIMESTAMP() ELSE DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration year) >= CURRENT_TIMESTAMP() END GROUP BY a.OrganizationUnitName  LIMIT @p2 OFFSET @p3";
            //  var count = "SELECT COUNT(*) FROM (  SELECT a.OrganizationUnitName, SUM(a.OriginQuantity) AS OriginQuantity, SUM(a.BrokenQuantity) AS BrokenQuantity, COUNT(a.Id) AS Status FROM Asset a WHERE a.AssetType = @p1 AND CASE WHEN a.WarrantyUnit = 1 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration DAY) >= CURRENT_TIMESTAMP() WHEN a.WarrantyUnit = 2 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration MONTH) >= CURRENT_TIMESTAMP() ELSE DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration year) >= CURRENT_TIMESTAMP() END GROUP BY a.OrganizationUnitName         ) t";
          
            
            var list = new StringBuilder();
            list.Append(" ( ");
            var department = _organizationRepository.Table.FirstOrDefault(x => x.Id.ToString() == model.OrganizationId);

            if (department != null &&!string.IsNullOrEmpty(department.TreePath))
            {
                var departmentIds = _organizationRepository.Table.Where(x => !string.IsNullOrEmpty(x.TreePath) &&
                                                                             x.TreePath.Contains(department.TreePath))
                    .Select(x => x.Id.ToString());
                if (departmentIds?.ToList().Count > 0)
                {
                    var listDepartmentIds = departmentIds.ToList();
                    for (int i = 0; i < listDepartmentIds.Count; i++)
                    {
                        if (i == listDepartmentIds.Count - 1)
                            list.Append(listDepartmentIds[i]);
                        else
                            list.Append(listDepartmentIds[i] + ", ");

                    }
                }
            }
            list.Append(" ) ");
            
            
            
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("  a.OrganizationUnitName, ");
            sb.Append("  SUM(a.OriginQuantity) AS OriginQuantity, ");
            sb.Append("  SUM(a.BrokenQuantity) AS BrokenQuantity, ");
            sb.Append("  SUM(a.RecallQuantity) AS RecallQuantity, ");
            sb.Append("  SUM(a.SoldQuantity) AS SoldQuantity, ");
            sb.Append("  SUM(a.OriginQuantity-a.RecallQuantity-a.BrokenQuantity-a.SoldQuantity) AS Status ");
            sb.Append("FROM Asset a ");
            sb.Append("WHERE a.AssetType =@p1 ");
            sb.Append("AND (CASE WHEN a.WarrantyUnit = 1 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration DAY)  ");
            sb.Append(" WHEN a.WarrantyUnit = 2 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration MONTH)  ");
            sb.Append(" ELSE DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration year) END) <= DATE_ADD(CURRENT_TIMESTAMP(), INTERVAL @p4 DAY) ");
            if (department != null && !string.IsNullOrEmpty(department.TreePath))
            {
                sb.Append(" AND a.OrganizationUnitId in ");
                sb.Append(list);
            }
            sb.Append("GROUP BY a.OrganizationUnitName  ");
            sb.Append("LIMIT @p2 OFFSET @p3 ");



            StringBuilder sbCount = new StringBuilder();

            sbCount.Append(" SELECT COUNT(*) FROM( SELECT ");
            sbCount.Append("  a.OrganizationUnitName, ");
            sbCount.Append("  SUM(a.OriginQuantity) AS OriginQuantity, ");
            sbCount.Append("  SUM(a.BrokenQuantity) AS BrokenQuantity, ");
            sbCount.Append("  COUNT(a.Id) AS Status ");
            sbCount.Append("FROM Asset a ");
            sbCount.Append("WHERE a.AssetType =@p1 ");
            sbCount.Append("AND (CASE WHEN a.WarrantyUnit = 1 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration DAY)  ");
            sbCount.Append(" WHEN a.WarrantyUnit = 2 THEN DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration MONTH)  ");
            sbCount.Append(" ELSE DATE_ADD(a.AllocationDate, INTERVAL a.WarrantyDuration year) END) <= DATE_ADD(CURRENT_TIMESTAMP(), INTERVAL @p4 DAY) ");
            if (department != null && !string.IsNullOrEmpty(department.TreePath))
            {
                sbCount.Append(" AND a.OrganizationUnitId in ");
                sbCount.Append(list);
            }
            sbCount.Append(" GROUP BY a.OrganizationUnitName ");
            sbCount.Append(" )d ");
            if (int.TryParse(model.Keywords, out number))
            {
                DataParameter p1 = new DataParameter("p1", int.Parse(model.Keywords));
                DataParameter p2 = new DataParameter("p2", model.PageSize);
                DataParameter p3 = new DataParameter("p3", model.PageIndex * model.PageSize);
                DataParameter p4 = new DataParameter("p4",0);

                var res = await _assetRepository.DataConnection.QueryToListAsync<Core.Domain.Asset.Asset>(sb.ToString(), p1, p2, p3,p4);
                var resCount = await _assetRepository.DataConnection.QueryToListAsync<int>(sbCount.ToString(), p1,p4);

                return new PagedList<Core.Domain.Asset.Asset>(res, model.PageIndex, model.PageSize, resCount.FirstOrDefault());
            }
            return new PagedList<Core.Domain.Asset.Asset>(new List<Core.Domain.Asset.Asset>(), model.PageIndex, model.PageSize);


        }
        #endregion


        #region List




        #endregion
    }
}

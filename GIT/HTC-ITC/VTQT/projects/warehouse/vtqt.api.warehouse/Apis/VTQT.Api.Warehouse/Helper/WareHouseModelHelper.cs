using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using VTQT.Services.Warehouse;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.SharedMvc.Warehouse.Models.WareHouse;

namespace VTQT.Api.Warehouse.Helper
{
    public class WareHouseModelHelper : IWareHouseModelHelper
    {
        private readonly IWareHouseService _organizationalUnitService;
        private readonly IRepository<BeginningWareHouse> _beginningWareHouseRepository;
        private readonly IWorkContext _workContext;



        // private readonly ICommonServices _services;
        private readonly IXBaseCacheManager _cacheManager;

        public WareHouseModelHelper(
            IXBaseCacheManager cacheManager,
            IWareHouseService organizationalUnitService,
            IWorkContext workContext)
        {
            _workContext = workContext;
            _organizationalUnitService = organizationalUnitService;
            // _services = services;
            _cacheManager = cacheManager;
            _beginningWareHouseRepository =
                EngineContext.Current.Resolve<IRepository<BeginningWareHouse>>(DataConnectionHelper
                    .ConnectionStringNames.Warehouse);
        }

        public async Task<int> CheckUIQuantity(string WareHouseId, string ItemId)
        {
            if (string.IsNullOrEmpty(WareHouseId) && string.IsNullOrEmpty(ItemId))
                throw new NotImplementedException("Not Id to Search Table");
            // BuildMyString.com generated code. Please enjoy your string responsibly.
            var date = DateTime.UtcNow;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("  SUM(d1.Quantity) AS Amount ");
            sb.Append("FROM (SELECT ");
            sb.Append("    bwh.ItemId, ");
            sb.Append("    bwh.WarehouseId, ");
            sb.Append("    bwh.Quantity ");
            sb.Append("  FROM BeginningWareHouse bwh ");
            sb.Append("  WHERE bwh.ItemId = @p1 ");
            sb.Append("  UNION ALL ");
            sb.Append("  SELECT ");
            sb.Append("    id.ItemId, ");
            sb.Append("    i.WarehouseId, ");
            sb.Append("    id.Quantity ");
            sb.Append("  FROM Inward i ");
            sb.Append("    INNER JOIN InwardDetail id ");
            sb.Append("      ON i.id = id.InwardId ");
            sb.Append("  WHERE id.ItemId = @p1 ");
            sb.Append("  AND i.VoucherDate <= '" + ConvertDateTimeToDateTimeSql(date) + " 12:0:0 AM' ");
            sb.Append("  UNION ALL ");
            sb.Append("  SELECT ");
            sb.Append("    od.ItemId, ");
            sb.Append("    o.WarehouseId, ");
            sb.Append("    -od.Quantity ");
            sb.Append("  FROM Outward o ");
            sb.Append("    INNER JOIN OutwardDetail od ");
            sb.Append("      ON o.Id = od.OutwardId ");
            sb.Append("  WHERE od.ItemId =@p1 ");
            sb.Append("  AND o.VoucherDate <= '" + ConvertDateTimeToDateTimeSql(date) + " 12:0:0 AM') d1 ");
            sb.Append("  INNER JOIN WareHouse wh ");
            sb.Append("    ON d1.WarehouseId = wh.Id ");
            sb.Append("    WHERE d1.WareHouseId=@p2 ");
            sb.Append("GROUP BY d1.ItemId, ");
            sb.Append("         d1.WarehouseId  ");
            DataParameter p1 = new DataParameter("p1", ItemId);
            DataParameter p2 = new DataParameter("p2", WareHouseId);
            var res = await _beginningWareHouseRepository.DataConnection.QueryToListAsync<int>(sb.ToString(), p1, p2);
            return res.FirstOrDefault();
        }

        public virtual async Task<IList<WareHouseTreeModel>> GetWareHouseTree(int? expandLevel, bool showHidden = false)
        {
            expandLevel = expandLevel ?? 1;
            var qq = new Queue<WareHouseTreeModel>();
            var lstCheck = new List<WareHouseTreeModel>();
            var result = new List<WareHouseTreeModel>();
            var list = await GetOrganizationalUnitsAsync(showHidden);
            var organizationalUnitModels =list
                .Select(s => new WareHouseTreeModel
                {
                    children = new List<WareHouseTreeModel>(),
                    folder = false,
                    key = s.Id,
                    title = s.Name,
                    tooltip = s.Name,
                    Path = s.Path,
                    ParentId = s.ParentId,
                    Code = s.Code,
                    Name = s.Name
                });
            var roots = organizationalUnitModels
                .Where(w => !w.ParentId.HasValue())
                .OrderBy(o => o.Name);

            foreach (var root in roots)
            {
                root.level = 1;
                root.expanded = !expandLevel.HasValue || root.level <= expandLevel.Value;
                root.folder = true;
                qq.Enqueue(root);
                lstCheck.Add(root);
                result.Add(root);
            }

            while (qq.Any())
            {
                var cur = qq.Dequeue();
                if (lstCheck.All(a => a.key != cur.key))
                    result.Add(cur);

                var childs = organizationalUnitModels
                    .Where(w => w.ParentId.HasValue() && w.ParentId.ToString() == cur.key)
                    .OrderBy(o => o.Name);

                if (!childs.Any())
                    continue;

                var childLevel = cur.level + 1;
                foreach (var child in childs)
                {
                    if (lstCheck.Any(a => a.key == child.key))
                        continue;

                    child.level = childLevel;
                    child.expanded = !expandLevel.HasValue || child.level <= expandLevel.Value;

                    qq.Enqueue(child);
                    lstCheck.Add(child);
                    cur.children.Add(child);
                }
            }

            return result;
        }

        #region Helpers

        private async Task<IList<WareHouseModel>> GetOrganizationalUnitsAsync(bool showHidden = false)
        {
            var cacheKey = WarehouseCacheKeys.Warehouses.AllCacheKey.FormatWith(showHidden);
            var models = _cacheManager.GetDb(cacheKey, () =>
            {
                var result = _organizationalUnitService.GetAll(showHidden);
                return result.ToList();
            });

            // get tree to user

            // BuildMyString.com generated code. Please enjoy your string responsibly.

            StringBuilder sb = new StringBuilder();

            sb.Append("WITH RECURSIVE cte (Id, Name, ParentId) ");
            sb.Append("AS ");
            sb.Append("(SELECT ");
            sb.Append("      wh.Id, ");
            sb.Append("      wh.Name, ");
            sb.Append("      wh.ParentId ");
            sb.Append("    FROM WareHouse wh ");
            sb.Append("    INNER JOIN WareHouseUser whu ");
            sb.Append("    ON wh.Id = whu.WarehouseId ");
            sb.Append("    WHERE whu.UserId='"+_workContext.UserId+"' ");
            sb.Append("  UNION ALL ");
            sb.Append("  SELECT ");
            sb.Append("    p.Id, ");
            sb.Append("    p.Name, ");
            sb.Append("    p.ParentId ");
            sb.Append("  FROM WareHouse p ");
            sb.Append("    INNER JOIN cte ");
            sb.Append("      ON p.ParentId = cte.Id) ");
            sb.Append("SELECT ");
            sb.Append("  cte.Id ");
            sb.Append("FROM cte ");
            sb.Append("GROUP BY cte.Id, ");
            sb.Append("         cte.Name, ");
            sb.Append("         cte.ParentId; ");

            var departmentIds =await _beginningWareHouseRepository.DataConnection.QueryToListAsync<string>(sb.ToString());
            if (departmentIds?.ToList().Count > 0)
            {
                var listDepartmentIds = departmentIds.ToList();
                models = models.Where(x => listDepartmentIds.Contains(x.Id)).ToList();
                var res = new List<WareHouseModel>();
                if (models?.Count > 0)
                {
                    models.ForEach(x =>
                    {
                        if (x.Inactive.Equals(showHidden))
                        {
                            var m = new WareHouseModel
                            {
                                Id = x.Id,
                                Code = x.Code,
                                Name = x.Name,
                                ParentId = x.ParentId,
                                Path = x.Path
                            };
                            res.Add(m);
                        }
                    });
                }

                return res;
            }

            return new List<WareHouseModel>();

        }

        public static string ConvertDateTimeToDateTimeSql(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return "";
            return "" + dateTime.Value.Year + "-" + dateTime.Value.Month + "-" + dateTime.Value.Day + "";
        }

        #endregion Helpers


        #region GetSelectItemTree

        public virtual async Task<IList<WareHouseModel>> GetWareHouseDropdownTreeAsync(bool showHidden = false, bool showList = false)
        {
            var models =await GetWareHousesAsync(showHidden,showList);
            return GetWareHouseTreeModel(models);
        }

        private async Task<IList<WareHouseModel>> GetWareHousesAsync(bool showHidden = false, bool showList = false)
        {
            // var cacheKey =
            //     ModelCacheEventConsumer.AvailableOrganizationalUnitsModelKey.FormatWith(
            //         _services.WorkContext.WorkingLanguage.Id, showHidden);
            // var models = _services.Cache.Get(cacheKey, () =>
            // {
            var result = _organizationalUnitService.GetAll()
                .Select(s => new WareHouseModel
                {
                    Id = s.Id,
                    ParentId = s.ParentId,
                    Code = s.Code,
                    Name = s.Name
                })
                .OrderBy(o => o.Name)
                .ToList();
            if(!showList)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("WITH RECURSIVE cte (Id, Name, ParentId) ");
                sb.Append("AS ");
                sb.Append("(SELECT ");
                sb.Append("      wh.Id, ");
                sb.Append("      wh.Name, ");
                sb.Append("      wh.ParentId ");
                sb.Append("    FROM WareHouse wh ");
                sb.Append("    INNER JOIN WareHouseUser whu ");
                sb.Append("    ON wh.Id = whu.WarehouseId ");
                sb.Append("    WHERE whu.UserId='" + _workContext.UserId + "' ");
                sb.Append("  UNION ALL ");
                sb.Append("  SELECT ");
                sb.Append("    p.Id, ");
                sb.Append("    p.Name, ");
                sb.Append("    p.ParentId ");
                sb.Append("  FROM WareHouse p ");
                sb.Append("    INNER JOIN cte ");
                sb.Append("      ON p.ParentId = cte.Id) ");
                sb.Append("SELECT ");
                sb.Append("  cte.Id ");
                sb.Append("FROM cte ");
                sb.Append("GROUP BY cte.Id, ");
                sb.Append("         cte.Name, ");
                sb.Append("         cte.ParentId; ");

                var departmentIds = await _beginningWareHouseRepository.DataConnection.QueryToListAsync<string>(sb.ToString());
                if (departmentIds?.ToList().Count > 0)
                {
                    var listDepartmentIds = departmentIds.ToList();
                    result = result.Where(x => listDepartmentIds.Contains(x.Id)).ToList();
                }

            }

            //
            //     return result;
            // });
            return result;
            // return models;
        }

        private List<WareHouseModel> GetWareHouseTreeModel(IEnumerable<WareHouseModel> models)
        {
            var parents = models
                .Where(w => !w.ParentId.HasValue())
                .OrderBy(o => o.Name);

            var result = new List<WareHouseModel>();
            var level = 0;
            foreach (var parent in parents)
            {
                result.Add(new WareHouseModel
                {
                    Id = parent.Id,
                    ParentId = parent.ParentId,
                    Name = "["+parent.Code+"] "+parent.Name,
                    Code = parent.Code
                });
                GetChildWareHouseTreeModel(ref models, parent.Id, ref result, level);
            }

            return result;
        }

        private void GetChildWareHouseTreeModel(ref IEnumerable<WareHouseModel> models, string parentId,
            ref List<WareHouseModel> result, int level)
        {
            level++;
            var childs = models
                .Where(w => w.ParentId == parentId)
                .OrderBy(o => o.Name);

            if (childs.Any())
            {
                foreach (var child in childs)
                {
                    child.Name = "[" + child.Code + "] " + child.Name;
                    result.Add(new WareHouseModel()
                    {
                        Id = child.Id,
                        ParentId = child.ParentId,
                        Name = level.ToTreeLevelString() + child.Name,
                        Code = child.Code
                    });
                    GetChildWareHouseTreeModel(ref models, child.Id, ref result, level);
                }
            }
        }

        public static string GetTreeLevelString(int level)
        {
            if (level <= 0)
                return "";

            var result = "";
            for (var i = 1; i <= level; i++)
            {
                result += "– ";
            }

            return result;
        }

        #endregion
    }
}
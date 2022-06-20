using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using VTQT.Core;
using VTQT.Core.Domain.Dashboard;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using VTQT.Services.Dashboard;
using VTQT.SharedMvc.Dashboard.Models;
using LinqToDB.Data;

namespace VTQT.Api.Dashboard.Helper
{
    public class TypeValueModelHelper : ITypeValueModelHelper
    {
        private readonly ITypeValueService _organizationalUnitService;
        private readonly Data.IRepository<TypeValue> _beginningWareHouseRepository;
        private readonly IWorkContext _workContext;

        public TypeValueModelHelper(
            ITypeValueService organizationalUnitService,
            IWorkContext workContext)
        {
            _workContext = workContext;
            _organizationalUnitService = organizationalUnitService;
            _beginningWareHouseRepository =
                EngineContext.Current.Resolve<Data.IRepository<TypeValue>>(DataConnectionHelper
                    .ConnectionStringNames.Dashboard);
        }

        public async Task<IList<TypeValuesTreeModel>> GetTypeValueTree(int? expandLevel, bool showHidden = false)
        {
            expandLevel = expandLevel ?? 1;
            var qq = new Queue<TypeValuesTreeModel>();
            var lstCheck = new List<TypeValuesTreeModel>();
            var result = new List<TypeValuesTreeModel>();
            var list = await GetTypeValuesAsync(showHidden);
            var organizationalUnitModels = list
                .Select(s => new TypeValuesTreeModel
                {
                    children = new List<TypeValuesTreeModel>(),
                    folder = false,
                    key = s.Id,
                    title = s.Name,
                    tooltip = s.Name,
                    ParentId = s.ParentId,
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

        private async Task<IList<TypeValueModel>> GetTypeValuesAsync(bool showHidden = false)
        {
            var models = _organizationalUnitService.GetAll(showHidden).ToList();


            StringBuilder sb = new StringBuilder();


            sb.Append("WITH RECURSIVE cte (Id, Name, ParentId) ");
            sb.Append("AS ");
            sb.Append("(SELECT ");
            sb.Append("      wh.Id, ");
            sb.Append("      wh.Name, ");
            sb.Append("      wh.ParentId ");
            sb.Append("    FROM TypeValue wh ");
            sb.Append("    INNER JOIN DashBoardUser whu ");
            sb.Append("    ON wh.Id = whu.TypeValueId ");
            sb.Append("    WHERE whu.UserId='" + _workContext.UserId + "' ");
            sb.Append("  UNION ALL ");
            sb.Append("  SELECT ");
            sb.Append("    p.Id, ");
            sb.Append("    p.Name, ");
            sb.Append("    p.ParentId ");
            sb.Append("  FROM TypeValue p ");
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
                models = models.Where(x => listDepartmentIds.Contains(x.Id)).ToList();
                var res = new List<TypeValueModel>();
                if (models?.Count > 0)
                {
                    models.ForEach(x =>
                    {
                        if (x.Inactive.Equals(showHidden))
                        {
                            var m = new TypeValueModel
                            {
                                Id = x.Id,
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

            return new List<TypeValueModel>();
        }

        public async Task<IList<TypeValueModel>> GetTypeValueDropdownTreeAsync(bool showHidden = false,
            bool showList = false)
        {
            var models = await GetTypeValueAsync(showHidden, showList);
            return GetTypeValueTreeModel(models);
        }

        private async Task<IList<TypeValueModel>> GetTypeValueAsync(bool showHidden = false, bool showList = false)
        {
            // var cacheKey =
            //     ModelCacheEventConsumer.AvailableOrganizationalUnitsModelKey.FormatWith(
            //         _services.WorkContext.WorkingLanguage.Id, showHidden);
            // var models = _services.Cache.Get(cacheKey, () =>
            // {
            var result = _organizationalUnitService.GetAll(showHidden)
                .Select(s => new TypeValueModel
                {
                    Id = s.Id,
                    ParentId = s.ParentId,
                    Name = s.Name
                })
                .OrderBy(o => o.Name)
                .ToList();
            if (!showList)
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

                var departmentIds =
                    await _beginningWareHouseRepository.DataConnection.QueryToListAsync<string>(sb.ToString());
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

        private List<TypeValueModel> GetTypeValueTreeModel(IEnumerable<TypeValueModel> models)
        {
            var parents = models
                .Where(w => !w.ParentId.HasValue())
                .OrderBy(o => o.Name);

            var result = new List<TypeValueModel>();
            var level = 0;
            foreach (var parent in parents)
            {
                result.Add(new TypeValueModel
                {
                    Id = parent.Id,
                    ParentId = parent.ParentId,
                    Name = parent.Name,
                });
                GetChildWareHouseTreeModel(ref models, parent.Id, ref result, level);
            }

            return result;
        }

        private void GetChildWareHouseTreeModel(ref IEnumerable<TypeValueModel> models, string parentId,
            ref List<TypeValueModel> result, int level)
        {
            level++;
            var childs = models
                .Where(w => w.ParentId == parentId)
                .OrderBy(o => o.Name);

            if (childs.Any())
            {
                foreach (var child in childs)
                {
                    child.Name = child.Name;
                    result.Add(new TypeValueModel()
                    {
                        Id = child.Id,
                        ParentId = child.ParentId,
                        Name = level.ToTreeLevelString() + child.Name
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
    }
}
using LinqToDB;
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
using VTQT.SharedMvc.Warehouse.Models;

namespace VTQT.Services.Warehouse
{
    public partial class WareHouseLimitService : IWareHouseLimitService
    {
        #region Fields

        private readonly IRepository<WareHouseLimit> _repository;
        private readonly IXBaseCacheManager _cacheManager;
        private readonly IRepository<Unit> _unitRepository;
        private readonly IRepository<WareHouse> _wareHouseRepository;
        private readonly IRepository<WareHouseItem> _wareHouseItemRepository;
        private readonly IRepository<BeginningWareHouse> _beginningWareHouseRepository;
        private readonly IRepository<OutwardDetail> _outwardDetailRepository;
        private readonly IRepository<InwardDetail> _inwardDetailRepository;
        private readonly IRepository<Outward> _outwardRepository;
        private readonly IRepository<Inward> _inwardRepository;
        private readonly IWorkContext _workContext;

        #endregion Fields

        #region Ctor

        public WareHouseLimitService(
            IXBaseCacheManager cacheManager, IWorkContext workContext)
        {
            _workContext = workContext;
            _repository =
                EngineContext.Current.Resolve<IRepository<WareHouseLimit>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _cacheManager = cacheManager;
            _unitRepository =
                EngineContext.Current.Resolve<IRepository<Unit>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _wareHouseRepository =
                EngineContext.Current.Resolve<IRepository<WareHouse>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _wareHouseItemRepository =
                EngineContext.Current.Resolve<IRepository<WareHouseItem>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _outwardRepository =
                EngineContext.Current.Resolve<IRepository<Outward>>(
                    DataConnectionHelper.ConnectionStringNames.Warehouse);
            _inwardRepository =
                EngineContext.Current.Resolve<IRepository<Inward>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _outwardDetailRepository =
                EngineContext.Current.Resolve<IRepository<OutwardDetail>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _inwardDetailRepository =
                EngineContext.Current.Resolve<IRepository<InwardDetail>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _beginningWareHouseRepository =
                EngineContext.Current.Resolve<IRepository<BeginningWareHouse>>(DataConnectionHelper
                    .ConnectionStringNames.Warehouse);
        }

        #endregion Ctor

        #region Methods

        public virtual async Task<int> InsertAsync(WareHouseLimit entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            var result = await _repository.InsertAsync(entity);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.WareHouseLimit.Prefix);

            return result;
        }

        public virtual async Task<long> InsertRangeAsync(IEnumerable<WareHouseLimit> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));
                var result = await _repository.InsertAsync(entities);

                await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.WareHouseLimit.Prefix);

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public virtual async Task<int> UpdateAsync(WareHouseLimit entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var result = await _repository.UpdateAsync(entity);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.WareHouseLimit.Prefix);

            return result;
        }

        public virtual async Task<int> UpdateRangeAsync(IEnumerable<WareHouseLimit> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = await _repository.UpdateAsync(entities);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.WareHouseLimit.Prefix);

            return result;
        }

        public virtual async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _repository.DeleteAsync(ids);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.WareHouseLimit.Prefix);

            return result;
        }

        public virtual IList<WareHouseLimit> GetAll()
        {
            var key = WarehouseCacheKeys.WareHouseLimit.AllCacheKey;
            var entities = _cacheManager.GetDb(key, () =>
            {
                var query = from p in _repository.Table select p;
                query =
                    from p in query
                    orderby p.CreatedDate
                    select p;

                return query.ToList();
            }, CachingDefaults.MonthCacheTime);

            return entities;
        }

        public IList<WareHouseLimit> GetSelect()
        {
            var query = from i in _repository.Table
                join v in _wareHouseRepository.Table on i.WareHouseId equals v.Id into vi
                join c in _wareHouseItemRepository.Table on i.ItemId equals c.Id into ci
                from iv in vi.DefaultIfEmpty()
                from ic in ci.DefaultIfEmpty()
                select new WareHouseLimit
                {
                    Id = i.Id,
                    ItemId = i.ItemId,
                    WareHouseId = i.WareHouseId,
                    MinQuantity = i.MinQuantity,
                    MaxQuantity = i.MaxQuantity,

                    WareHouse = iv == null
                        ? null
                        : new WareHouse
                        {
                            Id = iv.Id,
                            Code = iv.Code,
                            Name = iv.Name
                        },
                    WareHouseItem = ic == null
                        ? null
                        : new WareHouseItem
                        {
                            Id = ic.Id,
                            Code = ic.Code,
                            Name = ic.Name
                        }
                };

            return query.ToList();
        }

        public virtual async Task<IList<WareHouseLimit>> Get(WareHouseLimitSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _repository.Table
                join u in _unitRepository.Table on i.UnitId equals u.Id into ui
                join v in _wareHouseRepository.Table on i.WareHouseId equals v.Id into vi
                join c in _wareHouseItemRepository.Table on i.ItemId equals c.Id into ci
                from iu in ui.DefaultIfEmpty()
                from iv in vi.DefaultIfEmpty()
                from ic in ci.DefaultIfEmpty()
                select new WareHouseLimit
                {
                    Id = i.Id,
                    ItemId = i.ItemId,
                    WareHouseId = i.WareHouseId,
                    UnitId = i.UnitId,
                    UnitName = i.UnitName,
                    MinQuantity = i.MinQuantity,
                    MaxQuantity = i.MaxQuantity,
                    CreatedBy = i.CreatedBy,
                    CreatedDate = i.CreatedDate,
                    ModifiedBy = i.ModifiedBy,
                    ModifiedDate = i.ModifiedDate,


                    Unit = iu == null
                        ? null
                        : new Unit
                        {
                            Id = iu.Id,
                            UnitName = iu.UnitName
                        },
                    WareHouse = iv == null
                        ? null
                        : new WareHouse
                        {
                            Id = iv.Id,
                            Code = iv.Code,
                            Name = iv.Name
                        },
                    WareHouseItem = ic == null
                        ? null
                        : new WareHouseItem
                        {
                            Id = ic.Id,
                            Code = ic.Code,
                            Name = "[" + ic.Code + "] " + ic.Name
                        }
                };

            if (ctx.Keywords.HasValue())
            {
                //searchItem
                var queryItem = from i in _wareHouseItemRepository.Table
                    where i.Name.Contains(ctx.Keywords) ||
                          i.Code.Contains(ctx.Keywords)
                    select i.Id;


                if (queryItem?.ToList().Count > 0)
                {
                    var itemIds = queryItem.ToList();
                    query = query.Where(x => itemIds.Contains(x.ItemId));
                }
            }

            if (ctx.WareHouesId.HasValue())
            {
                var department = _wareHouseRepository.Table.FirstOrDefault(x => x.Id == ctx.WareHouesId);

                if (department != null)
                {
                    StringBuilder GetListChidren = new StringBuilder();

                    GetListChidren.Append("with recursive cte (Id, Name, ParentId) as ( ");
                    GetListChidren.Append("  select     wh.Id, ");
                    GetListChidren.Append("             wh.Name, ");
                    GetListChidren.Append("             wh.ParentId ");
                    GetListChidren.Append("  from       WareHouse wh ");
                    GetListChidren.Append("  where      wh.ParentId='" + ctx.WareHouesId + "' ");
                    GetListChidren.Append("  union all ");
                    GetListChidren.Append("  SELECT     p.Id, ");
                    GetListChidren.Append("             p.Name, ");
                    GetListChidren.Append("             p.ParentId ");
                    GetListChidren.Append("  from       WareHouse  p ");
                    GetListChidren.Append("  inner join cte ");
                    GetListChidren.Append("          on p.ParentId = cte.id ");
                    GetListChidren.Append(") ");
                    GetListChidren.Append(" select cte.Id FROM cte GROUP BY cte.Id,cte.Name,cte.ParentId; ");
                    var departmentIds =
                        await _unitRepository.DataConnection.QueryToListAsync<string>(GetListChidren.ToString());
                    departmentIds.Add(ctx.WareHouesId);
                    if (departmentIds?.ToList().Count > 0)
                    {
                        var listDepartmentIds = departmentIds.ToList();
                        query = from p in query
                                where listDepartmentIds.Contains(p.WareHouseId)
                                select p;
                    }
                    else
                        return new List<WareHouseLimit>();
                }
                else
                {
                    query = from p in query
                        where p.WareHouseId.Contains(ctx.WareHouesId.Trim())
                        select p;
                }
            }
            else
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
                    query = from p in query where listDepartmentIds.Contains(p.WareHouseId) select p;
                }

            }


            if (ctx.FromDate.HasValue)
                query = from p in query
                    where p.CreatedDate >= ctx.FromDate
                    select p;
            if (ctx.ToDate.HasValue)
                query = from p in query
                    where p.CreatedDate <= ctx.FromDate
                    select p;
            if (!ctx.DateSoft)
                query =
                    from p in query
                    orderby p.CreatedDate
                    select p;
            else
                query = from p in query
                    orderby p.CreatedDate descending
                    select p;

            return query.ToList();
        }

        public virtual async Task<WareHouseLimit> GetByIdAsync(string id)
        {
            if (id.IsEmpty())
                throw new ArgumentException(nameof(id));

            return await _repository.GetByIdAsync(id);
        }

        public async Task<IPagedList<WareHouseLimit>> GetToHome(WareHouseLimitSearchContext ctx)
        {
            var check = false;
            var list = new StringBuilder();
            list.Append(" ( ");
            var department = _wareHouseRepository.Table.FirstOrDefault(x => x.Id == ctx.WareHouesId);

            if (department != null)
            {
                StringBuilder GetListChidren = new StringBuilder();

                GetListChidren.Append("with recursive cte (Id, Name, ParentId) as ( ");
                GetListChidren.Append("  select     wh.Id, ");
                GetListChidren.Append("             wh.Name, ");
                GetListChidren.Append("             wh.ParentId ");
                GetListChidren.Append("  from       WareHouse wh ");
                GetListChidren.Append("  where      wh.ParentId='" + ctx.WareHouesId + "' ");
                GetListChidren.Append("  union all ");
                GetListChidren.Append("  SELECT     p.Id, ");
                GetListChidren.Append("             p.Name, ");
                GetListChidren.Append("             p.ParentId ");
                GetListChidren.Append("  from       WareHouse  p ");
                GetListChidren.Append("  inner join cte ");
                GetListChidren.Append("          on p.ParentId = cte.id ");
                GetListChidren.Append(") ");
                GetListChidren.Append(" select * FROM cte GROUP BY cte.Id,cte.Name,cte.ParentId; ");
                var departmentIds =
                    await _inwardRepository.DataConnection.QueryToListAsync<WareHouse>(GetListChidren.ToString());
                if (departmentIds?.ToList().Count > 0)
                {
                    check = true;
                    var listDepartmentIds = departmentIds.ToList();
                    for (int i = 0; i < listDepartmentIds.Count; i++)
                    {
                        if (i == listDepartmentIds.Count - 1)
                            list.Append("'" + listDepartmentIds[i].Id + "','" + ctx.WareHouesId + "'");
                        else
                            list.Append("'" + listDepartmentIds[i].Id + "'" + ", ");
                    }
                }
            }

            list.Append(" ) ");

            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("  whi.Name AS ItemId, ");
           // sb.Append("  whi.Id AS WareHouseId, ");
            sb.Append(" case ");
            sb.Append(" WHEN SUM(d1.Quantity) is null then 0 ");
            sb.Append(" else SUM(d1.Quantity)  ");
            sb.Append(" end AS UnitName,  ");
            sb.Append("  SUM(whl.MinQuantity) as MinQuantity, ");
            sb.Append("  SUM(whl.MaxQuantity) as MaxQuantity ");
            sb.Append("FROM (SELECT ");
            sb.Append("    bwh.ItemId, ");
            sb.Append("    bwh.WareHouseId, ");
            sb.Append("    bwh.Quantity ");
            sb.Append("  FROM BeginningWareHouse bwh ");
            sb.Append("  WHERE bwh.WareHouseId = @p1 ");
            sb.Append("  UNION ALL ");
            sb.Append("  SELECT ");
            sb.Append("    id.ItemId, ");
            sb.Append("    i.WareHouseId, ");
            sb.Append("    id.Quantity ");
            sb.Append("  FROM Inward i ");
            sb.Append("    INNER JOIN InwardDetail id ");
            sb.Append("      ON i.id = id.InwardId ");
            sb.Append("  WHERE i.WareHouseId = @p1 ");
            sb.Append("  UNION ALL ");
            sb.Append("  SELECT ");
            sb.Append("    od.ItemId, ");
            sb.Append("    o.WareHouseId, ");
            sb.Append("    -od.Quantity ");
            sb.Append("  FROM Outward o ");
            sb.Append("    INNER JOIN OutwardDetail od ");
            sb.Append("      ON o.Id = od.OutwardId ");
            sb.Append("  WHERE o.WareHouseId = @p1) d1 ");
            sb.Append("  right JOIN WareHouseItem whi ");
            sb.Append("    ON d1.ItemId = whi.Id ");
            sb.Append("  inner JOIN WareHouseLimit whl ");
            sb.Append("    ON whl.ItemId = whi.Id ");

            //  sb.Append("    WHERE whl.WareHouseId=@p1 ");
            sb.Append("     ");
            if (department != null && check)
            {
                sb.Append(" WHERE whl.WareHouseId in ");
                sb.Append(list);
            }
            else
            {
                sb.Append("    WHERE whl.WareHouseId=@p1 ");
            }

            sb.Append("GROUP BY  ");
       //     sb.Append("GROUP BY d1.ItemId, ");
        //    sb.Append("         whi.Id, ");
         //   sb.Append("         whi.Name, ");
            sb.Append("         whi.Name ");
        //    sb.Append("         whl.MaxQuantity, ");
         //   sb.Append("         whl.MinQuantity   ");
            //return @p2 record, start from @p3 record
            sb.Append(" LIMIT @p2 OFFSET @p3 ");
            sb.Append("  ");

            DataParameter p1 = new DataParameter("p1", ctx.WareHouesId);
            DataParameter p2 = new DataParameter("p2", ctx.PageSize);
            DataParameter p3 = new DataParameter("p3", ctx.PageIndex * ctx.PageSize);


            StringBuilder sbCount = new StringBuilder();

            sbCount.Append("SELECT COUNT(*) FROM ( ");
            sbCount.Append("  SELECT ");
            sbCount.Append("    whi.Name AS ItemId ");
            sbCount.Append("FROM (SELECT ");
            sbCount.Append("    bwh.ItemId, ");
            sbCount.Append("    bwh.WareHouseId, ");
            sbCount.Append("    bwh.Quantity ");
            sbCount.Append("  FROM BeginningWareHouse bwh ");
            sbCount.Append("  WHERE bwh.WareHouseId = @p1 ");
            sbCount.Append("  UNION ALL ");
            sbCount.Append("  SELECT ");
            sbCount.Append("    id.ItemId, ");
            sbCount.Append("    i.WareHouseId, ");
            sbCount.Append("    id.Quantity ");
            sbCount.Append("  FROM Inward i ");
            sbCount.Append("    INNER JOIN InwardDetail id ");
            sbCount.Append("      ON i.id = id.InwardId ");
            sbCount.Append("  WHERE i.WareHouseId = @p1 ");
            sbCount.Append("  UNION ALL ");
            sbCount.Append("  SELECT ");
            sbCount.Append("    od.ItemId, ");
            sbCount.Append("    o.WareHouseId, ");
            sbCount.Append("    -od.Quantity ");
            sbCount.Append("  FROM Outward o ");
            sbCount.Append("    INNER JOIN OutwardDetail od ");
            sbCount.Append("      ON o.Id = od.OutwardId ");
            sbCount.Append("  WHERE o.WareHouseId = @p1) d1 ");
            sbCount.Append("  right JOIN WareHouseItem whi ");
            sbCount.Append("    ON d1.ItemId = whi.Id ");
            sbCount.Append("  inner JOIN WareHouseLimit whl ");
            sbCount.Append("    ON whl.ItemId = whi.Id ");
            if (department != null && check)
            {
                sbCount.Append(" WHERE whl.WareHouseId in ");
                sbCount.Append(list);
            }
            else
            {
                sbCount.Append("    WHERE whl.WareHouseId=@p1 ");
            }

            sbCount.Append("GROUP BY d1.ItemId, ");
            sbCount.Append("         whi.Id, ");
            sbCount.Append("         whi.Name, ");
            sbCount.Append("         whl.MaxQuantity, ");
            sbCount.Append("         whl.MinQuantity   ");
            sbCount.Append("              ) t   ");
            sbCount.Append("  ");


            var res = await _inwardRepository.DataConnection.QueryToListAsync<WareHouseLimit>(sb.ToString(), p1, p2,
                p3);
            var resCount =
                await _beginningWareHouseRepository.DataConnection.QueryToListAsync<int>(sbCount.ToString(), p1);

            return new PagedList<WareHouseLimit>(res, ctx.PageIndex, ctx.PageSize, resCount.FirstOrDefault());
        }

        public async Task<bool> ExistAsync(string idWareHouse, string idItem)
        {
            if (idWareHouse.IsEmpty() || idItem.IsEmpty())
                throw new ArgumentException(nameof(idItem));


            var check = await _repository.Table.AnyAsync(
                a => a.WareHouseId.Equals(idWareHouse)
                     && a.ItemId.Equals(idItem));
            return check;
        }

        #endregion Methods


        private void GetChildWareHouseTreeModel(ref IEnumerable<WareHouse> models, string parentId,
            ref List<WareHouse> result, int level)
        {
            level++;
            var childs = models.Where(w => w.ParentId == parentId);

            if (childs.Any())
            {
                foreach (var child in childs)
                {
                    child.Name = "[" + child.Code + "] " + child.Name;
                    result.Add(new WareHouse()
                    {
                        Id = child.Id,
                        ParentId = child.ParentId
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
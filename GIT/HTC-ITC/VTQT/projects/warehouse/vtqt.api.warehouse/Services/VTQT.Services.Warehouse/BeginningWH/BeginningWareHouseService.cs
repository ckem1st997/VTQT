using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Core.Domain.Master;
using VTQT.Core.Domain.Warehouse;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Warehouse
{
    public partial class BeginningWareHouservice : IBeginningWareHouseService
    {
        #region Constants

        #endregion

        #region Fields

        private readonly IRepository<BeginningWareHouse> _beginningWareHouseRepository;
        private readonly IXBaseCacheManager _cacheManager;
        private readonly IRepository<Unit> _unitRepository;
        private readonly IRepository<WareHouse> _wareHouseRepository;
        private readonly IRepository<WareHouseItem> _wareHouseItemRepository;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public BeginningWareHouservice(
            IXBaseCacheManager cacheManager, IWorkContext workContext)
        {
            _workContext = workContext;
            _beginningWareHouseRepository =
                EngineContext.Current.Resolve<IRepository<BeginningWareHouse>>(DataConnectionHelper
                    .ConnectionStringNames.Warehouse);
            _unitRepository =
                EngineContext.Current.Resolve<IRepository<Unit>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _wareHouseRepository =
                EngineContext.Current.Resolve<IRepository<WareHouse>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _wareHouseItemRepository =
                EngineContext.Current.Resolve<IRepository<WareHouseItem>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        public virtual async Task<int> InsertAsync(BeginningWareHouse entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            var result = await _beginningWareHouseRepository.InsertAsync(entity);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.BeginningWareHouse.Prefix);

            return result;
        }

        public virtual async Task<long> InsertRangeAsync(IEnumerable<BeginningWareHouse> entities)
        {
            try
            {
                if (entities is null)
                    throw new ArgumentNullException(nameof(entities));
                var result = await _beginningWareHouseRepository.InsertAsync(entities);

                await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.WareHouseLimit.Prefix);

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public virtual async Task<int> UpdateAsync(BeginningWareHouse entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var result = await _beginningWareHouseRepository.UpdateAsync(entity);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.BeginningWareHouse.Prefix);

            return result;
        }

        public virtual async Task<int> UpdateRangeAsync(IEnumerable<BeginningWareHouse> entities)
        {
            if (entities is null)
                throw new ArgumentNullException(nameof(entities));

            var result = await _beginningWareHouseRepository.UpdateAsync(entities);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.WareHouseLimit.Prefix);

            return result;
        }

        public virtual async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _beginningWareHouseRepository.DeleteAsync(ids);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.BeginningWareHouse.Prefix);

            return result;
        }

        public virtual IList<BeginningWareHouse> GetAll()
        {
            var key = WarehouseCacheKeys.BeginningWareHouse.AllCacheKey;
            var entities = _cacheManager.GetDb(key, () =>
            {
                var query = from p in _beginningWareHouseRepository.Table select p;
                query =
                    from p in query
                    orderby p.CreatedDate
                    select p;

                return query.ToList();
            }, CachingDefaults.MonthCacheTime);

            return entities;
        }

        public virtual async Task<IPagedList<BeginningWareHouse>> Get(BeginningWareHouseSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _beginningWareHouseRepository.Table
                join u in _unitRepository.Table on i.UnitId equals u.Id into ui
                join v in _wareHouseRepository.Table on i.WareHouseId equals v.Id into vi
                join c in _wareHouseItemRepository.Table on i.ItemId equals c.Id into ci
                from iu in ui.DefaultIfEmpty()
                from iv in vi.DefaultIfEmpty()
                from ic in ci.DefaultIfEmpty()
                select new BeginningWareHouse
                {
                    Id = i.Id,
                    ItemId = i.ItemId,
                    WareHouseId = i.WareHouseId,
                    UnitId = i.UnitId,
                    UnitName = i.UnitName,
                    Quantity = i.Quantity,
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
                else
                    return new PagedList<BeginningWareHouse>(new List<BeginningWareHouse>(), ctx.PageIndex, ctx.PageSize);


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

            return new PagedList<BeginningWareHouse>(query, ctx.PageIndex, ctx.PageSize);
        }

        public virtual async Task<BeginningWareHouse> GetByIdAsync(string id)
        {
            if (id.IsEmpty())
                throw new ArgumentException(nameof(id));

            return await _beginningWareHouseRepository.GetByIdAsync(id);
        }

        public async Task<bool> ExistAsync(string idWareHouse, string idItem)
        {
            if (idWareHouse.IsEmpty() || idItem.IsEmpty())
                throw new ArgumentException(nameof(idItem));


            var check = await _beginningWareHouseRepository.Table.AnyAsync(
                a => a.WareHouseId.Equals(idWareHouse)
                     && a.ItemId.Equals(idItem));
            return check;
        }

        public async Task<IEnumerable<WareHouseItem>> GetByWareHouseIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException(nameof(id));
            // BuildMyString.com generated code. Please enjoy your string responsibly.

            // BuildMyString.com generated code. Please enjoy your string responsibly.

            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("  whi.Id, ");
            sb.Append("  whi.Code, ");
            sb.Append("  CONCAT('[', whi.Code, '] ', whi.Name) AS Name ");
            sb.Append("FROM BeginningWareHouse bwh ");
            sb.Append("  JOIN WareHouseItem whi ");
            sb.Append("    ON bwh.ItemId = whi.Id ");
            sb.Append("WHERE bwh.WareHouseId = @p1 ");


            DataParameter p1 = new DataParameter("p1", id);

            return await _beginningWareHouseRepository.DataConnection
                .QueryToListAsync<WareHouseItem>(sb.ToString(), p1);
        }

        #endregion
    }
}
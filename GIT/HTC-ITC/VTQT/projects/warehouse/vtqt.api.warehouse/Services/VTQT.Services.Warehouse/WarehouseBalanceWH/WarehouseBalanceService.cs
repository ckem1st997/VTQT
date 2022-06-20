using LinqToDB;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VTQT.Core;
using LinqToDB.Data;
using VTQT.Core.Domain.Warehouse;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Warehouse
{
    public partial class WarehouseBalanceService : IWarehouseBalanceService
    {
        #region Constants

        #endregion

        #region Fields

        private readonly IRepository<WarehouseBalance> _repository;
        private readonly IRepository<WareHouse> _repositoryWareHouse;
        private readonly IRepository<BeginningWareHouse> _beginningWareHouseRepository;
        private readonly IRepository<OutwardDetail> _outwardDetailRepository;
        private readonly IRepository<InwardDetail> _inwardDetailRepository;
        private readonly IRepository<Outward> _outwardRepository;
        private readonly IRepository<Inward> _inwardRepository;

        #endregion

        #region Ctor

        public WarehouseBalanceService()
        {
            _outwardRepository =
                EngineContext.Current.Resolve<IRepository<Outward>>(
                    DataConnectionHelper.ConnectionStringNames.Warehouse);
            _inwardRepository =
                EngineContext.Current.Resolve<IRepository<Inward>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _repository =
                EngineContext.Current.Resolve<IRepository<WarehouseBalance>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _repositoryWareHouse =
                EngineContext.Current.Resolve<IRepository<WareHouse>>(DataConnectionHelper.ConnectionStringNames
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

        #endregion

        #region Methods

        public virtual async Task<int> InsertAsync(WarehouseBalance entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            var result = await _repository.InsertAsync(entity);

            return result;
        }

        public virtual async Task<int> UpdateAsync(WarehouseBalance entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            var result = await _repository.UpdateAsync(entity);

            return result;
        }

        public virtual async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _repository.DeleteAsync(ids);

            return result;
        }

        public virtual IPagedList<WarehouseBalance> Get(WarehouseBalanceSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _repository.Table select p;

            if (ctx.Keywords.HasValue())
            {
                query = query.Where(el =>
                    el.ItemId.Contains(ctx.Keywords) ||
                    el.WarehouseId.Contains(ctx.Keywords)).Select(el => el).Distinct();
            }

            if (ctx.fromAmount.HasValue)
                query = from p in query
                    where p.Amount >= ctx.fromAmount
                    select p;
            if (ctx.fromQuantity.HasValue)
                query = from p in query
                    where p.Amount >= ctx.fromQuantity
                    select p;
            if (ctx.toQuantity.HasValue && ctx.toQuantity >= ctx.fromQuantity)
                query = from p in query
                    where p.Quantity <= ctx.toQuantity
                    select p;
            if (ctx.toAmount.HasValue && ctx.toAmount >= ctx.fromAmount)
                query = from p in query
                    where p.Amount <= ctx.toAmount
                    select p;
            return new PagedList<WarehouseBalance>(query, ctx.PageIndex, ctx.PageSize);
        }

        public virtual async Task<WarehouseBalance> GetByIdAsync(string id)
        {
            if (id.IsEmpty())
                throw new ArgumentException(nameof(id));

            return await _repository.GetByIdAsync(id);
        }

        public async Task<IPagedList<WarehouseBalance>> GetTableToHome(WarehouseBalanceSearchContext ctx)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("  wh.Name AS WarehouseId, ");
            sb.Append("  d1.WarehouseId AS Id, ");
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
            sb.Append("  AND i.VoucherDate <= '2099-12-12' ");
            sb.Append("  UNION ALL ");
            sb.Append("  SELECT ");
            sb.Append("    od.ItemId, ");
            sb.Append("    o.WarehouseId, ");
            sb.Append("    -od.Quantity ");
            sb.Append("  FROM Outward o ");
            sb.Append("    INNER JOIN OutwardDetail od ");
            sb.Append("      ON o.Id = od.OutwardId ");
            sb.Append("  WHERE od.ItemId= @p1 ");
            sb.Append("  AND o.VoucherDate <= '2099-12-12' ");
            sb.Append("  ) d1 ");
            sb.Append("  INNER JOIN WareHouse wh ");
            sb.Append("    ON d1.WarehouseId = wh.Id ");
            sb.Append("GROUP BY d1.ItemId, ");
            sb.Append("         d1.WarehouseId   ");
            sb.Append(" LIMIT @p2 OFFSET @p3 ");
            sb.Append("  ");
            DataParameter p1 = new DataParameter("p1", ctx.Keywords);
            DataParameter p2 = new DataParameter("p2", ctx.PageSize);
            DataParameter p3 = new DataParameter("p3", ctx.PageIndex * ctx.PageSize);


            StringBuilder sbCount = new StringBuilder();
            sbCount.Append("SELECT COUNT(*) FROM ( ");
            sbCount.Append("SELECT ");
            sbCount.Append("  wh.Name AS WarehouseId ");
            sbCount.Append("   ");
            sbCount.Append("FROM (SELECT ");
            sbCount.Append("    bwh.ItemId, ");
            sbCount.Append("    bwh.WarehouseId, ");
            sbCount.Append("    bwh.Quantity ");
            sbCount.Append("  FROM BeginningWareHouse bwh ");
            sbCount.Append("  WHERE bwh.ItemId = @p1 ");
            sbCount.Append("  UNION ALL ");
            sbCount.Append("  SELECT ");
            sbCount.Append("    id.ItemId, ");
            sbCount.Append("    i.WarehouseId, ");
            sbCount.Append("    id.Quantity ");
            sbCount.Append("  FROM Inward i ");
            sbCount.Append("    INNER JOIN InwardDetail id ");
            sbCount.Append("      ON i.id = id.InwardId ");
            sbCount.Append("  WHERE id.ItemId = @p1 ");
            sbCount.Append("  AND i.VoucherDate <= '2099-12-12' ");
            sbCount.Append("  UNION ALL ");
            sbCount.Append("  SELECT ");
            sbCount.Append("    od.ItemId, ");
            sbCount.Append("    o.WarehouseId, ");
            sbCount.Append("    -od.Quantity ");
            sbCount.Append("  FROM Outward o ");
            sbCount.Append("    INNER JOIN OutwardDetail od ");
            sbCount.Append("      ON o.Id = od.OutwardId ");
            sbCount.Append("  WHERE od.ItemId= @p1 ");
            sbCount.Append("  AND o.VoucherDate <= '2099-12-12' ");
            sbCount.Append("  ) d1 ");
            sbCount.Append("  INNER JOIN WareHouse wh ");
            sbCount.Append("    ON d1.WarehouseId = wh.Id ");
            sbCount.Append("GROUP BY d1.ItemId, ");
            sbCount.Append("         d1.WarehouseId   ");
            sbCount.Append("              ) t   ");


            var res = await _repository.DataConnection.QueryToListAsync<WarehouseBalance>(sb.ToString(), p1, p2, p3);
            var resCount =
                await _beginningWareHouseRepository.DataConnection.QueryToListAsync<int>(sbCount.ToString(), p1);

            foreach (var item in res)
            {
                StringBuilder GetListChidren = new StringBuilder();

                GetListChidren.Append("with recursive cte (Id, Name, ParentId) as ( ");
                GetListChidren.Append("  select     wh.Id, ");
                GetListChidren.Append("             wh.Name, ");
                GetListChidren.Append("             wh.ParentId ");
                GetListChidren.Append("  from       WareHouse wh ");
                GetListChidren.Append("  where      wh.ParentId='" + item.Id + "' ");
                GetListChidren.Append("  union all ");
                GetListChidren.Append("  SELECT     p.Id, ");
                GetListChidren.Append("             p.Name, ");
                GetListChidren.Append("             p.ParentId ");
                GetListChidren.Append("  from       WareHouse  p ");
                GetListChidren.Append("  inner join cte ");
                GetListChidren.Append("          on p.ParentId = cte.id ");
                GetListChidren.Append(") ");
                GetListChidren.Append(" select Id FROM cte GROUP BY cte.Id,cte.Name,cte.ParentId; ");
                var departmentIds =await _inwardRepository.DataConnection.QueryToListAsync<string>(GetListChidren.ToString());
                if (departmentIds?.ToList().Count > 0)
                {
                    var listDepartmentIds = departmentIds.ToList();
                    listDepartmentIds.Add(item.Id);
                    item.Amount = res.Where(x => listDepartmentIds.Contains(x.Id)).Sum(x => x.Amount);
                }

            }


            return new PagedList<WarehouseBalance>(res, ctx.PageIndex, ctx.PageSize, resCount.FirstOrDefault());
        }

        #endregion
    }
}
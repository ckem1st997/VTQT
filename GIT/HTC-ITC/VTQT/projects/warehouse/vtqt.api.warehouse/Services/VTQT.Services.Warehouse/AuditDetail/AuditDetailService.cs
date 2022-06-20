using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using VTQT.Core.Data;
using VTQT.Core.Domain.Warehouse;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Warehouse
{
    public partial class AuditDetailService : IAuditDetailService
    {
        #region Fields

        private readonly IRepository<AuditDetail> _auditDetailRepository;
        private readonly IRepository<WareHouseItem> _wareHouseItemRepository;
        private readonly IRepository<WareHouse> _wareHouseRepository;
        private readonly IRepository<WareHouseItemCategory> _wareHouseItemCategoryRepository;

        #endregion Fields

        #region Ctor

        public AuditDetailService()
        {
            _wareHouseRepository = EngineContext.Current.Resolve<IRepository<WareHouse>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _auditDetailRepository = EngineContext.Current.Resolve<IRepository<AuditDetail>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _wareHouseItemRepository = EngineContext.Current.Resolve<IRepository<WareHouseItem>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _wareHouseItemCategoryRepository = EngineContext.Current.Resolve<IRepository<WareHouseItemCategory>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
        }

        #endregion Ctor

        #region Methods

        public async Task InsertAsync(AuditDetail entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = TransactionConfig.IsolationLevel },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var db = _auditDetailRepository.DataConnection;

                await db.InsertAsync(entity);

                if (entity.AuditDetailSerials != null && entity.AuditDetailSerials.Any())
                {
                    var bulkSerialResult = await db.BulkCopyAsync(new BulkCopyOptions(), entity.AuditDetailSerials);
                }

                transaction.Complete();
            }
        }

        public async Task UpdateAsync(AuditDetail entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = TransactionConfig.IsolationLevel },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var db = _auditDetailRepository.DataConnection;

                await db.UpdateAsync(entity);

                await db.GetTable<AuditDetailSerial>()
                    .Where(w => w.AuditDetailId == entity.Id)
                    .DeleteAsync();

                if (entity.AuditDetailSerials != null && entity.AuditDetailSerials.Any())
                {
                    var bulkSerialResult = await db.BulkCopyAsync(new BulkCopyOptions(), entity.AuditDetailSerials);
                }

                transaction.Complete();
            }
        }

        public async Task DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            await _auditDetailRepository.DeleteAsync(ids);
        }

        public virtual IList<AuditDetail> GetByAuditId(AuditDetailSearchContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.AuditId))
                throw new ArgumentNullException(nameof(ctx.AuditId));

            var query =
                from x in _auditDetailRepository.Table
                join i in _wareHouseItemRepository.Table on x.ItemId equals i.Id into ij
                from i in ij
                where x.AuditId == ctx.AuditId
                orderby i.Code
                select x;

            return query.ToList();
        }

        public async Task<AuditDetail> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _auditDetailRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<bool> ExistsAsync(string itemId, string auditId)
        {
            return await _auditDetailRepository.Table
                .AnyAsync(x => !string.IsNullOrEmpty(x.ItemId)
                && x.ItemId.Equals(itemId)
                && !string.IsNullOrEmpty(x.AuditId)
                && x.AuditId.Equals(auditId));
        }

        public IQueryable<AuditDetail> GetListById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            var result = from p in _auditDetailRepository.Table where p.AuditId.Equals(id.Trim()) select p;

            return result;
        }

        public IQueryable<AuditDetail> GetListShowNameById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            var query = from i in _auditDetailRepository.Table
                        join v in _wareHouseItemRepository.Table on i.ItemId equals v.Id into vi
                        from iv in vi.DefaultIfEmpty()
                        select new AuditDetail
                        {
                            Id = i.Id,
                            ItemId = i.ItemId,
                            AuditId = i.AuditId,
                            Quantity = i.Quantity,
                            AuditQuantity = i.AuditQuantity,
                            Conclude = i.Conclude,

                            WareHouseItem = iv == null ? null : new WareHouseItem
                            {
                                Id = iv.Id,
                                Code = iv.Code,
                                Name = iv.Name
                            }
                        };
            var result = from p in query where p.AuditId.Equals(id.Trim()) select p;

            return result;
        }

        public async Task DeletesAsync(IEnumerable<AuditDetail> auditDetails)
        {
            if (auditDetails == null)
            {
                throw new ArgumentNullException(nameof(auditDetails));
            }

            await _auditDetailRepository.DeleteAsync(auditDetails);
        }

        public async Task<IList<AuditDetail>> GetAuditDetail(AuditDetailSearchContext ctx)
        {
            // BuildMyString.com generated code. Please enjoy your string responsibly.

            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("  whi.Name AS UnitName, ");
            sb.Append(" case ");
            sb.Append(" WHEN SUM(d1.Quantity) is null then 0 ");
            sb.Append(" else SUM(d1.Quantity)  ");
            sb.Append(" end AS ItemId,  ");
            sb.Append("  whl.MinQuantity, ");
            sb.Append("  whl.MaxQuantity ");
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

            sb.Append("    WHERE whl.WareHouseId=@p1 ");
            sb.Append("GROUP BY d1.ItemId, ");
            sb.Append("         whi.Name, ");
            sb.Append("         whl.MaxQuantity, ");
            sb.Append("         whl.MinQuantity;   ");
            sb.Append("  ");

            DataParameter p1 = new DataParameter("p1", ctx.WareHouesId);

            return await _auditDetailRepository.DataConnection.QueryToListAsync<AuditDetail>(sb.ToString(), p1);
        }

        public async Task<IList<AuditDetail>> GetAuditDetailById(AuditDetailPagingContext ctx)
        {
            // BuildMyString.com generated code. Please enjoy your string responsibly.

            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("  whi.Name AS UnitName, ");
            sb.Append(" case ");
            sb.Append(" WHEN SUM(d1.Quantity) is null then 0 ");
            sb.Append(" else SUM(d1.Quantity)  ");
            sb.Append(" end AS ItemId,  ");
            sb.Append("  whl.MinQuantity, ");
            sb.Append("  whl.MaxQuantity ");
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

            sb.Append("    WHERE whl.WareHouseId=@p1 ");
            sb.Append("GROUP BY d1.ItemId, ");
            sb.Append("         whi.Name, ");
            sb.Append("         whl.MaxQuantity, ");
            sb.Append("         whl.MinQuantity;   ");
            sb.Append("  ");

            DataParameter p1 = new DataParameter("p1", ctx.WareHouesId);

            return await _auditDetailRepository.DataConnection.QueryToListAsync<AuditDetail>(sb.ToString(), p1);
        }

        public async Task<IList<AuditDetail>> GetAuditDetailByWareHouseId(string dateTime,string idWh, string idItem)
        {
            if (string.IsNullOrEmpty(idWh) && string.IsNullOrEmpty(idItem))
                throw new NotImplementedException("Not Id to Search Table");

            var list = new StringBuilder();
            list.Append(" ( '" + idWh + "'");
            var department = _wareHouseRepository.Table.FirstOrDefault(x => x.Id == idWh);

            if (department != null)
            {
                StringBuilder GetListChidren = new StringBuilder();

                GetListChidren.Append("with recursive cte (Id, Name, ParentId) as ( ");
                GetListChidren.Append("  select     wh.Id, ");
                GetListChidren.Append("             wh.Name, ");
                GetListChidren.Append("             wh.ParentId ");
                GetListChidren.Append("  from       WareHouse wh ");
                GetListChidren.Append("  where      wh.ParentId='" + idWh + "' ");
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
                    await _wareHouseRepository.DataConnection.QueryToListAsync<WareHouse>(GetListChidren.ToString());
                if (departmentIds?.ToList().Count > 0)
                {
                    list.Append(",");
                    var listDepartmentIds = departmentIds.ToList();
                    for (int i = 0; i < listDepartmentIds.Count; i++)
                    {
                        if (i == listDepartmentIds.Count - 1)
                            list.Append("'" + listDepartmentIds[i].Id + "'");
                        else
                            list.Append("'" + listDepartmentIds[i].Id + "'" + ", ");
                    }
                }
            }

            list.Append(" ) ");

            // BuildMyString.com generated code. Please enjoy your string responsibly.
            var date = dateTime;
            // var date = DateTime.UtcNow;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("  whi.Id AS ItemId, ");
            sb.Append("  CONCAT('[', whi.Code, '] ', whi.Name) AS AuditId, ");
            sb.Append("  SUM(d1.Quantity) AS Quantity, ");
            sb.Append("  u.UnitName ");
            sb.Append("FROM (SELECT ");
            sb.Append("    bwh.ItemId, ");
            sb.Append("    bwh.WareHouseId, ");
            sb.Append("    bwh.Quantity ");
            sb.Append("  FROM BeginningWareHouse bwh   ");
            sb.Append("  WHERE  bwh.CreatedDate <= '" + dateTime + "' ");
            sb.Append("    and bwh.WareHouseId in ");
            sb.Append(list);
            if (idItem.HasValue())
                sb.Append("  and  bwh.ItemId = @pItem ");
            sb.Append("  UNION ALL ");
            sb.Append("  SELECT ");
            sb.Append("    id.ItemId, ");
            sb.Append("    i.WareHouseId, ");
            sb.Append("    id.Quantity ");
            sb.Append("  FROM Inward i ");
            sb.Append("    INNER JOIN InwardDetail id ");
            sb.Append("      ON i.id = id.InwardId  ");
            sb.Append("    WHERE i.WareHouseId in ");
            sb.Append(list);
            if (idItem.HasValue())
                sb.Append(" and id.ItemId =  @pItem  ");
            sb.Append(" AND i.VoucherDate <= '" + dateTime + "' ");
            sb.Append("  UNION ALL ");
            sb.Append("  SELECT ");
            sb.Append("    od.ItemId, ");
            sb.Append("    o.WareHouseId, ");
            sb.Append("    -od.Quantity ");
            sb.Append("  FROM Outward o ");
            sb.Append("    INNER JOIN OutwardDetail od ");
            sb.Append("      ON o.Id = od.OutwardId  ");
            sb.Append("    WHERE o.WareHouseId in ");
            sb.Append(list);
            if (idItem.HasValue())
                sb.Append(" and  od.ItemId =  @pItem  ");
            sb.Append(" and  o.VoucherDate <= '"+dateTime+"') d1 ");
            sb.Append("  INNER JOIN WareHouseItem whi ");
            sb.Append("    ON d1.ItemId = whi.Id ");
            sb.Append("  INNER JOIN Unit u ON whi.UnitId = u.Id ");
            //sb.Append("WHERE d1.WareHouseId =  @pW ");
            sb.Append("GROUP BY d1.ItemId, ");
            sb.Append("         whi.Code, ");
            sb.Append("         whi.Name; ");

            DataParameter p1 = new DataParameter("pItem", idItem);
            DataParameter p2 = new DataParameter("pW", idWh);

            return await _auditDetailRepository.DataConnection.QueryToListAsync<AuditDetail>(sb.ToString(), p1, p2);
        }

        public virtual async Task<long> InsertRangeAsync(IEnumerable<AuditDetail> entities)
        {
            try
            {
                if (entities is null)
                    throw new ArgumentNullException(nameof(entities));
                var result = await _auditDetailRepository.UpdateAsync(entities);

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

      

        #endregion Methods
    }
}
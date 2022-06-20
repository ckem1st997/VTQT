using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using LinqToDB;
using LinqToDB.Data;
using VTQT.Core;
using VTQT.Core.Data;
using VTQT.Core.Domain.Warehouse;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Warehouse
{
    public partial class AuditService : IAuditService
    {
        #region Fields
        private readonly IRepository<Audit> _auditRepository;
        private readonly IRepository<WareHouse> _warehouseRepository;
        private readonly IWorkContext _workContext;
        private readonly IRepository<AuditDetail> _auditDetailRepository;
        private readonly IRepository<AuditCouncil> _auditCouncilRepository;
        private readonly IRepository<AuditDetailSerial> _audiDetailSerialRepository;

        #endregion

        #region Ctor
        public AuditService(IWorkContext workContext)
        {
            _auditDetailRepository = EngineContext.Current.Resolve<IRepository<AuditDetail>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _auditCouncilRepository = EngineContext.Current.Resolve<IRepository<AuditCouncil>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _auditRepository = EngineContext.Current.Resolve<IRepository<Audit>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _warehouseRepository = EngineContext.Current.Resolve<IRepository<WareHouse>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _audiDetailSerialRepository = EngineContext.Current.Resolve<IRepository<AuditDetailSerial>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _workContext = workContext;

        }
        #endregion

        #region Methods

        public virtual async Task InsertAsync(Audit entity, IList<AuditDetail> details = null, IList<AuditCouncil> auditCouncils = null)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // TODO-XBase-API-Authen
            var utcNow = DateTime.UtcNow;
            entity.CreatedDate = utcNow;
            entity.ModifiedDate = utcNow;

            var serials = new List<AuditDetailSerial>();
            if (details != null && details.Any())
            {
                serials = details
                    .Where(w => w.AuditDetailSerials != null && w.AuditDetailSerials.Any())
                    .SelectMany(s => s.AuditDetailSerials).ToList();
            }

            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = TransactionConfig.IsolationLevel },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var db = _auditRepository.DataConnection;

                // Sử dụng cùng 1 Repository.DataConnection CRUD các Entity khác nhau trong 1 Transaction để tránh lỗi:
                // System.NotSupportedException: Multiple simultaneous connections or connections with different connection strings inside the same transaction are not currently supported.
                await db.InsertAsync(entity);

                if (details != null && details.Any())
                {
                    var bulkDetailsResult = await db.BulkCopyAsync(new BulkCopyOptions(), details);

                    if (serials.Any())
                    {
                        var bulkSerialResult = await db.BulkCopyAsync(new BulkCopyOptions(), serials);
                    }
                }
                if (auditCouncils != null && auditCouncils.Any())
                {
                    var bulkDetailsResult1 = await db.BulkCopyAsync(new BulkCopyOptions(), auditCouncils);
                }

                transaction.Complete();
            }
        }

        public virtual async Task UpdateAsync(Audit entity, IList<AuditDetail> details = null, IList<string> deleteDetailIds = null, IList<AuditCouncil> auditCouncils = null, IList<string> deleteAuditCouncilIds = null)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // TODO-XBase-API-Authen
            var utcNow = DateTime.UtcNow;
            entity.ModifiedDate = utcNow;

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _auditRepository.UpdateAsync(entity);
        }

        public virtual void DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var auditDetail = from p in _auditDetailRepository.Table
                              where ids.Contains(p.AuditId)
                              select p.Id;

            var listSerial = from p in _audiDetailSerialRepository.Table
                             where auditDetail.Contains(p.AuditDetailId)
                             select p.Id;

            if (listSerial?.ToList().Count > 0)
            {
                _audiDetailSerialRepository.Delete(listSerial);

            }
            if (auditDetail?.ToList().Count > 0)
            {
                _auditDetailRepository.Delete(auditDetail);

            }
            var listAuditCouncil = from p in _auditCouncilRepository.Table
                                   where ids.Contains(p.AuditId)
                                   select p.Id;
            if (listAuditCouncil?.ToList().Count > 0)
            {
                _auditCouncilRepository.Delete(listAuditCouncil);

            }
            _auditRepository.Delete(ids);
        }

        public virtual async Task<Audit> GetByIdAsync(string id)
        {
            if (id.IsEmpty())
                throw new ArgumentException(nameof(id));

            return await _auditRepository.GetByIdAsync(id);
        }

        public virtual async Task<bool> ExistsAsync(string code)
        {
            return await _auditRepository.Table
                .AnyAsync(a =>
                    !string.IsNullOrEmpty(a.VoucherCode)
                    && a.VoucherCode.Equals(code)
                );
        }

        public virtual async Task<bool> ExistsAsync(string oldCode, string newCode)
        {
            return await _auditRepository.Table
                .AnyAsync(a =>
                    !string.IsNullOrEmpty(a.VoucherCode)
                    && a.VoucherCode.Equals(newCode)
                    && !a.VoucherCode.Equals(oldCode)
                );
        }
        #endregion

        #region List

        public async Task<IPagedList<Audit>> GetListShowName(AuditSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _auditRepository.Table
                        join v in _warehouseRepository.Table on i.WareHouseId equals v.Id into vi
                        from iv in vi.DefaultIfEmpty()
                        select new Audit
                        {
                            Id = i.Id,
                            Voucher = i.Voucher,
                            VoucherCode = i.VoucherCode,
                            VoucherDate = i.VoucherDate,
                            WareHouseId = i.WareHouseId,
                            Description = i.Description,
                            CreatedDate = i.CreatedDate,
                            CreatedBy = i.CreatedBy,
                            ModifiedDate = i.ModifiedDate,
                            ModifiedBy = i.ModifiedBy,

                            WareHouse = iv == null ? null : new WareHouse
                            {
                                Id = iv.Id,
                                Code = iv.Code,
                                Name = iv.Name
                            }
                        };

            if (ctx.Keywords.HasValue())
            {
                query = query
                    .Where(x => x.VoucherCode.Contains(ctx.Keywords)
                             || x.CreatedBy.Contains(ctx.Keywords)
                             || x.Description.Contains(ctx.Keywords))
                    .Distinct();
            }
            if (ctx.WareHouesId.HasValue())
            {
                var department = _warehouseRepository.Table.FirstOrDefault(x => x.Id == ctx.WareHouesId);

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
                        await _auditCouncilRepository.DataConnection.QueryToListAsync<string>(GetListChidren.ToString());
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

                var departmentIds = await _auditRepository.DataConnection.QueryToListAsync<string>(sb.ToString());
                if (departmentIds?.ToList().Count > 0)
                {
                    var listDepartmentIds = departmentIds.ToList();
                    query = from p in query where listDepartmentIds.Contains(p.WareHouseId) select p;
                }
                else
                    return new PagedList<Audit>(new List<Audit>(), ctx.PageIndex, ctx.PageSize);

            }
            if (!string.IsNullOrEmpty(ctx.EmployeeId))
            {
                query = from p in query
                        where p.CreatedBy == ctx.EmployeeId
                        select p;
            }

            if (ctx.FromDate.HasValue)
            {
                query = query
                    .Where(x => x.VoucherDate >= ctx.FromDate.Value)
                    .Distinct();
            }

            if (ctx.ToDate.HasValue)
            {
                query = query
                    .Where(x => x.VoucherDate <= ctx.ToDate.Value)
                    .Distinct();
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

            query = from p in query orderby p.VoucherCode select p;


            return new PagedList<Audit>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IPagedList<Audit> Get(AuditSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _auditRepository.Table select p;

            if (ctx.Keywords.HasValue())
            {
                query = query
                    .Where(x => x.VoucherCode.Contains(ctx.Keywords)
                             || x.Voucher.Contains(ctx.Keywords)
                             || x.CreatedBy.Contains(ctx.Keywords))
                    .Distinct();
            }

            if (ctx.FromDate.HasValue)
            {
                query = query
                    .Where(x => x.VoucherDate >= ctx.FromDate.Value)
                    .Distinct();
            }

            if (ctx.ToDate.HasValue)
            {
                query = query
                    .Where(x => x.VoucherDate <= ctx.ToDate.Value)
                    .Distinct();
            }

            query = from p in query orderby p.VoucherCode select p;

            return new PagedList<Audit>(query, ctx.PageIndex, ctx.PageSize);
        }

        #endregion

    }
}

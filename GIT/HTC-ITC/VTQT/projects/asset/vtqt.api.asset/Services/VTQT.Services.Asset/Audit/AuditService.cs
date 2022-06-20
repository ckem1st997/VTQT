using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using VTQT.Core;
using VTQT.Core.Data;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Domain.FbmOrganization;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Asset
{
    public partial class AuditService : IAuditService
    {
        #region Fields

        private readonly IRepository<Audit> _auditRepository;
        private readonly IRepository<AuditDetail> _auditDetailRepository;
        private readonly IRepository<AuditCouncil> _auditCouncilRepository;
        private readonly IIntRepository<OrganizationUnit> _organizationRepository;
        private readonly IRepository<AuditDetailSerial> _audiDetailSerialRepository;

        #endregion Fields

        #region Ctor

        public AuditService()
        {
            _auditDetailRepository = EngineContext.Current.Resolve<IRepository<AuditDetail>>(DataConnectionHelper.ConnectionStringNames.Asset);
            _auditCouncilRepository = EngineContext.Current.Resolve<IRepository<AuditCouncil>>(DataConnectionHelper.ConnectionStringNames.Asset);
            _auditRepository = EngineContext.Current.Resolve<IRepository<Audit>>(DataConnectionHelper.ConnectionStringNames.Asset);
            _organizationRepository = EngineContext.Current.Resolve<IIntRepository<OrganizationUnit>>(DataConnectionHelper.ConnectionStringNames.FbmOrganization);
            _audiDetailSerialRepository = EngineContext.Current.Resolve<IRepository<AuditDetailSerial>>(DataConnectionHelper.ConnectionStringNames.Asset);

        }

        #endregion Ctor

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
                    .Where(w => w.FK_AuditDetail_Id_BackReferences != null && w.FK_AuditDetail_Id_BackReferences.Any())
                    .SelectMany(s => s.FK_AuditDetail_Id_BackReferences).ToList();
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

        #endregion Methods

        #region List
        public IList<OrganizationUnit> GetAll()
        {
            var query = from p in _organizationRepository.Table
                        select p;

            query = from p in query
                    orderby p.Name
                    select p;

            return query.ToList();
        }


        public IPagedList<Audit> GetListShowName(AuditSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _auditRepository.Table
                        where i.AssetType == ctx.AssetType
                        select i;

            if (ctx.Keywords.HasValue())
            {
                query = query
                    .Where(x => x.VoucherCode.Contains(ctx.Keywords)
                             || x.CreatedBy.Contains(ctx.Keywords)
                             || x.Description.Contains(ctx.Keywords))
                    .Distinct();
            }
            if (!string.IsNullOrEmpty(ctx.OrganizationId))
            {
                var department = _organizationRepository.Table.FirstOrDefault(x => x.Id.ToString() == ctx.OrganizationId);

                if (!string.IsNullOrEmpty(department.TreePath))
                {
                    var departmentCodes = _organizationRepository.Table.Where(x => !string.IsNullOrEmpty(x.TreePath) &&
                                                                            x.TreePath.Contains(department.TreePath))
                                                                      .Select(x => x.Code);
                    var departmentIds = _organizationRepository.Table.Where(x => !string.IsNullOrEmpty(x.TreePath) &&
                                                                 x.TreePath.Contains(department.TreePath))
                                                           .Select(x => x.Id.ToString());
                    if (departmentIds?.ToList().Count > 0)
                    {
                        var listCode = departmentCodes.ToList();
                        var listId = departmentIds.ToList();

                        query = query.Where(x =>listCode.Contains(x.AuditLocation)||listId.Contains(x.AuditLocation));
                    }
                }
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

        #endregion List
    }
}
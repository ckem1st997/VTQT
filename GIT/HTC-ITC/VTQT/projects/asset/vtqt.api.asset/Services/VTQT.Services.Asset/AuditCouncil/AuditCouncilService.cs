using LinqToDB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using VTQT.Core.Data;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;

namespace VTQT.Services.Asset
{
    public partial class AuditCouncilService : IAuditCouncilService
    {
        #region Fields
        private readonly IRepository<AuditCouncil> _auditCouncilRepository;
        #endregion

        #region Ctor
        public AuditCouncilService()
        {
            _auditCouncilRepository = EngineContext.Current.Resolve<IRepository<AuditCouncil>>(DataConnectionHelper.ConnectionStringNames.Asset);
        }
        #endregion

        #region Methods
        public async Task InsertAsync(AuditCouncil entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = TransactionConfig.IsolationLevel },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var db = _auditCouncilRepository.DataConnection;

                await db.InsertAsync(entity);

                transaction.Complete();
            }
        }

        public async Task UpdateAsync(AuditCouncil entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = TransactionConfig.IsolationLevel },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var db = _auditCouncilRepository.DataConnection;

                await db.UpdateAsync(entity);

                transaction.Complete();
            }
        }

        public async Task DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            await _auditCouncilRepository.DeleteAsync(ids);
        }

        public virtual IList<AuditCouncil> GetByAuditCouncilId(AuditCouncilSearchContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.AuditId))
                throw new ArgumentNullException(nameof(ctx.AuditId));

            var query =
              from x in _auditCouncilRepository.Table
              where x.AuditId == ctx.AuditId
              select x;

            return query.ToList();
        }

        public async Task<AuditCouncil> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _auditCouncilRepository.GetByIdAsync(id);

            return result;
        }
        public async Task<bool> ExistsAsync(string employeeId, string auditId)
        {
            return await _auditCouncilRepository.Table
                .AnyAsync(x => !string.IsNullOrEmpty(x.EmployeeId)
                && x.EmployeeId.Equals(employeeId)
                && !string.IsNullOrEmpty(x.AuditId)
                && x.AuditId.Equals(auditId)
                && x.EmployeeId == employeeId);
        }

        public IQueryable<AuditCouncil> GetListById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            var result = from p in _auditCouncilRepository.Table where p.AuditId.Equals(id.Trim()) select p;

            return result;
        }

        public IQueryable<AuditCouncil> GetListShowNameById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            var query = from i in _auditCouncilRepository.Table

                        select new AuditCouncil
                        {
                            Id = i.Id,
                            AuditId = i.AuditId,
                            EmployeeId = i.EmployeeId,
                            EmployeeName = i.EmployeeName,
                            Role = i.Role,
                        };
            var result = from p in query where p.AuditId.Equals(id.Trim()) select p;

            return result;
        }
        public async Task DeletesAsync(IEnumerable<AuditCouncil> auditCouncils)
        {
            if (auditCouncils == null)
            {
                throw new ArgumentNullException(nameof(auditCouncils));
            }

            await _auditCouncilRepository.DeleteAsync(auditCouncils);
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class OutWardService : IOutWardService
    {
        #region Fields

        private readonly IRepository<Outward> _outwardRepository;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor

        public OutWardService(
            IWorkContext workContext)
        {
            _outwardRepository = EngineContext.Current.Resolve<IRepository<Outward>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public virtual async Task InsertAsync(Outward entity, IList<OutwardDetail> details = null)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // TODO-XBase-API-Authen
            var utcNow = DateTime.UtcNow;
            //entity.CreatedBy = _workContext.UserId;
            entity.CreatedDate = utcNow;
            //entity.ModifiedBy = _workContext.UserId;
            entity.ModifiedDate = utcNow;

            var serials = new List<SerialWareHouse>();
            if (details != null && details.Any())
            {
                serials = details
                    .Where(w => w.SerialWareHouses != null && w.SerialWareHouses.Any())
                    .SelectMany(s => s.SerialWareHouses).ToList();
            }

            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = TransactionConfig.IsolationLevel },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var db = _outwardRepository.DataConnection;

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

                transaction.Complete();
            }
        }

        public virtual async Task UpdateAsync(Outward entity, IList<OutwardDetail> details = null, IList<string> deleteDetailIds = null)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // TODO-XBase-API-Authen
            var utcNow = DateTime.UtcNow;
            entity.ModifiedBy = _workContext.UserId;
            entity.ModifiedDate = utcNow;

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _outwardRepository.UpdateAsync(entity);
        }

        public virtual async Task DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            // Delete Cascade: Inward > InwardDetail > SerialWarehouse
            await _outwardRepository.DeleteAsync(ids);
        }

        public virtual async Task<Outward> GetByIdAsync(string id)
        {
            if (id.IsEmpty())
                throw new ArgumentException(nameof(id));

            return await _outwardRepository.GetByIdAsync(id);
        }

        public virtual async Task<bool> ExistsAsync(string code)
        {
            return await _outwardRepository.Table
                .AnyAsync(a =>
                    !string.IsNullOrEmpty(a.VoucherCode)
                    && a.VoucherCode.Equals(code)
                );
        }

        public virtual async Task<bool> ExistsAsync(string oldCode, string newCode)
        {
            return await _outwardRepository.Table
                .AnyAsync(a =>
                    !string.IsNullOrEmpty(a.VoucherCode)
                    && a.VoucherCode.Equals(newCode)
                    && !a.VoucherCode.Equals(oldCode)
                );
        }
        #endregion
    }
}

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
    public partial class InwardService : IInwardService
    {
        #region Constants



        #endregion

        #region Fields

        private readonly IRepository<Inward> _inwardRepository;
        private readonly IWorkContext _workContext;
        private readonly IRepository<InwardDetail> _inwardDetailRepository;
        private readonly IRepository<BeginningWareHouse> _beginningWareHouseRepository;
        #endregion

        #region Ctor

        public InwardService(
            IWorkContext workContext)
        {
            _inwardRepository = EngineContext.Current.Resolve<IRepository<Inward>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _inwardDetailRepository = EngineContext.Current.Resolve<IRepository<InwardDetail>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _beginningWareHouseRepository = EngineContext.Current.Resolve<IRepository<BeginningWareHouse>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public virtual async Task InsertAsync(Inward entity, IList<InwardDetail> details = null)
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
                var db = _inwardRepository.DataConnection;

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

        public virtual async Task UpdateAsync(Inward entity, IList<InwardDetail> details = null, IList<string> deleteDetailIds = null)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // TODO-XBase-API-Authen
            var utcNow = DateTime.UtcNow;
            entity.ModifiedBy = _workContext.UserId;
            entity.ModifiedDate = utcNow;

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _inwardRepository.UpdateAsync(entity);
        }

        public virtual async Task DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            // Delete Cascade: Inward > InwardDetail > SerialWarehouse
            await _inwardRepository.DeleteAsync(ids);
        }

        public virtual async Task<Inward> GetByIdAsync(string id)
        {
            if (id.IsEmpty())
                throw new ArgumentException(nameof(id));

            return await _inwardRepository.GetByIdAsync(id);
        }

        public virtual async Task<bool> ExistsAsync(string code)
        {
            return await _inwardRepository.Table
                .AnyAsync(a =>
                    !string.IsNullOrEmpty(a.VoucherCode)
                    && a.VoucherCode.Equals(code)
                );
        }

        public virtual async Task<bool> ExistsAsync(string oldCode, string newCode)
        {
            return await _inwardRepository.Table
                .AnyAsync(a =>
                    !string.IsNullOrEmpty(a.VoucherCode)
                    && a.VoucherCode.Equals(newCode)
                    && !a.VoucherCode.Equals(oldCode)
                );
        }

        public bool CheckItemExistAsync(string itemId, string warehouseId)
        {
            if (string.IsNullOrEmpty(itemId) || string.IsNullOrEmpty(warehouseId))
            {
                throw new ArgumentNullException(itemId);
            }

            var beginning = _beginningWareHouseRepository.Table.FirstOrDefault(x => x.WareHouseId == warehouseId && x.ItemId == itemId);

            if (beginning != null)
            {
                return true;
            }
            else
            {
                var inward = _inwardRepository.Table.FirstOrDefault(x => x.WareHouseID == warehouseId);
                if (inward != null)
                {
                    var inwardDetail = _inwardDetailRepository.Table.FirstOrDefault(x => x.ItemId == itemId);
                    if (inwardDetail != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion
    }
}

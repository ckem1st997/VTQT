using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using LinqToDB;
using LinqToDB.Data;
using VTQT.Core.Data;
using VTQT.Core.Domain.Warehouse;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Warehouse
{
    public partial class InwardDetailService : IInwardDetailService
    {
        #region Fields

        private readonly IRepository<InwardDetail> _inwardDetailRepository;
        private readonly IRepository<Unit> _unitRepository;
        private readonly IRepository<WareHouseItem> _wareHouseItemRepository;
        private readonly IRepository<WareHouseItemCategory> _wareHouseItemCategoryRepository;

        #endregion

        #region Ctor

        public InwardDetailService()
        {
            _inwardDetailRepository = EngineContext.Current.Resolve<IRepository<InwardDetail>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _unitRepository = EngineContext.Current.Resolve<IRepository<Unit>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _wareHouseItemRepository = EngineContext.Current.Resolve<IRepository<WareHouseItem>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _wareHouseItemCategoryRepository = EngineContext.Current.Resolve<IRepository<WareHouseItemCategory>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
        }

        #endregion

        #region Methods

        public async Task InsertAsync(InwardDetail entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = TransactionConfig.IsolationLevel },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var db = _inwardDetailRepository.DataConnection;

                await db.InsertAsync(entity);

                if (entity.SerialWareHouses != null && entity.SerialWareHouses.Any())
                {
                    var bulkSerialResult = await db.BulkCopyAsync(new BulkCopyOptions(), entity.SerialWareHouses);
                }

                transaction.Complete();
            }
        }

        public async Task UpdateAsync(InwardDetail entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = TransactionConfig.IsolationLevel },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var db = _inwardDetailRepository.DataConnection;

                await db.UpdateAsync(entity);

                await db.GetTable<SerialWareHouse>()
                    .Where(w => w.InwardDetailId == entity.Id)
                    .DeleteAsync();

                if (entity.SerialWareHouses != null && entity.SerialWareHouses.Any())
                {
                    var bulkSerialResult = await db.BulkCopyAsync(new BulkCopyOptions(), entity.SerialWareHouses);
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

            await _inwardDetailRepository.DeleteAsync(ids);
        }

        public virtual IList<InwardDetail> GetByInwardId(InwardDetailSearchContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.InwardId))
                throw new ArgumentNullException(nameof(ctx.InwardId));

            var query =
                from x in _inwardDetailRepository.Table
                join i in _wareHouseItemRepository.Table on x.ItemId equals i.Id into ij
                from i in ij
                where x.InwardId == ctx.InwardId
                orderby i.Code
                select x;

            return query.ToList();
        }

        public async Task<InwardDetail> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _inwardDetailRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<bool> ExistsAsync(string itemId, string inwardId,
            string departmentId, string employeeId, string stationId,
            string projectId, string customerId)
        {
            return await _inwardDetailRepository.Table
                .AnyAsync(x => !string.IsNullOrEmpty(x.ItemId)
                && x.ItemId.Equals(itemId)
                && !string.IsNullOrEmpty(x.InwardId)
                && x.InwardId.Equals(inwardId)
                && x.DepartmentId == departmentId
                && x.EmployeeId == employeeId
                && x.StationId == stationId
                && x.ProjectId == projectId
                && x.CustomerId == customerId);
        }

        public IQueryable<InwardDetail> GetListById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            var result = from p in _inwardDetailRepository.Table where p.InwardId.Equals(id.Trim()) select p;

            return result;
        }

        public IQueryable<InwardDetail> GetListShowNameById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            var query = from i in _inwardDetailRepository.Table
                        join u in _unitRepository.Table on i.UnitId equals u.Id into ui
                        join v in _wareHouseItemRepository.Table on i.ItemId equals v.Id into vi
                        from iu in ui.DefaultIfEmpty()
                        from iv in vi.DefaultIfEmpty()
                        select new InwardDetail
                        {
                            Id = i.Id,
                            ItemId = i.ItemId,
                            UnitId = i.UnitId,
                            UIQuantity = i.UIQuantity,
                            UIPrice = i.UIPrice,
                            StationName = i.StationName,
                            ProjectId = i.ProjectId,
                            StationId = i.StationId,
                            Quantity = i.Quantity,
                            InwardId = i.InwardId,
                            ProjectName = i.ProjectName,
                            EmployeeId = i.EmployeeId,
                            CustomerName = i.CustomerName,
                            Amount = i.Amount,
                            CustomerId = i.CustomerId,
                            DepartmentId = i.DepartmentId,
                            DepartmentName = i.DepartmentName,
                            EmployeeName = i.EmployeeName,
                            Price = i.Price,
                            Unit = iu == null ? null : new Unit
                            {
                                Id = iu.Id,
                                UnitName = iu.UnitName
                            },
                            WareHouseItem = iv == null ? null : new WareHouseItem
                            {
                                Id = iv.Id,
                                Code = iv.Code,
                                Name = iv.Name
                            }
                        };
            var result = from p in query where p.InwardId.Equals(id.Trim()) select p;

            return result;
        }


        public async Task DeletesAsync(IEnumerable<InwardDetail> inwardDetails)
        {
            if (inwardDetails == null)
            {
                throw new ArgumentNullException(nameof(inwardDetails));
            }

            await _inwardDetailRepository.DeleteAsync(inwardDetails);
        }

        public IList<InwardDetail> GetSelect()
        {
            var query = from p in _inwardDetailRepository.Table select p;

            return query.ToList();
        }

        #endregion
    }
}

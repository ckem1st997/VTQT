using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public partial class OutwardDetailService : IOutwardDetailService
    {
        #region Fields

        private readonly IRepository<OutwardDetail> _outwardDetailRepository;
        private readonly IRepository<Unit> _unitRepository;
        private readonly IRepository<WareHouseItem> _wareHouseItemRepository;
        private readonly IRepository<WareHouseItemCategory> _wareHouseItemCategoryRepository;

        #endregion

        #region Ctor

        public OutwardDetailService(
            )
        {
            _outwardDetailRepository = EngineContext.Current.Resolve<IRepository<OutwardDetail>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _unitRepository = EngineContext.Current.Resolve<IRepository<Unit>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _wareHouseItemRepository = EngineContext.Current.Resolve<IRepository<WareHouseItem>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _wareHouseItemCategoryRepository = EngineContext.Current.Resolve<IRepository<WareHouseItemCategory>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
        }

        #endregion

        #region Methods

        public async Task InsertAsync(OutwardDetail entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = TransactionConfig.IsolationLevel },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var db = _outwardDetailRepository.DataConnection;

                await db.InsertAsync(entity);

                if (entity.SerialWareHouses != null && entity.SerialWareHouses.Any())
                {
                    var bulkSerialResult = await db.BulkCopyAsync(new BulkCopyOptions(), entity.SerialWareHouses);
                }

                transaction.Complete();
            }
        }

        public async Task UpdateAsync(OutwardDetail entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = TransactionConfig.IsolationLevel },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var db = _outwardDetailRepository.DataConnection;

                await db.UpdateAsync(entity);

                await db.GetTable<SerialWareHouse>()
                    .Where(w => w.OutwardDetailId == entity.Id)
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
                throw new ArgumentNullException(nameof(ids));

            await _outwardDetailRepository.DeleteAsync(ids);
        }

        public  virtual IList<OutwardDetail> GetByOutwardId(OutwardDetailSearchContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.OutwardId))
                throw new ArgumentNullException(nameof(ctx.OutwardId));

            var query =
                from x in _outwardDetailRepository.Table
                join i in _wareHouseItemRepository.Table on x.ItemId equals i.Id into ij
                from i in ij
                where x.OutwardId == ctx.OutwardId
                orderby i.Code
                select x;

            var result = query.ToList();

            return result;

            // BuildMyString.com generated code. Please enjoy your string responsibly.
            //StringBuilder sb = new StringBuilder();

            //sb.Append("SELECT ");
            //sb.Append("	`x`.`Id`, ");
            //sb.Append("	`x`.`AccObjectId`, ");
            //sb.Append("	`x`.`Status`, ");
            //sb.Append("	`x`.`AccountYes`, ");
            //sb.Append("	`x`.`AccountMore`, ");
            //sb.Append("	`x`.`CustomerName`, ");
            //sb.Append("	`x`.`CustomerId`, ");
            //sb.Append("	`x`.`ProjectName`, ");
            //sb.Append("	`x`.`ProjectId`, ");
            //sb.Append("	`x`.`StationName`, ");
            //sb.Append("	`x`.`StationId`, ");
            //sb.Append("	`x`.`EmployeeName`, ");
            //sb.Append("	`x`.`EmployeeId`, ");
            //sb.Append("	`x`.`DepartmentName`, ");
            //sb.Append("	`x`.`DepartmentId`, ");
            //sb.Append("	`x`.`Price`, ");
            //sb.Append("	`x`.`Quantity`, ");
            //sb.Append("	`x`.`Amount`, ");
            //sb.Append("	`x`.`UIPrice`, ");
            //sb.Append("	`x`.`UIQuantity`, ");
            //sb.Append("	`x`.`UnitId`, ");
            //sb.Append("	`x`.`ItemId`, ");
            //sb.Append("	`x`.`OutwardId` ");
            //sb.Append("FROM ");
            //sb.Append("	`OutwardDetail` `x` ");
            //sb.Append("		INNER JOIN `WareHouseItem` `ij` ON `x`.`ItemId` = `ij`.`Id` ");
            //sb.Append("WHERE ");
            //sb.Append("	`x`.`OutwardId` = @p1 ");
            //sb.Append("ORDER BY ");
            //sb.Append("	`ij`.`Code` ");
            //DataParameter p1 = new DataParameter("p1", ctx.OutwardId);

            //return await _outwardDetailRepository.DataConnection.QueryToListAsync<OutwardDetail>(sb.ToString(), p1);

        }

        public async Task<OutwardDetail> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _outwardDetailRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<bool> ExistsAsync(string itemId, string outwardId,
            string departmentId, string employeeId, string stationId,
            string projectId, string customerId)
        {
            return await _outwardDetailRepository.Table
                .AnyAsync(x =>
                    !string.IsNullOrEmpty(x.ItemId)
                    && x.ItemId.Equals(itemId)
                    && !string.IsNullOrEmpty(x.OutwardId)
                    && x.OutwardId.Equals(outwardId)
                    && x.DepartmentId == departmentId
                    && x.EmployeeId == employeeId
                    && x.StationId == stationId
                    && x.ProjectId == projectId
                    && x.CustomerId == customerId
                );
        }

        public IQueryable<OutwardDetail> GetListById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            var result = from p in _outwardDetailRepository.Table where p.OutwardId.Equals(id.Trim()) select p;

            return result;
        }

        public async Task DeletesAsync(IEnumerable<OutwardDetail> outwardDetails)
        {
            if (outwardDetails == null)
            {
                throw new ArgumentNullException(nameof(outwardDetails));
            }

            await _outwardDetailRepository.DeleteAsync(outwardDetails);
        }

        public IQueryable<OutwardDetail> GetListShowNameById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            var query = from i in _outwardDetailRepository.Table
                        join u in _unitRepository.Table on i.UnitId equals u.Id into ui
                        join v in _wareHouseItemRepository.Table on i.ItemId equals v.Id into vi
                        from iu in ui.DefaultIfEmpty()
                        from iv in vi.DefaultIfEmpty()
                        select new OutwardDetail
                        {
                            Id = i.Id,
                            OutwardId = i.OutwardId,
                            ItemId = i.ItemId,
                            UnitId = i.UnitId,
                            UIQuantity = i.UIQuantity,
                            UIPrice = i.UIPrice,
                            Amount = i.Amount,
                            StationName = i.StationName,
                            ProjectId = i.ProjectId,
                            StationId = i.StationId,
                            Quantity = i.Quantity,
                            ProjectName = i.ProjectName,
                            EmployeeId = i.EmployeeId,
                            CustomerName = i.CustomerName,
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
            var result = from p in query where p.OutwardId.Equals(id.Trim()) select p;

            return result;
        }

        public IList<OutwardDetail> GetSelect()
        {

            var query = from p in _outwardDetailRepository.Table select p;

            return query.ToList();
        }

        #endregion
    }
}
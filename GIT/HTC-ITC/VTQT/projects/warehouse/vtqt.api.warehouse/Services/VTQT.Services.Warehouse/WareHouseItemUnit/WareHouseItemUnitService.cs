using LinqToDB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using VTQT.Core.Data;
using VTQT.Core.Domain.Warehouse;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;
using LinqToDB.Data;
using VTQT.Core;

namespace VTQT.Services.Warehouse
{
    public partial class WareHouseItemUnitService : IWareHouseItemUnitService
    {
        #region Fields

        private readonly IRepository<WareHouseItemUnit> _repository;
        private readonly IRepository<Unit> _unitRepository;

        #endregion

        #region Ctor

        public WareHouseItemUnitService()
        {
            _repository =
                EngineContext.Current.Resolve<IRepository<WareHouseItemUnit>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _unitRepository =
                EngineContext.Current.Resolve<IRepository<Unit>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
        }

        #endregion

        #region Methods

        public async Task InsertAsync(WareHouseItemUnit entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = TransactionConfig.IsolationLevel },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var db = _repository.DataConnection;

                await db.InsertAsync(entity);

                transaction.Complete();
            }
        }

        public async Task<long> InsertAsync(IEnumerable<WareHouseItemUnit> entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            var res = await _repository.InsertAsync(entity);
            return res;
        }

        public async Task UpdateAsync(WareHouseItemUnit entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = TransactionConfig.IsolationLevel },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var db = _repository.DataConnection;

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

            await _repository.DeleteAsync(ids);
        }

        public virtual IList<WareHouseItemUnit> GetByWareHouseItemUnitId(WareHouseItemUnitSearchContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.ItemId))
                throw new ArgumentNullException(nameof(ctx.ItemId));

            var query =
                from x in _repository.Table
                where x.ItemId == ctx.ItemId
                select x;

            return query.ToList();
        }

        public async Task<WareHouseItemUnit> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _repository.GetByIdAsync(id);

            return result;
        }

        public async Task<bool> ExistsAsync(string unitId, string itemId)
        {
            return await _repository.Table
                .AnyAsync(x => !string.IsNullOrEmpty(x.UnitId)
                               && x.UnitId.Equals(itemId)
                               && !string.IsNullOrEmpty(x.ItemId)
                               && x.ItemId.Equals(itemId));
        }


        public bool Exists(string unitId, string itemId)
        {
            if (string.IsNullOrEmpty(itemId) || string.IsNullOrEmpty(unitId))
                throw new ArgumentNullException("Not Id To Search...");
            var sql = $"SELECT Id FROM WareHouseItemUnit whiu WHERE whiu.ItemId='{itemId}' && whiu.UnitId='{unitId}'";

            var count = _repository.DataConnection.Query<string>(sql).Count();
            return count > 0 ? true : false;
        }

        public IQueryable<WareHouseItemUnit> GetListById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = from p in _repository.Table where p.ItemId.Equals(id.Trim()) select p;

            return result;
        }

        public IQueryable<WareHouseItemUnit> GetListShowNameById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var query = from i in _repository.Table
                join u in _unitRepository.Table on i.UnitId equals u.Id into ui
                from iu in ui.DefaultIfEmpty()
                select new WareHouseItemUnit
                {
                    Id = i.Id,
                    ItemId = i.ItemId,
                    UnitId = i.UnitId,
                    ConvertRate = i.ConvertRate,

                    Unit = iu == null
                        ? null
                        : new Unit
                        {
                            Id = iu.Id,
                            UnitName = iu.UnitName
                        },
                };
            var result = from p in query where p.ItemId.Equals(id.Trim()) select p;

            return result;
        }


        public async Task DeletesAsync(IEnumerable<WareHouseItemUnit> wareHouseItemUnits)
        {
            if (wareHouseItemUnits == null)
            {
                throw new ArgumentNullException(nameof(wareHouseItemUnits));
            }

            await _repository.DeleteAsync(wareHouseItemUnits);
        }

        public IPagedList<WareHouseItemUnit> Get(WareHouseItemUnitPagingContext ctx)
        {
            var query = from p in _repository.Table select p;

            return new PagedList<WareHouseItemUnit>(query, ctx.PageIndex, ctx.PageSize);
        }

        public async Task<int> GetConvertRate(string idItem, string UnitId)
        {
            if (string.IsNullOrEmpty(idItem) || string.IsNullOrEmpty(UnitId))
                throw new ArgumentNullException();
            var query = (from p in _repository.Table
                where p.ItemId.Equals(idItem) && p.UnitId.Equals(UnitId)
                select p.ConvertRate).FirstOrDefaultAsync();
            return await query;
        }

        public IList<WareHouseItemUnit> GetAll(bool showHidden = false)
        {
            var query = from p in _repository.Table select p;
            return query.ToList();
        }

        #endregion
    }
}
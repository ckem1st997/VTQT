using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Services.Warehouse
{
    public partial class SerialWareHouseService : ISerialWareHouseService
    {
        #region Constants



        #endregion

        #region Fields

        private readonly IRepository<SerialWareHouse> _repository;

        #endregion

        #region Ctor

        public SerialWareHouseService()
        {
            _repository = EngineContext.Current.Resolve<IRepository<SerialWareHouse>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
        }

        #endregion

        #region Methods

        public virtual async Task<int> InsertAsync(SerialWareHouse entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (!string.IsNullOrEmpty(entity.OutwardDetailId))
                entity.IsOver = true;
            var result = await _repository.InsertAsync(entity);

            return result;
        }

        public virtual async Task<int> UpdateAsync(SerialWareHouse entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (!string.IsNullOrEmpty(entity.OutwardDetailId))
                entity.IsOver = true;
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

        public virtual IPagedList<SerialWareHouse> Get(SerialWareHouseSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _repository.Table select p;

            if (ctx.Keywords.HasValue())
            {
                query = query.Where(el =>
                el.Serial.Contains(ctx.Keywords) ||
                el.InwardDetailId.Contains(ctx.Keywords) ||
                el.OutwardDetailId.Contains(ctx.Keywords)).Select(el => el).Distinct();
            }
            return new PagedList<SerialWareHouse>(query, ctx.PageIndex, ctx.PageSize);
        }

        public virtual async Task<SerialWareHouse> GetByIdAsync(string id)
        {
            if (id.IsEmpty())
                throw new ArgumentException(nameof(id));

            return await _repository.GetByIdAsync(id);
        }

        public virtual async Task<bool> ExistsAsync(string serial)
        {
            return await _repository.Table
                .AnyAsync(a => !string.IsNullOrEmpty(a.Serial) && a.Serial.Equals(serial));
        }

        public virtual async Task<bool> ExistsAsync(string oldSerial, string newSerial)
        {
            return await _repository.Table
                .AnyAsync(a => !string.IsNullOrEmpty(a.Serial) && a.Serial.Equals(newSerial) && !a.Serial.Equals(oldSerial));
        }

        #endregion
    }
}


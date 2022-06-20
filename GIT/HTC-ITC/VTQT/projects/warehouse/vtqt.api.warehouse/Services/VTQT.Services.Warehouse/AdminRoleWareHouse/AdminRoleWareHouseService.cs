using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain;
using VTQT.Core.Domain.Warehouse;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Warehouse
{
    public class AdminRoleWareHouseService : IAdminRoleWareHouseService
    {
        #region Fields

        private readonly IRepository<AdminRoleWareHouse> _adminRoleWareHouseRepository;

        private readonly IWorkContext _workContext;

        #endregion Fields

        #region Ctor

        public AdminRoleWareHouseService(IWorkContext workContext)
        {
            _workContext = workContext;
            _adminRoleWareHouseRepository = EngineContext.Current.Resolve<IRepository<AdminRoleWareHouse>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
        }

        #endregion Ctor

        #region Methods

        public virtual async Task<int> InsertAsync(AdminRoleWareHouse entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            var utcNow = DateTime.UtcNow;
            entity.CreatedDate = utcNow;

            var result = await _adminRoleWareHouseRepository.InsertAsync(entity);

            return result;
        }

        public virtual async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _adminRoleWareHouseRepository.DeleteAsync(ids);

            return result;
        }

        public IList<AdminRoleWareHouse> GetAll(bool showHidden = false)
        {
            var query = from p in _adminRoleWareHouseRepository.Table select p;
            if (!showHidden)
            {
                query = from p in query where !p.Inactive select p;
            }
            query = from p in query orderby p.UserId select p;
            return query.ToList();
        }

        public IPagedList<AdminRoleWareHouse> Get(AdminRoleWareHouseContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _adminRoleWareHouseRepository.Table select p;

            if (ctx.Keywords.HasValue())
            {
                query = query.Where(x => x.UserId.Contains(ctx.Keywords));
            }
            if (ctx.Status == (int)ActiveStatus.Activated)
            {
                query = from p in query
                        where p.Inactive == false
                        select p;
            }
            if (ctx.Status == (int)ActiveStatus.Deactivated)
            {
                query = from p in query
                        where p.Inactive == true
                        select p;
            }

            query = from p in query
                    orderby p.UserId
                    select p;

            return new PagedList<AdminRoleWareHouse>(query, ctx.PageIndex, ctx.PageSize);
        }

        public virtual async Task<AdminRoleWareHouse> GetByIdAsync(string id)
        {
            if (id.IsEmpty())
                throw new ArgumentException(nameof(id));

            return await _adminRoleWareHouseRepository.GetByIdAsync(id);
        }

        public virtual async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _adminRoleWareHouseRepository.Table
                .Where(w => ids.Contains(w.Id))
                .Set(x => x.Inactive, !active)
                .UpdateAsync();

            return result;
        }

        #endregion Methods
    }
}
using System;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.FbmCrm;
using VTQT.Data;
using VTQT.Core.Infrastructure;
using System.Linq;
using System.Collections.Generic;
using VTQT.Caching;

namespace VTQT.Services.Master
{
    public partial class CustomerService : ICustomerService
    {
        #region Fields
        private readonly IIntRepository<ApplicationUser> _customerRepository;
        private readonly IXBaseCacheManager _cacheManager;
        #endregion

        #region Ctor
        public CustomerService(
            IXBaseCacheManager cacheManager)
        {
            _customerRepository = EngineContext.Current.Resolve<IIntRepository<ApplicationUser>>(DataConnectionHelper.ConnectionStringNames.FbmCrm);
            _cacheManager = cacheManager;
        }
        #endregion

        #region Methods

        public virtual IList<ApplicationUser> GetAll(bool showHidden = false)
        {
            var key = FbmCrmCacheKeys.Customers.AllCacheKey.FormatWith(showHidden);
            var entities = _cacheManager.HybridProvider.Get(key, () =>
            {
                var query = from x in _customerRepository.Table select x;
                if (!showHidden)
                    query =
                        from x in query
                        where x.IsActive
                        select x;

                query =
                    from x in query
                    orderby x.CustomerCode
                    select x;

                return query.ToList();
            }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;

            return entities;
        }

        public IPagedList<ApplicationUser> Get(CustomerSearchContext ctx)
        {
            var query = from p in _customerRepository.Table select p;

            ctx.Keywords = ctx.Keywords?.Trim();

            if (ctx.Keywords != null && ctx.Keywords.HasValue())
            {
                query = from p in query 
                        where (p.FullName != null && p.FullName.Contains(ctx.Keywords)) ||
                              (p.CustomerCode != null && p.CustomerCode.Contains(ctx.Keywords)) 
                        select p;
            }

            query = from p in query orderby p.CustomerCode select p;

            return new PagedList<ApplicationUser>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<ApplicationUser> GetAvailable()
        {
            var query = from p in _customerRepository.Table select p;

            query = from p in query
                    orderby p.CustomerCode
                    select p;

            return query.ToList();
        }

        public async Task<ApplicationUser> GetByIdAsync(int id)
        {                        
            var result = await _customerRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<ApplicationUser> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var entity = await _customerRepository.Table.FirstOrDefaultAsync(x => x.CustomerCode == code);

            return entity;
        }
        #endregion
    }
}

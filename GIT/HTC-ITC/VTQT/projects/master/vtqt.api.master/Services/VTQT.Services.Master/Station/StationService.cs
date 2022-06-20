using System;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Qlsc;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;
using System.Collections.Generic;
using VTQT.Caching;

namespace VTQT.Services.Master
{
    public partial class StationService : IStationService
    {
        #region Fields
        private readonly IIntLowercaseRepository<danh_sach_tram> _stationRepository;
        private readonly IXBaseCacheManager _cacheManager;
        #endregion

        #region Ctor
        public StationService(
            IXBaseCacheManager cacheManager)
        {
            _stationRepository = EngineContext.Current.Resolve<IIntLowercaseRepository<danh_sach_tram>>(DataConnectionHelper.ConnectionStringNames.Qlsc);
            _cacheManager = cacheManager;
        }
        #endregion

        #region Methods

        public virtual IList<danh_sach_tram> GetAll()
        {
            var key = QlscCacheKeys.Tram.AllCacheKey;
            var entities = _cacheManager.HybridProvider.Get(key, () =>
            {
                var query =
                    from x in _stationRepository.Table
                    orderby x.loai_tram, x.phan_cap, x.ma_tram
                    select x;

                return query.ToList();
            }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;

            return entities;
        }

        public IPagedList<danh_sach_tram> Get(StationSearchContext ctx)
        {
            var query = from p in _stationRepository.Table select p;

            ctx.Keywords = ctx.Keywords?.Trim();

            if (ctx.Keywords != null && ctx.Keywords.HasValue())
            {
                query = from p in query
                        where (p.ma_tram != null && p.ma_tram.Contains(ctx.Keywords)) ||
                              (p.ten_tram != null && p.ten_tram.Contains(ctx.Keywords))
                        select p;
            }

            query = from p in query orderby p.ma_tram select p;

            return new PagedList<danh_sach_tram>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<danh_sach_tram> GetAvailable()
        {
            var query = from p in _stationRepository.Table select p;

            query = from p in query
                    orderby p.ma_tram
                    select p;

            return query.ToList();
        }

        public async Task<danh_sach_tram> GetByIdAsync(int id)
        {
            var result = await _stationRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<danh_sach_tram> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = await _stationRepository.Table.FirstOrDefaultAsync(x => x.ma_tram == code || x.ma_tram_vnm == code);

            return result;
        }
        #endregion
    }
}
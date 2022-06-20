using EasyCaching.Core;
using System;
using System.Threading.Tasks;

namespace VTQT.Caching
{
    public partial class XBaseCacheManager : IXBaseCacheManager
    {
        public IHybridCachingProvider HybridProvider { get; }

        public XBaseCacheManager(
            IHybridCachingProvider hybridProvider)
        {
            HybridProvider = hybridProvider;
        }

        public virtual T GetDb<T>(string key, Func<T> acquirer, int? cacheTime = null)
        {
            var duration = cacheTime.HasValue && cacheTime.Value > 0
                ? TimeSpan.FromMinutes(cacheTime.Value)
                : TimeSpan.FromMinutes(CachingDefaults.CacheTime);
            var cacheValue = HybridProvider.Get(key, acquirer, duration);
            if (cacheValue.HasValue)
                return cacheValue.Value;

            var result = acquirer();
            return result;
        }

        public virtual async Task<T> GetDbAsync<T>(string key, Func<Task<T>> acquirer, int? cacheTime = null)
        {
            var duration = cacheTime.HasValue && cacheTime.Value > 0
                ? TimeSpan.FromMinutes(cacheTime.Value)
                : TimeSpan.FromMinutes(CachingDefaults.CacheTime);
            var cacheValue = await HybridProvider.GetAsync(key, acquirer, duration);
            if (cacheValue.HasValue)
                return cacheValue.Value;

            var result = await acquirer();
            return result;
        }
    }
}

using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Caching.Helpers.CacheKeys;
using VTQT.Core.Domain.Master;
using VTQT.Core.Infrastructure;
using VTQT.Services.Caching;

namespace VTQT.Services.Apps.Caching
{
    /// <summary>
    /// Represents a app mapping cache event consumer
    /// </summary>
    public partial class AppMappingServiceCacheEventConsumer : ServiceCacheEventConsumer<AppMapping>
    {
        public AppMappingServiceCacheEventConsumer()
            : base(EngineContext.Current.Resolve<IXBaseCacheManager>())
        {

        }
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(AppMapping entity)
        {
            //await RemoveAsync(AppCacheKeys.AppMappingsCacheKey.FormatWith(entity.EntityId, entity.EntityName));
            //await RemoveAsync(AppCacheKeys.AppMappingIdsCacheKey.FormatWith(entity.EntityId, entity.EntityName));
            //await RemoveAsync(AppCacheKeys.AppMappingExistsCacheKey.FormatWith(entity.EntityName));
            await RemoveAsync(AppCacheKeys.AppMappingsAllPrefix);
        }
    }
}

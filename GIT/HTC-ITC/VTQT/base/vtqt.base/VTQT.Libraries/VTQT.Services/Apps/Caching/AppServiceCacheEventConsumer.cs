using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Core.Domain.Master;
using VTQT.Core.Infrastructure;
using VTQT.Services.Caching;

namespace VTQT.Services.Apps.Caching
{
    /// <summary>
    /// Represents a store cache event consumer
    /// </summary>
    public partial class AppServiceCacheEventConsumer : ServiceCacheEventConsumer<App>
    {
        public AppServiceCacheEventConsumer()
            : base(EngineContext.Current.Resolve<IXBaseCacheManager>())
        {

        }

        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(App entity)
        {
            await RemoveByPrefixAsync(LocalizationCacheKeys.LanguagesByAppPrefix.FormatWith(entity.Id));
        }
    }
}

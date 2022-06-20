using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Core.Domain.Master;
using VTQT.Core.Infrastructure;
using VTQT.Services.Caching;

namespace VTQT.Services.Localization.Caching
{
    /// <summary>
    /// Represents a localized property cache event consumer
    /// </summary>
    public partial class LocalizedPropertyServiceCacheEventConsumer : ServiceCacheEventConsumer<LocalizedProperty>
    {
        public LocalizedPropertyServiceCacheEventConsumer()
            : base(EngineContext.Current.Resolve<IXBaseCacheManager>())
        {

        }

        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(LocalizedProperty entity)
        {
            var cacheKey = LocalizationCacheKeys.LocalizedPropertyCacheKey.FormatWith(
                entity.LanguageId, entity.EntityId, entity.LocaleKeyGroup, entity.LocaleKey);

            await RemoveAsync(cacheKey);
        }
    }
}

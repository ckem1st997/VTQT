using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Core.Domain.Master;
using VTQT.Core.Infrastructure;
using VTQT.Services.Caching;

namespace VTQT.Services.Localization.Caching
{
    /// <summary>
    /// Represents a language cache event consumer
    /// </summary>
    public partial class LanguageServiceCacheEventConsumer : ServiceCacheEventConsumer<Language>
    {
        public LanguageServiceCacheEventConsumer()
            : base(EngineContext.Current.Resolve<IXBaseCacheManager>())
        {

        }

        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(Language entity)
        {
            await RemoveAsync(LocalizationCacheKeys.LocaleStringResourcesAllCacheKey.FormatWith(entity.Id));

            var prefix = LocalizationCacheKeys.LocaleStringResourcesByNamePrefix.FormatWith(entity.Id);
            await RemoveByPrefixAsync(prefix);

            await RemoveByPrefixAsync(LocalizationCacheKeys.LanguagesAllPrefix);
        }
    }
}

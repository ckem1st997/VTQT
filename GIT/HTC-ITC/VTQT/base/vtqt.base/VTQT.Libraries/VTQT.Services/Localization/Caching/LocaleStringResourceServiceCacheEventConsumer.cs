using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Core.Domain.Master;
using VTQT.Core.Infrastructure;
using VTQT.Services.Caching;

namespace VTQT.Services.Localization.Caching
{
    /// <summary>
    /// Represents a locale string resource cache event consumer
    /// </summary>
    public partial class LocaleStringResourceServiceCacheEventConsumer : ServiceCacheEventConsumer<LocaleStringResource>
    {
        public LocaleStringResourceServiceCacheEventConsumer()
            : base(EngineContext.Current.Resolve<IXBaseCacheManager>())
        {

        }

        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(LocaleStringResource entity)
        {
            await RemoveAsync(LocalizationCacheKeys.LocaleStringResourcesAllCacheKey.FormatWith(entity.LanguageId));

            //await RemoveByPrefixAsync(LocalizationCacheKeys.LocaleStringResourcesByNamePrefix.FormatWith(entity.LanguageId));
            await RemoveByPrefixAsync(LocalizationCacheKeys.LocaleStringResourcesAllPrefix);
        }
    }
}

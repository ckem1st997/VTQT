using VTQT.Core.Domain.Localization;
using VTQT.Core.Infrastructure;

namespace VTQT.Web.Framework.Routing
{
    /// <summary>
    /// Represents base provider
    /// </summary>
    public class BaseRouteProvider
    {
        /// <summary>
        /// Get pattern used to detect routes with language code
        /// </summary>
        /// <returns></returns>
        protected string GetLanguageRoutePattern()
        {
            var localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();
            if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                //this pattern is set once at the application start, when we don't have the selected language yet
                //so we use 'en' by default for the language value, later it'll be replaced with the working language code
                var code = "en";
                return $"{{{XBasePathRouteDefaults.LanguageRouteValue}:maxlength(2):{XBasePathRouteDefaults.LanguageParameterTransformer}={code}}}";
            }

            return string.Empty;
        }
    }
}

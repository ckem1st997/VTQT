using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using VTQT.Core;
using VTQT.Core.Infrastructure;

namespace VTQT.Web.Framework.Globalization
{
    /// <summary>
    /// Determines the culture information for a request
    /// </summary>
    public class XBaseRequestCultureProvider : RequestCultureProvider
    {
        public XBaseRequestCultureProvider(RequestLocalizationOptions options)
        {
            Options = options;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            //set working language culture
            var culture = EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage.LanguageCulture;

            return new ProviderCultureResult(culture, culture);
        }
    }
}

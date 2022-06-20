using System.Globalization;
using FluentValidation.Resources;
using VTQT.Core.Infrastructure;
using VTQT.Services;

namespace VTQT.Web.Framework.Validators
{
    public class ValidatorLanguageManager : LanguageManager
    {
        public override string GetString(string key, CultureInfo culture = null)
        {
            string result = base.GetString(key, culture);

            // (Perf) although FV expects a culture parameter, we gonna ignore it.
            // It's highly unlikely that it is anything different than our WorkingLanguage.
            var services = EngineContext.Current.Resolve<ICommonServices>();
            result = services.Localization.GetResource("Validation." + key, logIfNotFound: false, defaultValue: result, returnEmptyIfNotFound: true);

            return result;
        }
    }
}

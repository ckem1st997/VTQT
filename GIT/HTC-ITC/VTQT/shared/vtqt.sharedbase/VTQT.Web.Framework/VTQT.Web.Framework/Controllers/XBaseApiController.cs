using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VTQT.Core;
using VTQT.Core.Infrastructure;
using VTQT.Core.Localization;
using VTQT.Core.Logging;
using VTQT.Services;
using VTQT.Services.Localization;
using VTQT.Web.Framework.Modelling;

namespace VTQT.Web.Framework.Controllers
{
    public abstract partial class XBaseApiController : ControllerBase
    {
        protected XBaseApiController()
        {
        }

        public ILogger Logger
        {
            get;
            set;
        }

        public Localizer T
        {
            get;
            set;
        } = NullLocalizer.Instance;

        public ICommonServices Services
        {
            get;
            set;
        }

        #region Localization (Sync with XBaseMvcController)

        /// <summary>
        /// Add locales for localizable entities
        /// </summary>
        /// <typeparam name="T">Localizable model</typeparam>
        /// <param name="languageService">Language service</param>
        /// <param name="locales">Locales</param>
        /// <param name="localeLabels">Locale labels</param>
        /// <param name="actionLabels">(labels, languageId)</param>
        protected virtual void AddApiLocales<T>(ILanguageService languageService, IList<T> locales, IDictionary<string, string> localeLabels,
            Action<IDictionary<string, string>, string> actionLabels)
            where T : ILocalizedApiLocaleModel
        {
            AddApiLocales(languageService, locales, localeLabels, actionLabels, null);
        }

        /// <summary>
        /// Add locales for localizable entities
        /// </summary>
        /// <typeparam name="T">Localizable model</typeparam>
        /// <param name="languageService">Language service</param>
        /// <param name="locales">Locales</param>
        /// <param name="localeLabels">Locale labels</param>
        /// <param name="actionLabels">(labels, languageId)</param>
        /// <param name="configure">Configure action (locale, languageId)</param>
        protected virtual void AddApiLocales<T>(ILanguageService languageService, IList<T> locales, IDictionary<string, string> localeLabels,
            Action<IDictionary<string, string>, string> actionLabels, Action<T, string> configure)
            where T : ILocalizedApiLocaleModel
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var languages = languageService.GetAllLanguages(true);
            var curLanguageId = workContext.LanguageId;
            if (string.IsNullOrWhiteSpace(curLanguageId))
                curLanguageId = languages.FirstOrDefault()?.Id;
            foreach (var language in languages)
            {
                var cultureParts = language.LanguageCulture.Split('-');

                var locale = Activator.CreateInstance<T>();
                locale.LanguageId = language.Id;
                locale._LanguageCode = cultureParts[0];
                locale._FlagCode = cultureParts[1];
                if (configure != null)
                {
                    configure.Invoke(locale, locale.LanguageId);
                }
                locales.Add(locale);
            }

            actionLabels.Invoke(localeLabels, curLanguageId);
        }

        /// <summary>
        /// Add locales for localizable entities
        /// </summary>
        /// <typeparam name="TLocalizedModelLocal">Localizable model</typeparam>
        /// <param name="languageService">Language service</param>
        /// <param name="locales">Locales</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual void AddMvcLocales<TLocalizedMvcModelLocal>(ILanguageService languageService,
            IList<TLocalizedMvcModelLocal> locales) where TLocalizedMvcModelLocal : ILocalizedMvcLocaleModel
        {
            AddMvcLocales(languageService, locales, null);
        }

        /// <summary>
        /// Add locales for localizable entities
        /// </summary>
        /// <typeparam name="TLocalizedModelLocal">Localizable model</typeparam>
        /// <param name="languageService">Language service</param>
        /// <param name="locales">Locales</param>
        /// <param name="configure">Configure action</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual void AddMvcLocales<TLocalizedMvcModelLocal>(ILanguageService languageService,
            IList<TLocalizedMvcModelLocal> locales, Action<TLocalizedMvcModelLocal, string> configure) where TLocalizedMvcModelLocal : ILocalizedMvcLocaleModel
        {
            foreach (var language in languageService.GetAllLanguages(true))
            {
                var locale = Activator.CreateInstance<TLocalizedMvcModelLocal>();
                locale.LanguageId = language.Id;

                if (configure != null)
                    configure.Invoke(locale, locale.LanguageId);

                locales.Add(locale);
            }
        }

        #endregion

        #region Utilities

        protected virtual IActionResult InvalidModelResult()
        {
            return Ok(new XBaseResult
            {
                success = false,
                errors = ModelState.GetErrors()
            });
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Core.Configuration;
using VTQT.Core.Domain.Localization;
using VTQT.Core.Domain.Master;
using VTQT.Core.Events;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using VTQT.Services.Apps;
using VTQT.Services.Configuration;

namespace VTQT.Services.Localization
{
    /// <summary>
    /// Language service
    /// </summary>
    public partial class LanguageService : ILanguageService
    {
        #region Fields

        private readonly IRepository<Language> _languageRepository;
        private readonly IAppMappingService _appMappingService;
        private readonly ISettingService _settingService;
        private readonly IXBaseCacheManager _cacheManager;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public LanguageService(
            IAppMappingService appMappingService,
            ISettingService settingService,
            IXBaseCacheManager cacheManager,
            LocalizationSettings localizationSettings,
            IEventPublisher eventPublisher)
        {
            _appMappingService = appMappingService;
            _languageRepository = EngineContext.Current.Resolve<IRepository<Language>>(DataConnectionHelper.ConnectionStringNames.Master);
            _settingService = settingService;
            _cacheManager = cacheManager;
            _localizationSettings = localizationSettings;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a language
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteLanguageAsync(Language language)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            //update default admin area language (if required)
            if (_localizationSettings.DefaultAdminLanguageId == language.Id)
                foreach (var activeLanguage in await GetAllLanguagesAsync())
                {
                    if (activeLanguage.Id == language.Id)
                        continue;

                    _localizationSettings.DefaultAdminLanguageId = activeLanguage.Id;
                    await _settingService.SaveSettingAsync(_localizationSettings);
                    break;
                }

            await _languageRepository.DeleteAsync(language);

            //event notification
            await _eventPublisher.EntityDeletedAsync(language);
        }

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="appId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the languages
        /// </returns>
        public virtual async Task<IList<Language>> GetAllLanguagesAsync(bool showHidden = false, string appId = SettingDefaults.AllAppsId)
        {
            //cacheable copy
            var key = LocalizationCacheKeys.LanguagesAllCacheKey.FormatWith(appId, showHidden);

            var languages = await _cacheManager.GetDbAsync(key, async () =>
            {
                var query = _languageRepository.Table;
                if (!showHidden) query = query.Where(l => l.Published);
                query = query.OrderBy(l => l.DisplayOrder).ThenBy(l => l.Id);

                var allLanguages = await query.ToListAsync();

                //store mapping
                if (appId != SettingDefaults.AllAppsId)
                    allLanguages = await allLanguages
                        .WhereAwait(async l => await _appMappingService.AuthorizeAsync(l, appId))
                        .ToListAsync();

                return allLanguages;
            }, CachingDefaults.MonthCacheTime);

            return languages;
        }

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="appId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the languages
        /// </returns>
        public virtual IList<Language> GetAllLanguages(bool showHidden = false, string appId = SettingDefaults.AllAppsId)
        {
            //cacheable copy
            var key = LocalizationCacheKeys.LanguagesAllCacheKey.FormatWith(appId, showHidden);

            var languages = _cacheManager.GetDb(key, () =>
            {
                var query = _languageRepository.Table;
                if (!showHidden) query = query.Where(l => l.Published);
                query = query.OrderBy(l => l.DisplayOrder).ThenBy(l => l.Id);

                var allLanguages = query.ToList();

                //store mapping
                if (appId != SettingDefaults.AllAppsId)
                    allLanguages = allLanguages
                        .Where(l => _appMappingService.Authorize(l, appId))
                        .ToList();

                return allLanguages;
            }, CachingDefaults.MonthCacheTime);

            return languages;
        }

        /// <summary>
        /// Gets a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the language
        /// </returns>
        public virtual async Task<Language> GetLanguageByIdAsync(string languageId)
        {
            var key = BaseEntity.GetEntityCacheKey(typeof(Language), languageId);
            return await _cacheManager.GetDbAsync(key, async () => await _languageRepository.GetByIdAsync(languageId));
        }

        /// <summary>
        /// Gets a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the language
        /// </returns>
        public virtual Language GetLanguageById(string languageId)
        {
            var key = BaseEntity.GetEntityCacheKey(typeof(Language), languageId);
            return _cacheManager.GetDb(key, () => _languageRepository.GetById(languageId));
        }

        /// <summary>
        /// Inserts a language
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertLanguageAsync(Language language)
        {
            await _languageRepository.InsertAsync(language);

            //event notification
            await _eventPublisher.EntityInsertedAsync(language);
        }

        /// <summary>
        /// Updates a language
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateLanguageAsync(Language language)
        {
            //update language
            await _languageRepository.UpdateAsync(language);

            //event notification
            await _eventPublisher.EntityUpdatedAsync(language);
        }

        /// <summary>
        /// Get 2 letter ISO language code
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>ISO language code</returns>
        public virtual string GetTwoLetterIsoLanguageName(Language language)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            if (string.IsNullOrEmpty(language.LanguageCulture))
                return "en";

            var culture = new CultureInfo(language.LanguageCulture);
            var code = culture.TwoLetterISOLanguageName;

            return string.IsNullOrEmpty(code) ? "en" : code;
        }

        #endregion
    }
}

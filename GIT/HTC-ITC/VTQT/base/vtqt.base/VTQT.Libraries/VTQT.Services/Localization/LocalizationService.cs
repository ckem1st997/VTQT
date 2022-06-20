using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Core.Configuration;
using VTQT.Core.Domain.Localization;
using VTQT.Core.Domain.Master;
using VTQT.Core.Events;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using VTQT.Services.Configuration;
using VTQT.Services.ExportImport;

namespace VTQT.Services.Localization
{
    /// <summary>
    /// Provides information about localization
    /// </summary>
    public partial class LocalizationService : ILocalizationService
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IRepository<LocaleStringResource> _lsrRepository;
        private readonly ISettingService _settingService;
        private readonly IXBaseCacheManager _cacheManager;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IWorkContext _workContext;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public LocalizationService(
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            ISettingService settingService,
            IXBaseCacheManager cacheManager,
            LocalizationSettings localizationSettings,
            IWorkContext workContext,
            IEventPublisher eventPublisher)
        {
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _lsrRepository = EngineContext.Current.Resolve<IRepository<LocaleStringResource>>(DataConnectionHelper.ConnectionStringNames.Master);
            _settingService = settingService;
            _cacheManager = cacheManager;
            _localizationSettings = localizationSettings;
            _workContext = workContext;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Insert resources
        /// </summary>
        /// <param name="resources">Resources</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InsertLocaleStringResourcesAsync(IList<LocaleStringResource> resources)
        {
            await _lsrRepository.InsertAsync(resources);

            //event notification
            //foreach (var resource in resources)
            //{
            //    await _eventPublisher.EntityUpdatedAsync(resource);
            //}
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(LocalizationCacheKeys.LocaleStringResourcesAllPrefix);
        }

        /// <summary>
        /// Gets all locale string resources by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the locale string resources
        /// </returns>
        protected virtual async Task<IList<LocaleStringResource>> GetAllResourcesAsync(string languageId)
        {
            var key = LocalizationCacheKeys.LocaleStringResourcesByNamePrefix.FormatWith(languageId);

            var locales = (await _cacheManager.HybridProvider.GetAsync(key, async () =>
            {

                var query = from l in _lsrRepository.Table
                            orderby l.ResourceName
                            where l.LanguageId == languageId
                            select l;

                return await query.ToListAsync();
            }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime))).Value;

            return locales;
        }

        /// <summary>
        /// Update resources
        /// </summary>
        /// <param name="resources">Resources</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task UpdateLocaleStringResourcesAsync(IList<LocaleStringResource> resources)
        {
            await _lsrRepository.UpdateAsync(resources);

            //event notification
            //foreach (var resource in resources)
            //{
            //    await _eventPublisher.EntityUpdatedAsync(resource);
            //}
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(LocalizationCacheKeys.LocaleStringResourcesAllPrefix);
        }

        protected virtual HashSet<(string name, string value)> LoadLocaleResourcesFromStream(StreamReader xmlStreamReader, string language)
        {
            var result = new HashSet<(string name, string value)>();

            using (var xmlReader = XmlReader.Create(xmlStreamReader))
                while (xmlReader.ReadToFollowing("Language"))
                {
                    if (xmlReader.NodeType != XmlNodeType.Element)
                        continue;

                    using var languageReader = xmlReader.ReadSubtree();
                    while (languageReader.ReadToFollowing("LocaleResource"))
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.GetAttribute("Name") is string name)
                        {
                            using var lrReader = languageReader.ReadSubtree();
                            if (lrReader.ReadToFollowing("Value") && lrReader.NodeType == XmlNodeType.Element)
                                result.Add((name.ToLowerInvariant(), lrReader.ReadString()));
                        }

                    break;
                }

            return result;
        }

        private static Dictionary<string, KeyValuePair<string, string>> ResourceValuesToDictionary(IEnumerable<LocaleStringResource> locales)
        {
            //format: <name, <id, value>>
            var dictionary = new Dictionary<string, KeyValuePair<string, string>>();
            foreach (var locale in locales)
            {
                var resourceName = locale.ResourceName.ToLowerInvariant();
                if (!dictionary.ContainsKey(resourceName))
                    dictionary.Add(resourceName, new KeyValuePair<string, string>(locale.Id, locale.ResourceValue));
            }

            return dictionary;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteLocaleStringResourceAsync(LocaleStringResource localeStringResource)
        {
            await _lsrRepository.DeleteAsync(localeStringResource);

            //event notification
            await _eventPublisher.EntityDeletedAsync(localeStringResource);
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="localeStringResourceId">Locale string resource identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the locale string resource
        /// </returns>
        public virtual async Task<LocaleStringResource> GetLocaleStringResourceByIdAsync(string localeStringResourceId)
        {
            var key = BaseEntity.GetEntityCacheKey(typeof(LocaleStringResource), localeStringResourceId);
            return (await _cacheManager.HybridProvider.GetAsync(key, async () => await _lsrRepository.GetByIdAsync(localeStringResourceId),
                TimeSpan.FromMinutes(CachingDefaults.CacheTime))).Value;
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="resourceName">A string representing a resource name</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the locale string resource
        /// </returns>
        public virtual async Task<LocaleStringResource> GetLocaleStringResourceByNameAsync(string resourceName, string languageId,
            bool logIfNotFound = true)
        {
            var query = from lsr in _lsrRepository.Table
                        orderby lsr.ResourceName
                        where lsr.LanguageId == languageId && lsr.ResourceName == resourceName
                        select lsr;

            var localeStringResource = await query.FirstOrDefaultAsync();

            // TOTO-XBase-Log
            //if (localeStringResource == null && logIfNotFound)
            //    await _logger.WarningAsync($"Resource string ({resourceName}) not found. Language ID = {languageId}");

            return localeStringResource;
        }

        /// <summary>
        /// Inserts a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertLocaleStringResourceAsync(LocaleStringResource localeStringResource)
        {
            await _lsrRepository.InsertAsync(localeStringResource);

            //event notification
            await _eventPublisher.EntityInsertedAsync(localeStringResource);
        }

        /// <summary>
        /// Updates the locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateLocaleStringResourceAsync(LocaleStringResource localeStringResource)
        {
            await _lsrRepository.UpdateAsync(localeStringResource);

            //event notification
            await _eventPublisher.EntityUpdatedAsync(localeStringResource);
        }

        /// <summary>
        /// Gets all locale string resources by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <param name="loadPublicLocales">A value indicating whether to load data for the public store only (if "false", then for admin area only. If null, then load all locales. We use it for performance optimization of the site startup</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the locale string resources
        /// </returns>
        public virtual async Task<Dictionary<string, KeyValuePair<string, string>>> GetAllResourceValuesAsync(string languageId)
        {
            var key = LocalizationCacheKeys.LocaleStringResourcesAllCacheKey.FormatWith(languageId);

            return (await _cacheManager.HybridProvider.GetAsync(key, async () =>
            {
                //we use no tracking here for performance optimization
                //anyway records are loaded only for read-only operations
                var query = from l in _lsrRepository.Table
                    orderby l.ResourceName
                    where l.LanguageId == languageId
                    select l;

                var locales = await query.ToListAsync();

                return ResourceValuesToDictionary(locales);
            }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime))).Value;
        }

        /// <summary>
        /// Gets all locale string resources by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <param name="loadPublicLocales">A value indicating whether to load data for the public store only (if "false", then for admin area only. If null, then load all locales. We use it for performance optimization of the site startup</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the locale string resources
        /// </returns>
        public virtual Dictionary<string, KeyValuePair<string, string>> GetAllResourceValues(string languageId)
        {
            var key = LocalizationCacheKeys.LocaleStringResourcesAllCacheKey.FormatWith(languageId);

            return _cacheManager.HybridProvider.Get(key, () =>
            {
                //we use no tracking here for performance optimization
                //anyway records are loaded only for read-only operations
                var query = from l in _lsrRepository.Table
                    orderby l.ResourceName
                    where l.LanguageId == languageId
                    select l;

                var locales = query.ToList();

                return ResourceValuesToDictionary(locales);
            }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;
        }

        ///// <summary>
        ///// Gets a resource string based on the specified ResourceKey property.
        ///// </summary>
        ///// <param name="resourceKey">A string representing a ResourceKey.</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains a string representing the requested resource string.
        ///// </returns>
        //public virtual async Task<string> GetResourceAsync(string resourceKey)
        //{
        //    var workingLanguage = await _workContext.GetWorkingLanguageAsync();

        //    if (workingLanguage != null)
        //        return await GetResourceAsync(resourceKey, workingLanguage.Id);

        //    return string.Empty;
        //}

        ///// <summary>
        ///// Gets a resource string based on the specified ResourceKey property.
        ///// </summary>
        ///// <param name="resourceKey">A string representing a ResourceKey.</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains a string representing the requested resource string.
        ///// </returns>
        //public virtual string GetResource(string resourceKey)
        //{
        //    var workingLanguage = _workContext.GetWorkingLanguage();

        //    if (workingLanguage != null)
        //        return GetResource(resourceKey, workingLanguage.Id);

        //    return string.Empty;
        //}

        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="returnEmptyIfNotFound">A value indicating whether an empty string will be returned if a resource is not found and default value is set to empty string</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a string representing the requested resource string.
        /// </returns>
        public virtual async Task<string> GetResourceAsync(string resourceKey, string languageId = SettingDefaults.AllLanguagesId,
            bool logIfNotFound = true, string defaultValue = "", bool returnEmptyIfNotFound = false)
        {
            if (languageId == SettingDefaults.AllLanguagesId || string.IsNullOrWhiteSpace(languageId))
            {
                var curLanguage = _workContext.WorkingLanguage;
                if (curLanguage == null)
                {
                    return defaultValue;
                }

                languageId = curLanguage.Id;
            }

            var result = string.Empty;
            if (resourceKey == null)
                resourceKey = string.Empty;
            resourceKey = resourceKey.Trim().ToLowerInvariant();
            if (_localizationSettings.LoadAllLocaleRecordsOnStartup)
            {
                //load all records (we know they are cached)
                //var resources = await GetAllResourceValuesAsync(languageId, !resourceKey.StartsWith(LocalizationCacheKeys.AdminLocaleStringResourcesPrefix, StringComparison.InvariantCultureIgnoreCase));
                var resources = await GetAllResourceValuesAsync(languageId);
                if (resources.ContainsKey(resourceKey))
                {
                    result = resources[resourceKey].Value;
                }
            }
            else
            {
                //gradual loading
                var key = LocalizationCacheKeys.LocaleStringResourcesByNameCacheKey.FormatWith(languageId, resourceKey);

                var lsr = (await _cacheManager.HybridProvider.GetAsync(key, async () =>
                    {
                        var query = from l in _lsrRepository.Table
                                    where l.ResourceName == resourceKey
                                          && l.LanguageId == languageId
                                    select l.ResourceValue;

                        return await query.FirstOrDefaultAsync();
                    }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime))).Value;

                if (lsr != null)
                    result = lsr;
            }

            if (!string.IsNullOrEmpty(result))
                return result;

            //if (logIfNotFound)
            //    await _logger.WarningAsync($"Resource string ({resourceKey}) is not found. Language ID = {languageId}");

            if (!string.IsNullOrEmpty(defaultValue))
            {
                result = defaultValue;
            }
            else
            {
                if (!returnEmptyIfNotFound)
                    result = resourceKey;
            }

            return result;
        }

        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="returnEmptyIfNotFound">A value indicating whether an empty string will be returned if a resource is not found and default value is set to empty string</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a string representing the requested resource string.
        /// </returns>
        public virtual string GetResource(string resourceKey, string languageId = SettingDefaults.AllLanguagesId,
            bool logIfNotFound = true, string defaultValue = "", bool returnEmptyIfNotFound = false)
        {
            if (languageId == SettingDefaults.AllLanguagesId || string.IsNullOrWhiteSpace(languageId))
            {
                var curLanguage = _workContext.WorkingLanguage;
                if (curLanguage == null)
                {
                    return defaultValue;
                }

                languageId = curLanguage.Id;
            }

            var result = string.Empty;
            if (resourceKey == null)
                resourceKey = string.Empty;
            resourceKey = resourceKey.Trim().ToLowerInvariant();
            if (_localizationSettings.LoadAllLocaleRecordsOnStartup)
            {
                //load all records (we know they are cached)
                //var resources = GetAllResourceValues(languageId, !resourceKey.StartsWith(LocalizationCacheKeys.AdminLocaleStringResourcesPrefix, StringComparison.InvariantCultureIgnoreCase));\
                var resources = GetAllResourceValues(languageId);
                if (resources.ContainsKey(resourceKey))
                {
                    result = resources[resourceKey].Value;
                }
            }
            else
            {
                //gradual loading
                var key = LocalizationCacheKeys.LocaleStringResourcesByNameCacheKey.FormatWith(languageId, resourceKey);

                var lsr = _cacheManager.HybridProvider.Get(key, () =>
                {
                    var query = from l in _lsrRepository.Table
                                where l.ResourceName == resourceKey
                                      && l.LanguageId == languageId
                                select l.ResourceValue;

                    return query.FirstOrDefault();
                }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;

                if (lsr != null)
                    result = lsr;
            }

            if (!string.IsNullOrEmpty(result))
                return result;

            //if (logIfNotFound)
            //    await _logger.WarningAsync($"Resource string ({resourceKey}) is not found. Language ID = {languageId}");

            if (!string.IsNullOrEmpty(defaultValue))
            {
                result = defaultValue;
            }
            else
            {
                if (!returnEmptyIfNotFound)
                    result = resourceKey;
            }

            return result;
        }

        /// <summary>
        /// Export language resources to XML
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result in XML format
        /// </returns>
        public virtual async Task<string> ExportResourcesToXmlAsync(Language language)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            var settings = new XmlWriterSettings
            {
                Async = true,
                Encoding = Encoding.UTF8,
                ConformanceLevel = ConformanceLevel.Auto
            };

            await using var stream = new MemoryStream();
            //await using var xmlWriter = XmlWriter.Create(stream, settings);
            using var xmlWriter = XmlWriter.Create(stream, settings);

            await xmlWriter.WriteStartDocumentAsync();
            await xmlWriter.WriteStartElementAsync("Language");
            await xmlWriter.WriteAttributeStringAsync("Name", language.Name);
            await xmlWriter.WriteAttributeStringAsync("SupportedVersion", XBaseVersion.CurrentVersion);

            var resources = await GetAllResourcesAsync(language.Id);
            foreach (var resource in resources)
            {
                await xmlWriter.WriteStartElementAsync("LocaleResource");
                await xmlWriter.WriteAttributeStringAsync("Name", resource.ResourceName);
                await xmlWriter.WriteElementStringAsync("Value", null, resource.ResourceValue);
                await xmlWriter.WriteEndElementAsync();
            }

            await xmlWriter.WriteEndElementAsync();
            await xmlWriter.WriteEndDocumentAsync();
            await xmlWriter.FlushAsync();

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// Import language resources from XML file
        /// </summary>
        /// <param name="language">Language</param>
        /// <param name="xmlStreamReader">Stream reader of XML file</param>
        /// <param name="updateExistingResources">A value indicating whether to update existing resources</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ImportResourcesFromXmlAsync(Language language, StreamReader xmlStreamReader, bool updateExistingResources = true)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            if (xmlStreamReader.EndOfStream)
                return;

            var lsNamesList = new Dictionary<string, LocaleStringResource>();

            foreach (var localeStringResource in _lsrRepository.Table.Where(lsr => lsr.LanguageId == language.Id)
                .OrderBy(lsr => lsr.Id))
                lsNamesList[localeStringResource.ResourceName.ToLowerInvariant()] = localeStringResource;

            var lrsToUpdateList = new List<LocaleStringResource>();
            var lrsToInsertList = new Dictionary<string, LocaleStringResource>();

            foreach (var (name, value) in LoadLocaleResourcesFromStream(xmlStreamReader, language.Name))
            {
                if (lsNamesList.ContainsKey(name))
                {
                    if (!updateExistingResources)
                        continue;

                    var lsr = lsNamesList[name];
                    lsr.ResourceValue = value;
                    lrsToUpdateList.Add(lsr);
                }
                else
                {
                    var lsr = new LocaleStringResource { LanguageId = language.Id, ResourceName = name, ResourceValue = value };
                    lrsToInsertList[name] = lsr;
                }
            }

            await _lsrRepository.UpdateAsync(lrsToUpdateList);
            await _lsrRepository.InsertAsync(lrsToInsertList.Values.ToList());

            //clear cache
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(BaseEntity.GetEntityPrefix(typeof(LocaleStringResource)));
        }

        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier; pass null to use the current working language; pass 0 to get standard language value</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the localized property
        /// </returns>
        public virtual async Task<TPropType> GetLocalizedAsync<TEntity, TPropType>(TEntity entity, Expression<Func<TEntity, TPropType>> keySelector,
            string languageId = null, bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true)
            where TEntity : BaseEntity, ILocalizedEntity
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (!(keySelector.Body is MemberExpression member))
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            if (!(member.Member is PropertyInfo propInfo))
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            var result = default(TPropType);
            var resultStr = string.Empty;

            var localeKeyGroup = entity.GetType().Name;
            var localeKey = propInfo.Name;

            var workingLanguage = _workContext.WorkingLanguage;

            if (string.IsNullOrWhiteSpace(languageId))
                languageId = workingLanguage.Id;

            if (!string.IsNullOrWhiteSpace(languageId))
            {
                //ensure that we have at least two published languages
                var loadLocalizedValue = true;
                if (ensureTwoPublishedLanguages)
                {
                    var totalPublishedLanguages = (await _languageService.GetAllLanguagesAsync()).Count;
                    loadLocalizedValue = totalPublishedLanguages >= 2;
                }

                //localized value
                if (loadLocalizedValue)
                {
                    resultStr = await _localizedEntityService
                        .GetLocalizedValueAsync(languageId, entity.Id, localeKeyGroup, localeKey);
                    if (!string.IsNullOrEmpty(resultStr))
                        result = CommonHelper.To<TPropType>(resultStr);
                }
            }

            //set default value if required
            if (!string.IsNullOrEmpty(resultStr) || !returnDefaultValue)
                return result;
            var localizer = keySelector.Compile();
            result = localizer(entity);

            return result;
        }

        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier; pass null to use the current working language; pass 0 to get standard language value</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the localized property
        /// </returns>
        public virtual TPropType GetLocalized<TEntity, TPropType>(TEntity entity, Expression<Func<TEntity, TPropType>> keySelector,
            string languageId = null, bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true)
            where TEntity : BaseEntity, ILocalizedEntity
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (!(keySelector.Body is MemberExpression member))
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            if (!(member.Member is PropertyInfo propInfo))
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            var result = default(TPropType);
            var resultStr = string.Empty;

            var localeKeyGroup = entity.GetType().Name;
            var localeKey = propInfo.Name;

            var workingLanguage = _workContext.WorkingLanguage;

            if (string.IsNullOrWhiteSpace(languageId))
                languageId = workingLanguage.Id;

            if (!string.IsNullOrWhiteSpace(languageId))
            {
                //ensure that we have at least two published languages
                var loadLocalizedValue = true;
                if (ensureTwoPublishedLanguages)
                {
                    var totalPublishedLanguages = _languageService.GetAllLanguages().Count;
                    loadLocalizedValue = totalPublishedLanguages >= 2;
                }

                //localized value
                if (loadLocalizedValue)
                {
                    resultStr = _localizedEntityService
                        .GetLocalizedValue(languageId, entity.Id, localeKeyGroup, localeKey);
                    if (!string.IsNullOrEmpty(resultStr))
                        result = CommonHelper.To<TPropType>(resultStr);
                }
            }

            //set default value if required
            if (!string.IsNullOrEmpty(resultStr) || !returnDefaultValue)
                return result;
            var localizer = keySelector.Compile();
            result = localizer(entity);

            return result;
        }

        /// <summary>
        /// Get localized property of setting
        /// </summary>
        /// <typeparam name="TSettings">Settings type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="appId">Store identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the localized property
        /// </returns>
        public virtual async Task<string> GetLocalizedSettingAsync<TSettings>(TSettings settings, Expression<Func<TSettings, string>> keySelector,
            string languageId, string appId, bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true)
            where TSettings : ISettings, new()
        {
            var key = _settingService.GetSettingKey(settings, keySelector);

            //we do not support localized settings per store (overridden store settings)
            var setting = await _settingService.GetSettingAsync(key, appId: appId, loadSharedValueIfNotFound: true);
            if (setting == null)
                return null;

            return await GetLocalizedAsync(setting, x => x.Value, languageId, returnDefaultValue, ensureTwoPublishedLanguages);
        }

        /// <summary>
        /// Save localized property of setting
        /// </summary>
        /// <typeparam name="TSettings">Settings type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="value">Localized value</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the localized property
        /// </returns>
        public virtual async Task SaveLocalizedSettingAsync<TSettings>(TSettings settings, Expression<Func<TSettings, string>> keySelector,
            string languageId, string value) where TSettings : ISettings, new()
        {
            var key = _settingService.GetSettingKey(settings, keySelector);

            //we do not support localized settings per store (overridden store settings)
            var setting = await _settingService.GetSettingAsync(key);
            if (setting == null)
                return;

            await _localizedEntityService.SaveLocalizedValueAsync(setting, x => x.Value, value, languageId);
        }

        /// <summary>
        /// Get localized value of enum
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <param name="languageId">Language identifier; pass null to use the current working language</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the localized value
        /// </returns>
        public virtual async Task<string> GetLocalizedEnumAsync<TEnum>(TEnum enumValue, string languageId = null) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            //localized value
            var workingLanguage = _workContext.WorkingLanguage;
            var resourceName = $"{LocalizationCacheKeys.EnumLocaleStringResourcesPrefix}.{typeof(TEnum)}.{enumValue}";
            var result = await GetResourceAsync(resourceName, languageId ?? workingLanguage.Id, false, string.Empty, true);

            //set default value if required
            if (string.IsNullOrEmpty(result))
                result = CommonHelper.ConvertEnum(enumValue.ToString());

            return result;
        }

        /// <summary>
        /// Get localized value of enum
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <param name="languageId">Language identifier; pass null to use the current working language</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the localized value
        /// </returns>
        public virtual string GetLocalizedEnum<TEnum>(TEnum enumValue, string languageId = null) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            //localized value
            var workingLanguage = _workContext.WorkingLanguage;
            var resourceName = $"{LocalizationCacheKeys.EnumLocaleStringResourcesPrefix}.{typeof(TEnum)}.{enumValue}";
            var result = GetResource(resourceName, languageId ?? workingLanguage.Id, false, string.Empty, true);

            //set default value if required
            if (string.IsNullOrEmpty(result))
                result = CommonHelper.ConvertEnum(enumValue.ToString());

            return result;
        }

        /// <summary>
        /// Add a locale resource (if new) or update an existing one
        /// </summary>
        /// <param name="resourceName">Resource name</param>
        /// <param name="resourceValue">Resource value</param>
        /// <param name="languageCulture">Language culture code. If null or empty, then a resource will be added for all languages</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddOrUpdateLocaleResourceAsync(string resourceName, string resourceValue, string languageCulture = null)
        {
            foreach (var lang in await _languageService.GetAllLanguagesAsync(true))
            {
                if (!string.IsNullOrEmpty(languageCulture) && !languageCulture.Equals(lang.LanguageCulture))
                    continue;

                var lsr = await GetLocaleStringResourceByNameAsync(resourceName, lang.Id, false);
                if (lsr == null)
                {
                    lsr = new LocaleStringResource
                    {
                        LanguageId = lang.Id,
                        ResourceName = resourceName,
                        ResourceValue = resourceValue
                    };
                    await InsertLocaleStringResourceAsync(lsr);
                }
                else
                {
                    lsr.ResourceValue = resourceValue;
                    await UpdateLocaleStringResourceAsync(lsr);
                }
            }
        }

        /// <summary>
        /// Add locale resources
        /// </summary>
        /// <param name="resources">Resource name-value pairs</param>
        /// <param name="languageId">Language identifier; pass null to add the passed resources for all languages</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddLocaleResourceAsync(IDictionary<string, string> resources, string languageId = null)
        {
            //first delete all previous locales with the passed names if they exist
            await DeleteLocaleResourcesAsync(resources.Keys.ToList(), languageId);

            //insert new locale resources
            var locales = (await _languageService.GetAllLanguagesAsync(true))
                .Where(language => string.IsNullOrWhiteSpace(languageId) || language.Id == languageId)
                .SelectMany(language => resources.Select(resource => new LocaleStringResource
                {
                    LanguageId = language.Id,
                    ResourceName = resource.Key,
                    ResourceValue = resource.Value
                }))
                .ToList();

            await _lsrRepository.InsertAsync(locales);

            //clear cache
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(BaseEntity.GetEntityPrefix(typeof(LocaleStringResource)));
        }

        /// <summary>
        /// Delete a locale resource
        /// </summary>
        /// <param name="resourceName">Resource name</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteLocaleResourceAsync(string resourceName)
        {
            foreach (var lang in await _languageService.GetAllLanguagesAsync(true))
            {
                var lsr = await GetLocaleStringResourceByNameAsync(resourceName, lang.Id, false);
                if (lsr != null)
                    await DeleteLocaleStringResourceAsync(lsr);
            }
        }

        /// <summary>
        /// Delete locale resources
        /// </summary>
        /// <param name="resourceNames">Resource names</param>
        /// <param name="languageId">Language identifier; pass null to delete the passed resources from all languages</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteLocaleResourcesAsync(IList<string> resourceNames, string languageId = null)
        {
            await _lsrRepository.DeleteAsync(locale => (string.IsNullOrWhiteSpace(languageId) || locale.LanguageId == languageId) &&
                resourceNames.Contains(locale.ResourceName, StringComparer.InvariantCultureIgnoreCase));

            //clear cache
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(BaseEntity.GetEntityPrefix(typeof(LocaleStringResource)));
        }

        /// <summary>
        /// Delete locale resources by the passed name prefix
        /// </summary>
        /// <param name="resourceNamePrefix">Resource name prefix</param>
        /// <param name="languageId">Language identifier; pass null to delete resources by prefix from all languages</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteLocaleResourcesAsync(string resourceNamePrefix, string languageId = null)
        {
            await _lsrRepository.DeleteAsync(locale => (string.IsNullOrWhiteSpace(languageId) || locale.LanguageId == languageId) &&
                !string.IsNullOrEmpty(locale.ResourceName) &&
                locale.ResourceName.StartsWith(resourceNamePrefix, StringComparison.InvariantCultureIgnoreCase));

            //clear cache
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(BaseEntity.GetEntityPrefix(typeof(LocaleStringResource)));
        }

        #endregion
    }
}

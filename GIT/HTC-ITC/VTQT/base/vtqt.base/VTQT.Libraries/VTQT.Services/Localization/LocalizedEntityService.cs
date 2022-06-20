using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Core.Domain.Localization;
using VTQT.Core.Domain.Master;
using VTQT.Core.Events;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Localization
{
    /// <summary>
    /// Provides information about localizable entities
    /// </summary>
    public partial class LocalizedEntityService : ILocalizedEntityService
    {
        #region Fields

        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IXBaseCacheManager _cacheManager;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public LocalizedEntityService(
            IXBaseCacheManager cacheManager,
            LocalizationSettings localizationSettings,
            IEventPublisher eventPublisher)
        {
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
            _cacheManager = cacheManager;
            _localizationSettings = localizationSettings;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets localized properties
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="localeKeyGroup">Locale key group</param>
        /// <returns>Localized properties</returns>
        protected virtual async Task<IList<LocalizedProperty>> GetLocalizedPropertiesAsync(string entityId, string localeKeyGroup)
        {
            if (string.IsNullOrWhiteSpace(entityId) || string.IsNullOrWhiteSpace(localeKeyGroup))
                return new List<LocalizedProperty>();

            var query = from lp in _localizedPropertyRepository.Table
                        orderby lp.Id
                        where lp.EntityId == entityId &&
                              lp.LocaleKeyGroup == localeKeyGroup
                        select lp;

            var props = await query.ToListAsync();

            return props;
        }

        /// <summary>
        /// Gets all cached localized properties
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the cached localized properties
        /// </returns>
        protected virtual async Task<IList<LocalizedProperty>> GetAllLocalizedPropertiesAsync()
        {
            var key = LocalizationCacheKeys.LocalizedPropertyAllCacheKey;

            var entities = await _cacheManager.GetDbAsync(key, async () =>
            {
                var query = from lp in _localizedPropertyRepository.Table
                            select lp;

                return await query.ToListAsync();
            }, CachingDefaults.MonthCacheTime);

            return entities;
        }

        /// <summary>
        /// Deletes a localized property
        /// </summary>
        /// <param name="localizedProperty">Localized property</param>
        public virtual async Task DeleteLocalizedPropertyAsync(LocalizedProperty localizedProperty)
        {
            await _localizedPropertyRepository.DeleteAsync(localizedProperty);

            //event notification
            await _eventPublisher.EntityDeletedAsync(localizedProperty);
        }

        /// <summary>
        /// Inserts a localized property
        /// </summary>
        /// <param name="localizedProperty">Localized property</param>
        public virtual async Task InsertLocalizedPropertyAsync(LocalizedProperty localizedProperty)
        {
            await _localizedPropertyRepository.InsertAsync(localizedProperty);

            //event notification
            await _eventPublisher.EntityInsertedAsync(localizedProperty);
        }

        /// <summary>
        /// Updates the localized property
        /// </summary>
        /// <param name="localizedProperty">Localized property</param>
        public virtual async Task UpdateLocalizedPropertyAsync(LocalizedProperty localizedProperty)
        {
            await _localizedPropertyRepository.UpdateAsync(localizedProperty);

            //event notification
            await _eventPublisher.EntityUpdatedAsync(localizedProperty);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Find localized value
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="localeKeyGroup">Locale key group</param>
        /// <param name="localeKey">Locale key</param>
        /// <returns>Found localized value</returns>
        public virtual async Task<string> GetLocalizedValueAsync(string languageId, string entityId, string localeKeyGroup, string localeKey, Func<string> defaultValue = null)
        {
            var key = LocalizationCacheKeys.LocalizedPropertyCacheKey.FormatWith(languageId, entityId, localeKeyGroup, localeKey);

            return _cacheManager.HybridProvider.Get(key, () =>
            {
                //var source = _localizationSettings.LoadAllLocalizedPropertiesOnStartup
                //    //load all records (we know they are cached)
                //    ? (await GetAllLocalizedPropertiesAsync()).AsQueryable()
                //    //gradual loading
                //    : _localizedPropertyRepository.Table;
                var source = _localizedPropertyRepository.Table;

                var query = from lp in source
                            where lp.LanguageId == languageId &&
                                  lp.EntityId == entityId &&
                                  lp.LocaleKeyGroup == localeKeyGroup &&
                                  lp.LocaleKey == localeKey
                            select lp.LocaleValue;

                //little hack here. nulls aren't cacheable so set it to ""
                var localeValue = query.FirstOrDefault() ?? string.Empty;

                // Custom: Fix default value cho Entity trong trường hợp không lấy được locale tương ứng,
                // thì sẽ lấy theo tham số hàm truyền vào, để tránh trường hợp bị lưu Cache trống
                if (string.IsNullOrEmpty(localeValue) && defaultValue != null)
                {
                    var defaultLocale = defaultValue();
                    if (!string.IsNullOrEmpty(defaultLocale))
                        localeValue = defaultLocale;
                }

                return localeValue;
            }, TimeSpan.FromMinutes(CachingDefaults.CacheTime)).Value;

            // Tạm thời bỏ Cache vì nếu Redis lỗi thì sẽ gây load dữ liệu lâu vì tính timeout * (N) Entity.
            // Set timeout Redis để dùng Cache, nếu không khi lấy danh sách nhiều dữ liệu cũng sẽ bị chậm,
            // hơn nữa có nhiều danh sách được dùng lại nhiều
            // TODO-Refactor-Localized: Get List Localized cả danh sách rồi bind về Model
            //var source = _localizedPropertyRepository.Table;

            //var query = from lp in source
            //    where lp.LanguageId == languageId &&
            //          lp.EntityId == entityId &&
            //          lp.LocaleKeyGroup == localeKeyGroup &&
            //          lp.LocaleKey == localeKey
            //    select lp.LocaleValue;

            ////little hack here. nulls aren't cacheable so set it to ""
            //var localeValue = query.FirstOrDefault() ?? string.Empty;

            //return localeValue;
        }

        /// <summary>
        /// Find localized value
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="localeKeyGroup">Locale key group</param>
        /// <param name="localeKey">Locale key</param>
        /// <returns>Found localized value</returns>
        public virtual string GetLocalizedValue(string languageId, string entityId, string localeKeyGroup, string localeKey, Func<string> defaultValue = null)
        {
            var key = LocalizationCacheKeys.LocalizedPropertyCacheKey.FormatWith(languageId, entityId, localeKeyGroup, localeKey);

            return _cacheManager.HybridProvider.Get(key, () =>
            {
                //var source = _localizationSettings.LoadAllLocalizedPropertiesOnStartup
                //    //load all records (we know they are cached)
                //    ? (await GetAllLocalizedPropertiesAsync()).AsQueryable()
                //    //gradual loading
                //    : _localizedPropertyRepository.Table;
                var source = _localizedPropertyRepository.Table;

                var query = from lp in source
                            where lp.LanguageId == languageId &&
                                  lp.EntityId == entityId &&
                                  lp.LocaleKeyGroup == localeKeyGroup &&
                                  lp.LocaleKey == localeKey
                            select lp.LocaleValue;

                //little hack here. nulls aren't cacheable so set it to ""
                var localeValue = query.FirstOrDefault() ?? string.Empty;

                // Custom: Fix default value cho Entity trong trường hợp không lấy được locale tương ứng,
                // thì sẽ lấy theo tham số hàm truyền vào, để tránh trường hợp bị lưu Cache trống
                if (string.IsNullOrEmpty(localeValue) && defaultValue != null)
                {
                    var defaultLocale = defaultValue();
                    if (!string.IsNullOrEmpty(defaultLocale))
                        localeValue = defaultLocale;
                }

                return localeValue;
            }, TimeSpan.FromMinutes(CachingDefaults.CacheTime)).Value;

            // Tạm thời bỏ Cache vì nếu Redis lỗi thì sẽ gây load dữ liệu lâu vì tính timeout * (N) Entity.
            // Set timeout Redis để dùng Cache, nếu không khi lấy danh sách nhiều dữ liệu cũng sẽ bị chậm,
            // hơn nữa có nhiều danh sách được dùng lại nhiều
            // TODO-Refactor-Localized: Get List Localized cả danh sách rồi bind về Model
            //var source = _localizedPropertyRepository.Table;

            //var query = from lp in source
            //    where lp.LanguageId == languageId &&
            //          lp.EntityId == entityId &&
            //          lp.LocaleKeyGroup == localeKeyGroup &&
            //          lp.LocaleKey == localeKey
            //    select lp.LocaleValue;

            ////little hack here. nulls aren't cacheable so set it to ""
            //var localeValue = query.FirstOrDefault() ?? string.Empty;

            //return localeValue;
        }

        /// <summary>
        /// Dùng trong trường hợp muốn lấy giá trị default là Standard ở Table/Entity chính khi không có bản ghi đa ngôn ngữ
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="keySelector"></param>
        /// <param name="languageId"></param>
        /// <param name="entityId"></param>
        /// <param name="returnDefaultValue"></param>
        /// <returns></returns>
        public virtual async Task<string> GetLocalizedAsync<TEntity>(Expression<Func<TEntity, string>> keySelector, string languageId, string entityId, bool returnDefaultValue = true)
            where TEntity : BaseEntity, ILocalizedEntity
        {
            if (!(keySelector.Body is MemberExpression member))
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            if (!(member.Member is PropertyInfo propInfo))
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            var localeKeyGroup = typeof(TEntity).Name;
            var localeKey = ((keySelector.Body as MemberExpression).Member as PropertyInfo).Name;

            var key = LocalizationCacheKeys.LocalizedPropertyCacheKey.FormatWith(languageId, entityId, localeKeyGroup, localeKey);

            string value = (await _cacheManager.HybridProvider.GetAsync(key, async () =>
            {
                var source = _localizedPropertyRepository.Table;

                var query = from lp in source
                            where lp.LanguageId == languageId &&
                                  lp.EntityId == entityId &&
                                  lp.LocaleKeyGroup == localeKeyGroup &&
                                  lp.LocaleKey == localeKey
                            select lp.LocaleValue;

                //little hack here. nulls aren't cacheable so set it to ""
                var localeValue = await query.FirstOrDefaultAsync() ?? string.Empty;
                if (string.IsNullOrEmpty(localeValue) && returnDefaultValue)
                {
                    var defValue = await _localizedPropertyRepository.DataConnection.GetTable<TEntity>()
                        .Where(w => w.Id == entityId)
                        .Select(keySelector)
                        .FirstOrDefaultAsync();
                    localeValue = defValue;
                }

                return localeValue;
            }, TimeSpan.FromMinutes(CachingDefaults.CacheTime))).Value;
            return value;
        }

        /// <summary>
        /// Save localized value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="localeValue">Locale value</param>
        /// <param name="languageId">Language ID</param>
        public virtual async Task SaveLocalizedValueAsync<T>(T entity,
            Expression<Func<T, string>> keySelector,
            string localeValue,
            string languageId) where T : BaseEntity, ILocalizedEntity
        {
            await SaveLocalizedValueAsync<T, string>(entity, keySelector, localeValue, languageId);
        }

        /// <summary>
        /// Save localized value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="localeValue">Locale value</param>
        /// <param name="languageId">Language ID</param>
        public virtual async Task SaveLocalizedValueAsync<T, TPropType>(T entity,
            Expression<Func<T, TPropType>> keySelector,
            TPropType localeValue,
            string languageId) where T : BaseEntity, ILocalizedEntity
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (string.IsNullOrWhiteSpace(languageId))
                throw new ArgumentOutOfRangeException(nameof(languageId), "Language ID should not be empty");

            if (!(keySelector.Body is MemberExpression member))
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    keySelector));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                       "Expression '{0}' refers to a field, not a property.",
                       keySelector));
            }

            //load localized value (check whether it's a cacheable entity. In such cases we load its original entity type)
            var localeKeyGroup = entity.GetType().Name;
            var localeKey = propInfo.Name;

            var props = await GetLocalizedPropertiesAsync(entity.Id, localeKeyGroup);
            var prop = props.FirstOrDefault(lp => lp.LanguageId == languageId &&
                lp.LocaleKey.Equals(localeKey, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

            var localeValueStr = CommonHelper.To<string>(localeValue);

            if (prop != null)
            {
                if (string.IsNullOrWhiteSpace(localeValueStr))
                {
                    //delete
                    await DeleteLocalizedPropertyAsync(prop);
                }
                else
                {
                    //update
                    prop.LocaleValue = localeValueStr;
                    await UpdateLocalizedPropertyAsync(prop);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(localeValueStr))
                    return;

                //insert
                prop = new LocalizedProperty
                {
                    EntityId = entity.Id,
                    LanguageId = languageId,
                    LocaleKey = localeKey,
                    LocaleKeyGroup = localeKeyGroup,
                    LocaleValue = localeValueStr
                };
                await InsertLocalizedPropertyAsync(prop);
            }
        }

        #endregion
    }
}

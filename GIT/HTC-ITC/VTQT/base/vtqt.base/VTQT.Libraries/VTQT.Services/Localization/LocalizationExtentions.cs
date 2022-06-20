using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Localization;
using VTQT.Core.Infrastructure;

namespace VTQT.Services.Localization
{
    public static class LocalizationExtentions
    {
        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <returns>Localized property</returns>
        public static async Task<string> GetLocalizedAsync<T>(this T entity, Expression<Func<T, string>> keySelector)
            where T : BaseEntity, ILocalizedEntity
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var ls = EngineContext.Current.Resolve<ILocalizationService>();
            var languageId = workContext.LanguageId;
            return await ls.GetLocalizedAsync(entity, keySelector, languageId);
        }

        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <returns>Localized property</returns>
        public static string GetLocalized<T>(this T entity, Expression<Func<T, string>> keySelector)
            where T : BaseEntity, ILocalizedEntity
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var ls = EngineContext.Current.Resolve<ILocalizationService>();
            var languageId = workContext.LanguageId;
            return ls.GetLocalized(entity, keySelector, languageId);
        }

        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>Localized property</returns>
        public static async Task<string> GetLocalizedAsync<T>(this T entity,
            Expression<Func<T, string>> keySelector, string languageId,
            bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true)
            where T : BaseEntity, ILocalizedEntity
        {
            var ls = EngineContext.Current.Resolve<ILocalizationService>();
            return await ls.GetLocalizedAsync<T, string>(entity, keySelector, languageId, returnDefaultValue, ensureTwoPublishedLanguages);
        }

        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>Localized property</returns>
        public static string GetLocalized<T>(this T entity,
            Expression<Func<T, string>> keySelector, string languageId,
            bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true)
            where T : BaseEntity, ILocalizedEntity
        {
            var ls = EngineContext.Current.Resolve<ILocalizationService>();
            return ls.GetLocalized<T, string>(entity, keySelector, languageId, returnDefaultValue, ensureTwoPublishedLanguages);
        }

        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>Localized property</returns>
        public static async Task<TPropType> GetLocalizedAsync<T, TPropType>(this T entity,
            Expression<Func<T, TPropType>> keySelector, string languageId,
            bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true)
            where T : BaseEntity, ILocalizedEntity
        {
            var ls = EngineContext.Current.Resolve<ILocalizationService>();
            return await ls.GetLocalizedAsync<T, TPropType>(entity, keySelector, languageId, returnDefaultValue, ensureTwoPublishedLanguages);
        }

        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>Localized property</returns>
        public static TPropType GetLocalized<T, TPropType>(this T entity,
            Expression<Func<T, TPropType>> keySelector, string languageId,
            bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true)
            where T : BaseEntity, ILocalizedEntity
        {
            var ls = EngineContext.Current.Resolve<ILocalizationService>();
            return ls.GetLocalized<T, TPropType>(entity, keySelector, languageId, returnDefaultValue, ensureTwoPublishedLanguages);
        }

        /// <summary>
        /// Get localized value of enum
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="workContext">Work context</param>
        /// <returns>Localized value</returns>
        public static async Task<string> GetLocalizedEnumAsync<T>(this T enumValue, ILocalizationService localizationService, IWorkContext workContext)
            where T : struct
        {
            var languageId = workContext.LanguageId;
            return await localizationService.GetLocalizedEnumAsync<T>(enumValue, languageId);
        }

        /// <summary>
        /// Get localized value of enum
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="workContext">Work context</param>
        /// <returns>Localized value</returns>
        public static string GetLocalizedEnum<T>(this T enumValue, ILocalizationService localizationService, IWorkContext workContext)
            where T : struct
        {
            var languageId = workContext.LanguageId;
            return localizationService.GetLocalizedEnum<T>(enumValue, languageId);
        }

        public static async Task<string> GetLocalizedEnumAsync<T>(this T enumValue, ILocalizationService localizationService, string languageId)
            where T : struct
        {
            return await localizationService.GetLocalizedEnumAsync<T>(enumValue, languageId);
        }

        public static string GetLocalizedEnum<T>(this T enumValue, ILocalizationService localizationService, string languageId)
            where T : struct
        {
            return localizationService.GetLocalizedEnum<T>(enumValue, languageId);
        }
    }
}

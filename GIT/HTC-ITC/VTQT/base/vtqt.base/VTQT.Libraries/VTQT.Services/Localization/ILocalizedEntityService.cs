using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Localization;

namespace VTQT.Services.Localization
{
    /// <summary>
    /// Localized entity service interface
    /// </summary>
    public partial interface ILocalizedEntityService
    {
        /// <summary>
        /// Find localized value
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="localeKeyGroup">Locale key group</param>
        /// <param name="localeKey">Locale key</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the found localized value
        /// </returns>
        Task<string> GetLocalizedValueAsync(string languageId, string entityId, string localeKeyGroup, string localeKey, Func<string> defaultValue = null);

        /// <summary>
        /// Find localized value
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="localeKeyGroup">Locale key group</param>
        /// <param name="localeKey">Locale key</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the found localized value
        /// </returns>
        string GetLocalizedValue(string languageId, string entityId, string localeKeyGroup, string localeKey, Func<string> defaultValue = null);

        /// <summary>
        /// Dùng trong trường hợp muốn lấy giá trị default là Standard ở Table/Entity chính khi không có bản ghi đa ngôn ngữ
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="keySelector"></param>
        /// <param name="languageId"></param>
        /// <param name="entityId"></param>
        /// <param name="returnDefaultValue"></param>
        /// <returns></returns>
        Task<string> GetLocalizedAsync<TEntity>(Expression<Func<TEntity, string>> keySelector, string languageId, string entityId, bool returnDefaultValue = true)
            where TEntity : BaseEntity, ILocalizedEntity;

        /// <summary>
        /// Save localized value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="localeValue">Locale value</param>
        /// <param name="languageId">Language ID</param>
        Task SaveLocalizedValueAsync<T>(T entity,
            Expression<Func<T, string>> keySelector,
            string localeValue,
            string languageId) where T : BaseEntity, ILocalizedEntity;

        /// <summary>
        /// Save localized value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="localeValue">Locale value</param>
        /// <param name="languageId">Language ID</param>
        Task SaveLocalizedValueAsync<T, TPropType>(T entity,
           Expression<Func<T, TPropType>> keySelector,
           TPropType localeValue,
           string languageId) where T : BaseEntity, ILocalizedEntity;
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Configuration;
using VTQT.Core.Domain.Master;

namespace VTQT.Services.Localization
{
    /// <summary>
    /// Language service interface
    /// </summary>
    public partial interface ILanguageService
    {
        /// <summary>
        /// Deletes a language
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteLanguageAsync(Language language);

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="appId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the languages
        /// </returns>
        Task<IList<Language>> GetAllLanguagesAsync(bool showHidden = false, string appId = SettingDefaults.AllAppsId);

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="appId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the languages
        /// </returns>
        IList<Language> GetAllLanguages(bool showHidden = false, string appId = SettingDefaults.AllAppsId);

        /// <summary>
        /// Gets a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the language
        /// </returns>
        Task<Language> GetLanguageByIdAsync(string languageId);

        /// <summary>
        /// Gets a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the language
        /// </returns>
        Language GetLanguageById(string languageId);

        /// <summary>
        /// Inserts a language
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertLanguageAsync(Language language);

        /// <summary>
        /// Updates a language
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateLanguageAsync(Language language);

        //TODO: migrate to an extension method
        /// <summary>
        /// Get 2 letter ISO language code
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>ISO language code</returns>
        string GetTwoLetterIsoLanguageName(Language language);
    }
}

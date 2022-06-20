using System.Collections.Generic;

namespace VTQT.Web.Framework.Modelling
{
    /// <summary>
    /// Represents localized model
    /// </summary>
    public interface ILocalizedModel
    {
    }

    #region API

    /// <summary>
    /// Represents generic localized model
    /// </summary>
    /// <typeparam name="T">Localized model type</typeparam>
    public interface ILocalizedApiModel<T> : ILocalizedModel where T : ILocalizedApiLocaleModel
    {
        public IList<T> Locales { get; set; }

        // Key: Field Name; Value: Label Value
        public IDictionary<string, string> LocaleLabels { get; set; }
    }

    /// <summary>
    /// Represents localized locale model
    /// </summary>
    public interface ILocalizedApiLocaleModel
    {
        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        string LanguageId { get; set; }

        /// <summary>
        /// ISO two-letter language code
        /// </summary>
        string _LanguageCode { get; set; }

        /// <summary>
        /// ISO two-letter country code
        /// </summary>
        string _FlagCode { get; set; }
    }

    #endregion

    #region MVC

    /// <summary>
    /// Represents generic localized model
    /// </summary>
    /// <typeparam name="T">Localized model type</typeparam>
    public interface ILocalizedMvcModel<T> : ILocalizedModel where T : ILocalizedMvcLocaleModel
    {
        public IList<T> Locales { get; set; }
    }

    /// <summary>
    /// Represents localized locale model
    /// </summary>
    public interface ILocalizedMvcLocaleModel
    {
        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        string LanguageId { get; set; }
    }

    #endregion
}

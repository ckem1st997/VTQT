namespace VTQT.Caching
{
    public static partial class LocalizationCacheKeys
    {
        #region Locales

        /// <summary>
        /// Gets a prefix of locale resources for enumerations 
        /// </summary>
        public static string EnumLocaleStringResourcesPrefix => "Enums";

        #endregion

        #region Caching defaults

        #region Languages

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : app ID
        /// {1} : show hidden records?
        /// </remarks>
        public static string LanguagesAllCacheKey => "xbase.language.all-{0}-{1}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : app ID
        /// </remarks>
        public static string LanguagesByAppPrefix => "xbase.language.all.{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string LanguagesAllPrefix => "xbase.language";

        #endregion

        #region Locales

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static string LocaleStringResourcesAllCacheKey => "xbase.localestringresource.bylanguage-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : resource key
        /// </remarks>
        public static string LocaleStringResourcesByNameCacheKey => "xbase.localestringresource.byname.{0}-{1}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static string LocaleStringResourcesByNamePrefix => "xbase.localestringresource.byname.{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string LocaleStringResourcesAllPrefix => "xbase.localestringresource";

        #endregion

        #region Localized properties

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : entity ID
        /// {2} : locale key group
        /// {3} : locale key
        /// </remarks>
        public static string LocalizedPropertyCacheKey => "xbase.localizedproperty.value-{0}-{1}-{2}-{3}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string LocalizedPropertyAllCacheKey => "xbase.localizedproperty.all";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string LocalizedPropertyAllPrefix => "xbase.localizedproperty";

        #endregion

        #endregion
    }
}

namespace VTQT.Caching.Helpers.CacheKeys
{
    /// <summary>
    /// Represents default values related to apps services
    /// </summary>
    public static partial class AppCacheKeys
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string AppsAllCacheKey => "xbase.app.all";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string AppsAllPrefix => "xbase.app";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// </remarks>
        public static string AppMappingIdsCacheKey => "xbase.appmapping.ids.{0}-{1}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// </remarks>
        public static string AppMappingsCacheKey => "xbase.appmapping.{0}-{1}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity name
        /// </remarks>
        public static string AppMappingExistsCacheKey => "xbase.appmapping.exists.{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string AppMappingsAllPrefix => "xbase.appmapping";

        #endregion
    }
}

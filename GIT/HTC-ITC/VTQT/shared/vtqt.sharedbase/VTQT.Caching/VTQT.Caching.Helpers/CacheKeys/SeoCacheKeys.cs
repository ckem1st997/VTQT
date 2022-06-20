namespace VTQT.Caching
{
    public static partial class SeoCacheKeys
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string UrlRecordAllCacheKey => "xbase.urlrecord.all";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// {2} : language ID
        /// </remarks>
        public static string UrlRecordCacheKey => "xbase.urlrecord.{0}-{1}-{2}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : slug
        /// </remarks>
        public static string UrlRecordBySlugCacheKey => "xbase.urlrecord.byslug.{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string UrlRecordAllPrefix => "xbase.urlrecord";

        #endregion
    }
}

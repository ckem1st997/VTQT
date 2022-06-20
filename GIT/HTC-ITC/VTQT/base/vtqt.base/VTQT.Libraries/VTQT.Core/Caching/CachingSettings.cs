using VTQT.Core.Configuration;

namespace VTQT.Caching
{
    /// <summary>
    /// Caching settings
    /// </summary>
    public class CachingSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the default cache time in minutes
        /// </summary>
        public int DefaultCacheTime { get; set; } = CachingDefaults.CacheTime;

        /// <summary>
        /// Gets or sets the short term cache time in minutes
        /// </summary>
        public int ShortTermCacheTime { get; set; } = CachingDefaults.ShortTermCacheTime;
    }
}

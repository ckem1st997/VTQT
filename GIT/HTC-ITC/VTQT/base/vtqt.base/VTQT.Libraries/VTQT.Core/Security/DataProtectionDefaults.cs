namespace VTQT.Core.Security
{
    /// <summary>
    /// Represents default values related to data protection
    /// </summary>
    public static partial class DataProtectionDefaults
    {
        /// <summary>
        /// Gets the key used to store the protection key list to Redis (used with the PersistDataProtectionKeysToRedis option enabled)
        /// </summary>
        public static string RedisDataProtectionKey => "XBase.DataProtectionKeys";
    }
}

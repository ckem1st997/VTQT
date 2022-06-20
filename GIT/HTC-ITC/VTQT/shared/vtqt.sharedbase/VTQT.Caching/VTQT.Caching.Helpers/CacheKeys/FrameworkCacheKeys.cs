namespace VTQT.Caching
{
    public static partial class FrameworkCacheKeys
    {
        /// <summary>
        /// Key for user
        /// </summary>
        /// <remarks>
        /// {0} : Current User Id
        /// </remarks>
        public const string UserCacheKey = "xbase.fwcache.user-{0}";
        public const string UserPrefix = "xbase.fwcache.user";

        /// <summary>
        /// Key for user token
        /// </summary>
        /// <remarks>
        /// {0} : Current User Id
        /// </remarks>
        public const string UserTokenCacheKey = "xbase.fwcache.usertoken-{0}";
        public const string UserTokenPrefix = "xbase.fwcache.usertoken";

        /// <summary>
        /// Key for user permissions
        /// </summary>
        /// <remarks>
        /// {0} : Current User Id
        /// </remarks>
        public static string UserPermissionsCacheKey => "xbase.fwcache.userpermission-{0}";
        public static string UserPermissionsPrefix => "xbase.fwcache.userpermission";
    }
}

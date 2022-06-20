namespace VTQT.Caching
{
    public static partial class FbmContractCacheKeys
    {
        public static class Projects
        {
            // {0}: showHidden
            public static string AllCacheKey => "xbase.project.all-{0}";
            public static string Prefix => "xbase.project.";
        }
    }
}

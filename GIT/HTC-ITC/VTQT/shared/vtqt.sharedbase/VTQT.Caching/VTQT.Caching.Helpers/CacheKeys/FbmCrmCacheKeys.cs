namespace VTQT.Caching
{
    public static partial class FbmCrmCacheKeys
    {
        public static class Customers
        {
            // {0}: showHidden
            public static string AllCacheKey => "xbase.customer.all-{0}";
            public static string Prefix => "xbase.customer.";
        }
    }
}

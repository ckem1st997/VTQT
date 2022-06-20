namespace VTQT.Caching
{
    public static partial class WarehouseCacheKeys
    {
        public static class BeginningWareHouse
        {
            public static string AllCacheKey => "xbase.beginning-warehouse.all";
            public static string Prefix => "xbase.beginning-warehouse.";
        }

        public static class Unit
        {
            // {0}: showHidden
            public static string AllCacheKey => "xbase.unit.all-{0}";
            public static string Prefix => "xbase.unit.";
        }

        public static class Vendor
        {
            // {0}: showHidden
            public static string AllCacheKey => "xbase.vendor.all-{0}";
            public static string Prefix => "xbase.vendor.";
        }

        public static class Warehouses
        {
            // {0}: showHidden
            public static string AllCacheKey => "xbase.warehouse.all-{0}";
            public static string Prefix => "xbase.warehouse.";
        }

        public static class WareHouseItem
        {
            // {0}: showHidden
            public static string AllCacheKey => "xbase.warehouse-item.all-{0}";
            public static string Prefix => "xbase.warehouse-item.";
        }

        public static class WareHouseItemCategory
        {
            // {0}: showHidden
            public static string AllCacheKey => "xbase.warehouse-item-category.all-{0}";
            public static string Prefix => "xbase.warehouse-item-category.";
        }

        public static class WareHouseItemUnit
        {
            public static string AllCacheKey => "xbase.warehouse-item-unit.all";
            public static string Prefix => "xbase.warehouse-item-unit.";
        }

        public static class WareHouseLimit
        {
            // {0}: showHidden
            public static string AllCacheKey => "xbase.warehouse-limit.all";
            public static string Prefix => "xbase.warehouse-limit.";
        }
    }
}

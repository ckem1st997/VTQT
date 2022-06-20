namespace VTQT.Caching
{
    public static partial class MasterCacheKeys
    {
        public static class Banks
        {
            // {0}: showHidden
            public static string AllCacheKey => "xbase.bank.all-{0}";
            public static string Prefix => "xbase.bank.";
        }

        public static class BankAccounts
        {
            // {0}: showHidden
            public static string AllCacheKey => "xbase.bankaccount.all-{0}";
            public static string Prefix => "xbase.bankaccount.";
        }

        public static class BankBranches
        {
            // {0}: bankId
            public static string ByBankIdCacheKey => "xbase.bankbranch.bybankid-{0}";
            public static string Prefix => "xbase.bankbranch.";
        }

        public static class Countries
        {
            // {0}: showHidden
            public static string AllCacheKey => "xbase.country.all-{0}";
            public static string Prefix => "xbase.country.";
        }

        public static class Currencies
        {
            // {0}: showHidden
            public static string AllCacheKey => "xbase.currency.all-{0}";
            public static string Prefix => "xbase.currency.";
        }

        public static class Districts
        {
            // {0}: stateProvinceId
            public static string ByStateProvinceIdCacheKey => "xbase.district.bystateprovinceid-{0}";
            public static string Prefix => "xbase.district.";
        }

        public static class MeasureDimensions
        {
            // {0}: showHidden
            public static string AllCacheKey => "xbase.measuredimension.all-{0}";
            public static string Prefix => "xbase.measuredimension.";
        }

        public static class MeasureWeights
        {
            // {0}: showHidden
            public static string AllCacheKey => "xbase.measureweight.all-{0}";
            public static string Prefix => "xbase.measureweight.";
        }

        public static class StateProvinces
        {
            // {0}: countryId
            public static string ByCountryIdCacheKey => "xbase.stateprovince.bycountryid-{0}";
            public static string Prefix => "xbase.stateprovince.";
        }

        public static class Wards
        {
            // {0}: districtId
            public static string ByDistrictIdCacheKey => "xbase.ward.bydistrictid-{0}";
            public static string Prefix => "xbase.ward.";
        }

        public static class Warehouses
        {
            // {0}: showHidden
            public static string AllCacheKey => "xbase.warehouse.all-{0}";
            public static string Prefix => "xbase.warehouse.";
        }

        #region System

        public static class OrganizationUnits
        {
            // {0}: showHidden
            public static string AllCacheKey => "xbase.organizationunit.all-{0}";
            public static string Prefix => "xbase.organizationunit.";
        }

        public static class AppActions
        {
            // {0}: showHidden
            public static string AllCacheKey => "xbase.appaction.all-{0}";
            public static string Prefix => "xbase.appaction.";
        }

        public static class Users
        {
            // {0}: showHidden
            public static string AllCacheKey => "xbase.user.all-{0}";
            public static string Prefix => "xbase.user.";
        }

        public static class Roles
        {
            // {0}: showHidden
            public static string AllCacheKey => "xbase.role.all-{0}";
            public static string Prefix => "xbase.role.";
        }

        public static class Settings
        {
            /// <summary>
            /// Gets a key for caching
            /// </summary>
            public static string AllAsDictionaryCacheKey => "xbase.setting.all.as.dictionary";

            /// <summary>
            /// Gets a key for caching
            /// </summary>
            public static string AllCacheKey => "xbase.setting.all";

            /// <summary>
            /// Gets a key pattern to clear cache
            /// </summary>
            public static string Prefix => "xbase.setting.";
        }

        #endregion
    }
}

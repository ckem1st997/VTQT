namespace VTQT.Caching
{
    public static partial class ModelCacheKeys
    {
        /// <summary>
        /// Key for all apps
        /// </summary>
        /// <remarks>
        /// {0} : Language Id
        /// </remarks>
        public const string AppsModelCacheKey = "xbase.modelcache.app.all-{0}";
        public const string AppsPrefix = "xbase.modelcache.app.";

        /// <summary>
        /// Key for available app actions
        /// </summary>
        /// <remarks>
        /// {0} : App Id
        /// {1} : Language Id
        /// {2} : Show hidden
        /// </remarks>
        public static string AppActionsModelCacheKey => "xbase.modelcache.appaction.all-{0}-{1}-{2}";
        /// <summary>
        /// Key for available app actions
        /// </summary>
        /// <remarks>
        /// {0} : App Id
        /// {1} : Language Id
        /// {2} : Expand level
        /// {3} : Show hidden
        /// </remarks>
        public static string AppActionTreeModelCacheKey => "xbase.modelcache.appaction.tree-{0}-{1}-{2}-{3}";
        public static string AppActionsPrefix => "xbase.modelcache.appaction.";

        /// <summary>
        /// Key for available currencies
        /// </summary>
        /// <remarks>
        /// {0} : Language Id
        /// {1} : Current app Id
        /// </remarks>
        public const string CurrenciesModelCacheKey = "xbase.modelcache.currency.all-{0}-{1}";
        public const string CurrenciesPrefix = "xbase.modelcache.currency.";

        /// <summary>
        /// Key for available languages
        /// </summary>
        /// <remarks>
        /// {0} : Current app Id
        /// </remarks>
        public const string LanguagesModelCacheKey = "xbase.modelcache.language.all-{0}";
        public const string LanguagesPrefix = "xbase.modelcache.language.";

        /// <summary>
        /// Key for available organizational units
        /// </summary>
        /// <remarks>
        /// {0} : Language Id
        /// {1} : Show hidden
        /// </remarks>
        public const string OrganizationalUnitsModelCacheKey = "xbase.modelcache.organizational-unit.all-{0}-{1}";
        /// <summary>
        /// Key for available organizational units
        /// </summary>
        /// <remarks>
        /// {0} : Language Id
        /// {1} : Expand level
        /// {2} : Show hidden
        /// </remarks>
        public const string OrganizationalUnitTreeModelCacheKey = "xbase.modelcache.organizational-unit.tree-{0}-{1}-{2}";
        public const string OrganizationalUnitsPrefix = "xbase.modelcache.organizational-unit.";

        /// <summary>
        /// Key for available other lists
        /// </summary>
        /// <remarks>
        /// {0} : Language Id
        /// </remarks>
        public const string OtherListsModelCacheKey = "xbase.modelcache.other-list.all-{0}";
        public const string OtherListsPrefix = "xbase.modelcache.other-list.";

        /// <summary>
        /// Key for available other list items
        /// </summary>
        /// <remarks>
        /// {0} : OtherList code
        /// {1} : Language Id
        /// {2} : Show hidden
        /// </remarks>
        public const string OtherListItemsByOtherListCodeModelCacheKey = "xbase.modelcache.other-list-item-by-other-list-code.all-{0}-{1}-{2}";
        public const string OtherListItemsByOtherListCodePrefix = "xbase.modelcache.other-list-item-by-other-list-code.";

        /// <summary>
        /// Key for admin menus
        /// </summary>
        /// <remarks>
        /// {0} : Current account Id
        /// {1} : Current app Id
        /// {2} : Language Id
        /// </remarks>
        public const string AdminMenusModelCacheKey = "xbase.modelcache.admin.menus-{0}-{1}-{2}";
        /// <summary>
        /// Key for admin menus by account
        /// </summary>
        /// <remarks>
        /// {0} : Current account Id
        /// </remarks>
        public const string AdminMenusByAccountPrefix = "xbase.modelcache.admin.menu-{0}";
        public const string AdminMenusPrefix = "xbase.modelcache.admin.menu";

        /// <summary>
        /// Key for admin breadcrumbs
        /// </summary>
        /// <remarks>
        /// {0} : App Type
        /// {1} : App controller
        /// {2} : App action
        /// {3} : Language Id
        /// </remarks>
        public const string AdminBreadcrumbsModelCacheKey = "xbase.modelcache.admin.breadcrumbs-{0}-{1}-{2}-{3}";
        public const string AdminBreadcrumbsPrefix = "xbase.modelcache.admin.breadcrumbs";

        // {0}: Show hidden
        public const string UsersModelCacheKey = "xbase.modelcache.user.all-{0}";
        // {0}: Show hidden
        public const string UsersMvcListItemsModelCacheKey = "xbase.modelcache.user.mvclistitems.all-{0}";
        public const string UsersPrefix = "xbase.modelcache.user.";

        /// <summary>
        /// Key for departments tree
        /// </summary>
        /// <remarks>
        /// {0} : App Id
        /// {1} : User Id
        /// {2} : Path
        /// </remarks>
        public const string DepartmentsTreeModelCacheKey = "xbase.modelcache.departments.tree-{0}-{1}-{2}";
        public const string DepartmentsTreePrefix = "xbase.modelcache.departments.tree";

        /// <summary>
        /// Key for departments tree all
        /// </summary>
        /// <remarks>
        /// {0} : showHidden
        /// </remarks>
        public const string DepartmentsTreeAllModelCacheKey = "xbase.modelcache.departments.tree.all-{0}";
        public const string DepartmentsTreeAllPrefix = "xbase.modelcache.departments.tree.";

        /// <summary>
        /// Key for warehouses tree
        /// </summary>
        /// <remarks>
        /// {0} : App Id
        /// {1} : User Id
        /// {2} : Path
        /// </remarks>
        public const string WarehousesTreeModelCacheKey = "xbase.modelcache.warehouses.tree-{0}-{1}-{2}";
        public const string WarehousesTreePrefix = "xbase.modelcache.warehouses.tree";

        /// <summary>
        /// Key for warehouses tree all
        /// </summary>
        /// <remarks>
        /// {0} : showHidden
        /// </remarks>
        public const string WarehousesTreeAllModelCacheKey = "xbase.modelcache.warehouses.tree.all-{0}";
        public const string WarehousesTreeAllPrefix = "xbase.modelcache.warehouses.tree.";
        
        /// <summary>
        /// Key for typevalues tree
        /// </summary>
        /// <remarks>
        /// {0} : App Id
        /// {1} : User Id
        /// {2} : Path
        /// </remarks>
        public const string TypeValuesTreeModelCacheKey = "xbase.modelcache.typevalues.tree-{0}-{1}-{2}";
        public const string TypeValuesTreePrefix = "xbase.modelcache.typevalues.tree";
    }
}

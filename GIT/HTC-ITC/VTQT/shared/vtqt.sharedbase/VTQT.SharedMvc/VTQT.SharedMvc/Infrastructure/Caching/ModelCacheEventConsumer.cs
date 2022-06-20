using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Core.Domain.Master;
using VTQT.Core.Domain.Security;
using VTQT.Core.Events;
using VTQT.Services.Events;

namespace VTQT.SharedMvc.Infrastructure.Caching
{
    public partial class ModelCacheEventConsumer :
        // App
        IConsumer<EntityInsertedEvent<App>>,
        IConsumer<EntityUpdatedEvent<App>>,
        IConsumer<EntityDeletedEvent<App>>,
        // AppAction
        IConsumer<EntityInsertedEvent<AppAction>>,
        IConsumer<EntityUpdatedEvent<AppAction>>,
        IConsumer<EntityDeletedEvent<AppAction>>,
        // Role
        IConsumer<EntityInsertedEvent<Role>>,
        IConsumer<EntityUpdatedEvent<Role>>,
        IConsumer<EntityDeletedEvent<Role>>,
        // User
        IConsumer<EntityInsertedEvent<User>>,
        IConsumer<EntityUpdatedEvent<User>>,
        IConsumer<EntityDeletedEvent<User>>,
        // Currency
        IConsumer<EntityInsertedEvent<Currency>>,
        IConsumer<EntityUpdatedEvent<Currency>>,
        IConsumer<EntityDeletedEvent<Currency>>,
        // Language
        IConsumer<EntityInsertedEvent<Language>>,
        IConsumer<EntityUpdatedEvent<Language>>,
        IConsumer<EntityDeletedEvent<Language>>,
        //// OrganizationalUnit
        //IConsumer<EntityInsertedEvent<OrganizationalUnit>>,
        //IConsumer<EntityUpdatedEvent<OrganizationalUnit>>,
        //IConsumer<EntityDeletedEvent<OrganizationalUnit>>,
        //// OtherList
        //IConsumer<EntityInsertedEvent<OtherList>>,
        //IConsumer<EntityUpdatedEvent<OtherList>>,
        //IConsumer<EntityDeletedEvent<OtherList>>,
        //// OtherListItem
        //IConsumer<EntityInsertedEvent<OtherListItem>>,
        //IConsumer<EntityUpdatedEvent<OtherListItem>>,
        //IConsumer<EntityDeletedEvent<OtherListItem>>,
        // Setting
        IConsumer<EntityUpdatedEvent<Setting>>
    {
        #region Fields

        protected readonly IXBaseCacheManager _cacheManager;

        #endregion

        #region Ctor

        public ModelCacheEventConsumer(
            IXBaseCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        // App
        public async Task HandleEventAsync(EntityInsertedEvent<App> eventMessage)
        {
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AppsPrefix);
        }
        public async Task HandleEventAsync(EntityUpdatedEvent<App> eventMessage)
        {
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AppsPrefix);
        }
        public async Task HandleEventAsync(EntityDeletedEvent<App> eventMessage)
        {
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AppActionsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminMenusPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminBreadcrumbsPrefix);
        }

        // AppAction
        public async Task HandleEventAsync(EntityInsertedEvent<AppAction> eventMessage)
        {
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AppActionsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminMenusPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminBreadcrumbsPrefix);
        }
        public async Task HandleEventAsync(EntityUpdatedEvent<AppAction> eventMessage)
        {
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AppActionsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminMenusPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminBreadcrumbsPrefix);
        }
        public async Task HandleEventAsync(EntityDeletedEvent<AppAction> eventMessage)
        {
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AppActionsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminMenusPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminBreadcrumbsPrefix);
        }

        // Role
        public async Task HandleEventAsync(EntityInsertedEvent<Role> eventMessage)
        {
            await Task.CompletedTask;
        }
        public async Task HandleEventAsync(EntityUpdatedEvent<Role> eventMessage)
        {
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminMenusPrefix);
        }
        public async Task HandleEventAsync(EntityDeletedEvent<Role> eventMessage)
        {
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminMenusPrefix);
        }

        // User
        public async Task HandleEventAsync(EntityInsertedEvent<User> eventMessage)
        {
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.UsersPrefix);
        }
        public async Task HandleEventAsync(EntityUpdatedEvent<User> eventMessage)
        {
            var e = eventMessage.Entity;

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminMenusByAccountPrefix.FormatWith(e.Id));
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.UsersPrefix);
        }
        public async Task HandleEventAsync(EntityDeletedEvent<User> eventMessage)
        {
            var e = eventMessage.Entity;

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminMenusByAccountPrefix.FormatWith(e.Id));
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.UsersPrefix);
        }

        // Currency
        public async Task HandleEventAsync(EntityInsertedEvent<Currency> eventMessage)
        {
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.CurrenciesPrefix);
        }
        public async Task HandleEventAsync(EntityUpdatedEvent<Currency> eventMessage)
        {
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.CurrenciesPrefix);
        }
        public async Task HandleEventAsync(EntityDeletedEvent<Currency> eventMessage)
        {
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.CurrenciesPrefix);
        }

        // Language
        public async Task HandleEventAsync(EntityInsertedEvent<Language> eventMessage)
        {
            // Clear all localizable models
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.LanguagesPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.CurrenciesPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.OtherListsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.OtherListItemsByOtherListCodePrefix);

            // Admin
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AppActionsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.OrganizationalUnitsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminMenusPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminBreadcrumbsPrefix);
        }
        public async Task HandleEventAsync(EntityUpdatedEvent<Language> eventMessage)
        {
            // Clear all localizable models
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.LanguagesPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.CurrenciesPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.OtherListsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.OtherListItemsByOtherListCodePrefix);

            // Admin
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AppActionsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.OrganizationalUnitsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminMenusPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminBreadcrumbsPrefix);
        }
        public async Task HandleEventAsync(EntityDeletedEvent<Language> eventMessage)
        {
            // Clear all localizable models
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.LanguagesPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.CurrenciesPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.OtherListsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.OtherListItemsByOtherListCodePrefix);

            // Admin
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AppActionsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.OrganizationalUnitsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminMenusPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminBreadcrumbsPrefix);
        }

        // Setting
        public async Task HandleEventAsync(EntityUpdatedEvent<Setting> eventMessage)
        {
            await Task.CompletedTask;
        }

        #endregion


    }
}

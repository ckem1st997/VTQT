using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Core.Domain.Master;
using VTQT.Core.Domain.Security;
using VTQT.Core.Events;
using VTQT.Services.Events;

namespace VTQT.Web.Framework.Infrastructure.Caching
{
    public partial class FrameworkCacheEventConsumer :
        // App
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
        //// UserBase
        //IConsumer<EntityInsertedEvent<UserBase>>,
        //IConsumer<EntityUpdatedEvent<UserBase>>,
        //IConsumer<EntityDeletedEvent<UserBase>>,
        // Setting
        IConsumer<EntityUpdatedEvent<Setting>>
    {
        #region Fields

        protected readonly IXBaseCacheManager _cacheManager;

        #endregion

        #region Ctor

        public FrameworkCacheEventConsumer(
            IXBaseCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        // App
        public async Task HandleEventAsync(EntityUpdatedEvent<App> eventMessage)
        {
            await Task.CompletedTask;
        }
        public async Task HandleEventAsync(EntityDeletedEvent<App> eventMessage)
        {
            // Admin
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(FrameworkCacheKeys.UserPermissionsPrefix);
        }

        // AppAction
        public async Task HandleEventAsync(EntityInsertedEvent<AppAction> eventMessage)
        {
            await Task.CompletedTask;
        }
        public async Task HandleEventAsync(EntityUpdatedEvent<AppAction> eventMessage)
        {
            // Admin
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(FrameworkCacheKeys.UserPermissionsPrefix);
        }
        public async Task HandleEventAsync(EntityDeletedEvent<AppAction> eventMessage)
        {
            // Admin
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(FrameworkCacheKeys.UserPermissionsPrefix);
        }

        // Role
        public async Task HandleEventAsync(EntityInsertedEvent<Role> eventMessage)
        {
            await Task.CompletedTask;
        }
        public async Task HandleEventAsync(EntityUpdatedEvent<Role> eventMessage)
        {
            // Admin
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(FrameworkCacheKeys.UserPermissionsPrefix);
        }
        public async Task HandleEventAsync(EntityDeletedEvent<Role> eventMessage)
        {
            // Admin
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(FrameworkCacheKeys.UserPermissionsPrefix);
        }

        // User
        public async Task HandleEventAsync(EntityInsertedEvent<User> eventMessage)
        {
            await Task.CompletedTask;
        }
        public async Task HandleEventAsync(EntityUpdatedEvent<User> eventMessage)
        {
            var e = eventMessage.Entity;

            await _cacheManager.HybridProvider.RemoveAsync(FrameworkCacheKeys.UserCacheKey.FormatWith(e.Id));

            // Admin
            await _cacheManager.HybridProvider.RemoveAsync(FrameworkCacheKeys.UserPermissionsCacheKey.FormatWith(e.Id));
        }
        public async Task HandleEventAsync(EntityDeletedEvent<User> eventMessage)
        {
            var e = eventMessage.Entity;

            await _cacheManager.HybridProvider.RemoveAsync(FrameworkCacheKeys.UserCacheKey.FormatWith(e.Id));

            // Admin
            await _cacheManager.HybridProvider.RemoveAsync(FrameworkCacheKeys.UserPermissionsCacheKey.FormatWith(e.Id));
        }

        // Setting
        public async Task HandleEventAsync(EntityUpdatedEvent<Setting> eventMessage)
        {
            await Task.CompletedTask;
        }

        #endregion
    }
}

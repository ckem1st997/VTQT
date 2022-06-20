using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Core.Events;
using VTQT.Services.Events;

namespace VTQT.Services.Caching
{
    public abstract partial class ServiceCacheEventConsumer<TEntity> :
        IConsumer<EntityInsertedEvent<TEntity>>,
        IConsumer<EntityUpdatedEvent<TEntity>>,
        IConsumer<EntityDeletedEvent<TEntity>>
        where TEntity : BaseEntity
    {
        #region Fields

        protected readonly IXBaseCacheManager _cacheManager;

        #endregion

        #region Ctor

        protected ServiceCacheEventConsumer(IXBaseCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task ClearCacheAsync(TEntity entity, EntityEventType entityEventType)
        {
            await RemoveByPrefixAsync(CachingDefaults.EntityPrefix.FormatWith(typeof(TEntity).Name.ToLowerInvariant()));

            //if (entityEventType != EntityEventType.Insert)
            //    await RemoveByPrefixAsync(BaseEntity.GetEntityCacheKey(typeof(TEntity), entity.Id));

            await ClearCacheAsync(entity);
        }

        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual Task ClearCacheAsync(TEntity entity)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes items by key prefix
        /// </summary>
        /// <param name="prefixCacheKey">String key prefix</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task RemoveByPrefixAsync(string prefixCacheKey)
        {
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(prefixCacheKey);
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="cacheKey">Key of cached item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task RemoveAsync(string cacheKey)
        {
            await _cacheManager.HybridProvider.RemoveAsync(cacheKey);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle entity inserted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public virtual async Task HandleEventAsync(EntityInsertedEvent<TEntity> eventMessage)
        {
            var entity = eventMessage.Entity;
            await ClearCacheAsync(entity, EntityEventType.Insert);
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public virtual async Task HandleEventAsync(EntityUpdatedEvent<TEntity> eventMessage)
        {
            var entity = eventMessage.Entity;
            await ClearCacheAsync(eventMessage.Entity, EntityEventType.Update);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public virtual async Task HandleEventAsync(EntityDeletedEvent<TEntity> eventMessage)
        {
            var entity = eventMessage.Entity;
            await ClearCacheAsync(eventMessage.Entity, EntityEventType.Delete);
        }

        #endregion

        #region Nested

        protected enum EntityEventType
        {
            Insert,
            Update,
            Delete
        }

        #endregion
    }
}

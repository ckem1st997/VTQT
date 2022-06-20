using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Caching.Helpers.CacheKeys;
using VTQT.Core;
using VTQT.Core.Configuration;
using VTQT.Core.Domain.Apps;
using VTQT.Core.Domain.Common;
using VTQT.Core.Domain.Master;
using VTQT.Core.Events;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Apps
{
    /// <summary>
    /// Store mapping service
    /// </summary>
    public partial class AppMappingService : IAppMappingService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IRepository<AppMapping> _appMappingRepository;
        private readonly IXBaseCacheManager _cacheManager;
        private readonly IAppContext _appContext;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public AppMappingService(CatalogSettings catalogSettings,
            IXBaseCacheManager cacheManager,
            IAppContext appContext,
            IEventPublisher eventPublisher)
        {
            _catalogSettings = catalogSettings;
            _appMappingRepository = EngineContext.Current.Resolve<IRepository<AppMapping>>(DataConnectionHelper.ConnectionStringNames.Master);
            _cacheManager = cacheManager;
            _appContext = appContext;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Inserts a store mapping record
        /// </summary>
        /// <param name="appMapping">Store mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InsertAppMappingAsync(AppMapping appMapping)
        {
            await _appMappingRepository.InsertAsync(appMapping);

            //event notification
            await _eventPublisher.EntityInsertedAsync(appMapping);
        }

        /// <summary>
        /// Get a value indicating whether a store mapping exists for an entity type
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if exists; otherwise false
        /// </returns>
        protected virtual async Task<bool> IsEntityMappingExistsAsync<TEntity>() where TEntity : BaseEntity, IAppMappingSupported
        {
            var entityName = typeof(TEntity).Name;
            var key = AppCacheKeys.AppMappingExistsCacheKey.FormatWith(entityName);

            var query = from sm in _appMappingRepository.Table
                        where sm.EntityName == entityName
                        select sm.AppId;

            return await _cacheManager.GetDbAsync(key, async () => await query.AnyAsync(), CachingDefaults.MonthCacheTime);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Apply store mapping to the passed query
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="query">Query to filter</param>
        /// <param name="appId">Store identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the filtered query
        /// </returns>
        public virtual async Task<IQueryable<TEntity>> ApplyAppMapping<TEntity>(IQueryable<TEntity> query, string appId)
            where TEntity : BaseEntity, IAppMappingSupported
        {
            if (query is null)
                throw new ArgumentNullException(nameof(query));

            if (appId == SettingDefaults.AllAppsId || _catalogSettings.IgnoreAppLimitations || !await IsEntityMappingExistsAsync<TEntity>())
                return query;

            return from entity in query
                   where !entity.LimitedToApps || _appMappingRepository.Table.Any(sm =>
                         sm.EntityName == typeof(TEntity).Name && sm.EntityId == entity.Id && sm.AppId == appId)
                   select entity;
        }

        /// <summary>
        /// Deletes a store mapping record
        /// </summary>
        /// <param name="appMapping">Store mapping record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteAppMappingAsync(AppMapping appMapping)
        {
            await _appMappingRepository.DeleteAsync(appMapping);

            //event notification
            await _eventPublisher.EntityDeletedAsync(appMapping);
        }

        /// <summary>
        /// Gets store mapping records
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the store mapping records
        /// </returns>
        public virtual async Task<IList<AppMapping>> GetAppMappingsAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAppMappingSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var key = AppCacheKeys.AppMappingsCacheKey.FormatWith(entityId, entityName);

            var query = from sm in _appMappingRepository.Table
                        where sm.EntityId == entityId &&
                        sm.EntityName == entityName
                        select sm;

            var appMappings = await _cacheManager.GetDbAsync(key, async () => await query.ToListAsync(), CachingDefaults.MonthCacheTime);

            return appMappings;
        }

        /// <summary>
        /// Inserts a store mapping record
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="appId">Store id</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertAppMappingAsync<TEntity>(TEntity entity, string appId) where TEntity : BaseEntity, IAppMappingSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (appId == SettingDefaults.AllAppsId)
                throw new ArgumentOutOfRangeException(nameof(appId));

            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var storeMapping = new AppMapping
            {
                EntityId = entityId,
                EntityName = entityName,
                AppId = appId
            };

            await InsertAppMappingAsync(storeMapping);
        }

        /// <summary>
        /// Find store identifiers with granted access (mapped to the entity)
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the store identifiers
        /// </returns>
        public virtual async Task<IList<string>> GetAppsIdsWithAccessAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAppMappingSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var key = AppCacheKeys.AppMappingIdsCacheKey.FormatWith(entityId, entityName);

            var query = from sm in _appMappingRepository.Table
                        where sm.EntityId == entityId &&
                              sm.EntityName == entityName
                        select sm.AppId;

            return await _cacheManager.GetDbAsync(key, async () => await query.ToListAsync(), CachingDefaults.MonthCacheTime);
        }

        /// <summary>
        /// Find store identifiers with granted access (mapped to the entity)
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the store identifiers
        /// </returns>
        public virtual IList<string> GetAppsIdsWithAccess<TEntity>(TEntity entity) where TEntity : BaseEntity, IAppMappingSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var key = AppCacheKeys.AppMappingIdsCacheKey.FormatWith(entityId, entityName);

            var query = from sm in _appMappingRepository.Table
                        where sm.EntityId == entityId &&
                              sm.EntityName == entityName
                        select sm.AppId;

            return _cacheManager.GetDb(key, () => query.ToList(), CachingDefaults.MonthCacheTime);
        }

        /// <summary>
        /// Authorize whether entity could be accessed in the current store (mapped to this store)
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAppMappingSupported
        {
            return await AuthorizeAsync(entity, _appContext.CurrentApp.Id);
        }

        /// <summary>
        /// Authorize whether entity could be accessed in the current store (mapped to this store)
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        public virtual bool Authorize<TEntity>(TEntity entity) where TEntity : BaseEntity, IAppMappingSupported
        {
            return Authorize(entity, _appContext.CurrentApp.Id);
        }

        /// <summary>
        /// Authorize whether entity could be accessed in a store (mapped to this store)
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="appId">Store identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync<TEntity>(TEntity entity, string appId) where TEntity : BaseEntity, IAppMappingSupported
        {
            if (entity == null)
                return false;

            if (appId == SettingDefaults.AllAppsId)
                //return true if no store specified/found
                return true;

            if (_catalogSettings.IgnoreAppLimitations)
                return true;

            if (!entity.LimitedToApps)
                return true;

            foreach (var appIdWithAccess in await GetAppsIdsWithAccessAsync(entity))
                if (appId == appIdWithAccess)
                    //yes, we have such permission
                    return true;

            //no permission found
            return false;
        }

        /// <summary>
        /// Authorize whether entity could be accessed in a store (mapped to this store)
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="appId">Store identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        public virtual bool Authorize<TEntity>(TEntity entity, string appId) where TEntity : BaseEntity, IAppMappingSupported
        {
            if (entity == null)
                return false;

            if (appId == SettingDefaults.AllAppsId)
                //return true if no store specified/found
                return true;

            if (_catalogSettings.IgnoreAppLimitations)
                return true;

            if (!entity.LimitedToApps)
                return true;

            foreach (var appIdWithAccess in GetAppsIdsWithAccess(entity))
                if (appId == appIdWithAccess)
                    //yes, we have such permission
                    return true;

            //no permission found
            return false;
        }

        #endregion
    }
}

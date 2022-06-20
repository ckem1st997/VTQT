using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Caching.Helpers.CacheKeys;
using VTQT.Core;
using VTQT.Core.Domain.Master;
using VTQT.Core.Events;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Apps
{
    /// <summary>
    /// App service
    /// </summary>
    public partial class AppService : IAppService
    {
        #region Fields

        private readonly IRepository<App> _appRepository;
        private readonly IXBaseCacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public AppService(
            IXBaseCacheManager cacheManager,
            IEventPublisher eventPublisher)
        {
            _appRepository = EngineContext.Current.Resolve<IRepository<App>>(DataConnectionHelper.ConnectionStringNames.Master);
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Parse comma-separated Hosts
        /// </summary>
        /// <param name="app">App</param>
        /// <returns>Comma-separated hosts</returns>
        protected virtual string[] ParseHostValues(App app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            var parsedValues = new List<string>();
            if (string.IsNullOrEmpty(app.Hosts))
                return parsedValues.ToArray();

            var hosts = app.Hosts.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var host in hosts)
            {
                var tmp = host.Trim();
                if (!string.IsNullOrEmpty(tmp))
                    parsedValues.Add(tmp);
            }

            return parsedValues.ToArray();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a app
        /// </summary>
        /// <param name="app">App</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteAppAsync(App app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            var allApps = await GetAllAppsAsync();
            if (allApps.Count == 1)
                throw new Exception("You cannot delete the only configured app");

            await _appRepository.DeleteAsync(app);

            // Event notification
            await _eventPublisher.EntityDeletedAsync(app);
        }

        /// <summary>
        /// Gets all apps
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the apps
        /// </returns>
        public virtual async Task<IList<App>> GetAllAppsAsync()
        {
            var key = AppCacheKeys.AppsAllCacheKey;

            var result = await _cacheManager.GetDbAsync(key, async () =>
            {
                var query = from s in _appRepository.Table
                            orderby s.DisplayOrder, s.Id
                            select s;

                return await query.ToListAsync();
            }, CachingDefaults.MonthCacheTime);

            return result;
        }

        /// <summary>
        /// Gets all apps
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the apps
        /// </returns>
        public virtual IList<App> GetAllApps()
        {
            var key = AppCacheKeys.AppsAllCacheKey;

            var result = _cacheManager.GetDb(key, () =>
            {
                var query = from s in _appRepository.Table
                            orderby s.DisplayOrder, s.Id
                            select s;

                return query.ToList();
            }, CachingDefaults.MonthCacheTime);

            return result;
        }

        /// <summary>
        /// Gets a app 
        /// </summary>
        /// <param name="appId">App identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the app
        /// </returns>
        public virtual async Task<App> GetAppByIdAsync(string appId)
        {
            var key = BaseEntity.GetEntityCacheKey(typeof(App), appId);
            return await _cacheManager.GetDbAsync(key, async () => await _appRepository.GetByIdAsync(appId));
        }

        /// <summary>
        /// Gets a app 
        /// </summary>
        /// <param name="appId">App identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the app
        /// </returns>
        public virtual App GetAppById(string appId)
        {
            var key = BaseEntity.GetEntityCacheKey(typeof(App), appId);
            return _cacheManager.GetDb(key, () => _appRepository.GetById(appId));
        }

        /// <summary>
        /// Inserts a app
        /// </summary>
        /// <param name="app">App</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertAppAsync(App app)
        {
            await _appRepository.InsertAsync(app);

            // Event notification
            await _eventPublisher.EntityInsertedAsync(app);
        }

        /// <summary>
        /// Updates the app
        /// </summary>
        /// <param name="app">App</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateAppAsync(App app)
        {
            await _appRepository.UpdateAsync(app);

            // Event notification
            await _eventPublisher.EntityUpdatedAsync(app);
        }

        /// <summary>
        /// Indicates whether a app contains a specified host
        /// </summary>
        /// <param name="app">App</param>
        /// <param name="host">Host</param>
        /// <returns>true - contains, false - no</returns>
        public virtual bool ContainsHostValue(App app, string host)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            if (string.IsNullOrEmpty(host))
                return false;

            var contains = ParseHostValues(app).Any(x => x.Equals(host, StringComparison.InvariantCultureIgnoreCase));

            return contains;
        }

        /// <summary>
        /// Returns a list of names of not existing apps
        /// </summary>
        /// <param name="appIdsNames">The names and/or IDs of the app to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of names and/or IDs not existing apps
        /// </returns>
        public async Task<string[]> GetNotExistingAppsAsync(string[] appIdsNames)
        {
            if (appIdsNames == null)
                throw new ArgumentNullException(nameof(appIdsNames));

            var query = _appRepository.Table;
            var queryFilter = appIdsNames.Distinct().ToArray();
            //filtering by name
            var filter = await query.Select(app => app.Name)
                .Where(app => queryFilter.Contains(app))
                .ToListAsync();
            queryFilter = queryFilter.Except(filter).ToArray();

            //if some names not found
            if (!queryFilter.Any())
                return queryFilter.ToArray();

            //filtering by IDs
            filter = await query.Select(app => app.Id.ToString())
                .Where(app => queryFilter.Contains(app))
                .ToListAsync();
            queryFilter = queryFilter.Except(filter).ToArray();

            return queryFilter.ToArray();
        }

        #endregion
    }
}

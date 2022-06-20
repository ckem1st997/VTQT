using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Master;

namespace VTQT.Services.Apps
{
    public partial interface IAppService
    {
        /// <summary>
        /// Deletes a app
        /// </summary>
        /// <param name="app">App</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteAppAsync(App app);

        /// <summary>
        /// Gets all apps
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the apps
        /// </returns>
        Task<IList<App>> GetAllAppsAsync();

        /// <summary>
        /// Gets all apps
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the apps
        /// </returns>
        IList<App> GetAllApps();

        /// <summary>
        /// Gets a app 
        /// </summary>
        /// <param name="appId">App identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the app
        /// </returns>
        Task<App> GetAppByIdAsync(string appId);

        /// <summary>
        /// Gets a app 
        /// </summary>
        /// <param name="appId">App identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the app
        /// </returns>
        App GetAppById(string appId);

        /// <summary>
        /// Inserts a app
        /// </summary>
        /// <param name="app">App</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertAppAsync(App app);

        /// <summary>
        /// Updates the app
        /// </summary>
        /// <param name="app">App</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateAppAsync(App app);

        /// <summary>
        /// Indicates whether a app contains a specified host
        /// </summary>
        /// <param name="app">App</param>
        /// <param name="host">Host</param>
        /// <returns>true - contains, false - no</returns>
        bool ContainsHostValue(App app, string host);

        /// <summary>
        /// Returns a list of names of not existing apps
        /// </summary>
        /// <param name="appIdsNames">The names and/or IDs of the app to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of names and/or IDs not existing apps
        /// </returns>
        Task<string[]> GetNotExistingAppsAsync(string[] appIdsNames);
    }
}

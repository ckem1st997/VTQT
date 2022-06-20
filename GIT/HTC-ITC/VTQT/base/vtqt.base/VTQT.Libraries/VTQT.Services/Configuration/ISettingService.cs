using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VTQT.Core.Configuration;
using VTQT.Core.Domain.Master;

namespace VTQT.Services.Configuration
{
    /// <summary>
    /// Setting service interface
    /// </summary>
    public partial interface ISettingService
    {
        /// <summary>
        /// Gets a setting by identifier
        /// </summary>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setting
        /// </returns>
        Task<Setting> GetSettingByIdAsync(string settingId);

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteSettingAsync(Setting setting);

        /// <summary>
        /// Deletes settings
        /// </summary>
        /// <param name="settings">Settings</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteSettingsAsync(IList<Setting> settings);

        /// <summary>
        /// Get setting by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="appId">Store identifier</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setting
        /// </returns>
        Task<Setting> GetSettingAsync(string key, string appId = SettingDefaults.AllAppsId, bool loadSharedValueIfNotFound = false);

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="appId">Store identifier</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setting value
        /// </returns>
        Task<T> GetSettingByKeyAsync<T>(string key, T defaultValue = default,
            string appId = SettingDefaults.AllAppsId, bool loadSharedValueIfNotFound = false);

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="appId">Store identifier</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setting value
        /// </returns>
        T GetSettingByKey<T>(string key, T defaultValue = default,
            string appId = SettingDefaults.AllAppsId, bool loadSharedValueIfNotFound = false);

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="appId">Store identifier</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SetSettingAsync<T>(string key, T value, string appId = SettingDefaults.AllAppsId, bool clearCache = true);

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the settings
        /// </returns>
        Task<IList<Setting>> GetAllSettingsAsync();

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the settings
        /// </returns>
        IList<Setting> GetAllSettings();

        /// <summary>
        /// Determines whether a setting exists
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="appId">Store identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue -setting exists; false - does not exist
        /// </returns>
        Task<bool> SettingExistsAsync<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector, string appId = SettingDefaults.AllAppsId)
            where T : ISettings, new();

        /// <summary>
        /// Load settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="appId">Store identifier for which settings should be loaded</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<T> LoadSettingAsync<T>(string appId = SettingDefaults.AllAppsId) where T : ISettings, new();

        /// <summary>
        /// Load settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="appId">Store identifier for which settings should be loaded</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        T LoadSetting<T>(string appId = SettingDefaults.AllAppsId) where T : ISettings, new();

        /// <summary>
        /// Load settings
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="appId">Store identifier for which settings should be loaded</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<ISettings> LoadSettingAsync(Type type, string appId = SettingDefaults.AllAppsId);

        /// <summary>
        /// Load settings
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="appId">Store identifier for which settings should be loaded</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        ISettings LoadSetting(Type type, string appId = SettingDefaults.AllAppsId);

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="appId">Store identifier</param>
        /// <param name="settings">Setting instance</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SaveSettingAsync<T>(T settings, string appId = SettingDefaults.AllAppsId) where T : ISettings, new();

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="appId">Store ID</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SaveSettingAsync<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector,
            string appId = SettingDefaults.AllAppsId, bool clearCache = true) where T : ISettings, new();

        /// <summary>
        /// Save settings object (per store). If the setting is not overridden per store then it'll be delete
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="overrideForStore">A value indicating whether to setting is overridden in some store</param>
        /// <param name="appId">Store ID</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SaveSettingOverridablePerStoreAsync<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector,
            bool overrideForStore, string appId = SettingDefaults.AllAppsId, bool clearCache = true) where T : ISettings, new();

        /// <summary>
        /// Delete all settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteSettingAsync<T>() where T : ISettings, new();

        /// <summary>
        /// Delete settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="appId">Store ID</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteSettingAsync<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector, string appId = SettingDefaults.AllAppsId) where T : ISettings, new();

        Task DeleteSettingAsync(string key, string appId = SettingDefaults.AllAppsId);

        /// <summary>
        /// Clear cache
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ClearCacheAsync();

        //TODO: migrate to an extension method
        /// <summary>
        /// Get setting key (stored into database)
        /// </summary>
        /// <typeparam name="TSettings">Type of settings</typeparam>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <returns>Key</returns>
        string GetSettingKey<TSettings, T>(TSettings settings, Expression<Func<TSettings, T>> keySelector)
            where TSettings : ISettings, new();
    }
}

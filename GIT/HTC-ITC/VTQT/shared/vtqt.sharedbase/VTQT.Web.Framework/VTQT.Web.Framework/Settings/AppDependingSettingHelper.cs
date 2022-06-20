using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using VTQT.ComponentModel;
using VTQT.Core.Configuration;
using VTQT.Core.Infrastructure;
using VTQT.Services.Configuration;
using VTQT.Services.Localization;
using VTQT.Web.Framework.Modelling;

namespace VTQT.Web.Framework.Settings
{
    public class AppDependingSettingHelper
    {
        private ViewDataDictionary _viewData;

        public AppDependingSettingHelper(ViewDataDictionary viewData)
        {
            _viewData = viewData;
        }

        public static string ViewDataKey => nameof(AppDependingSettingData);

        public AppDependingSettingData Data
        {
            get
            {
                return _viewData[ViewDataKey] as AppDependingSettingData;
            }
        }

        private bool IsOverrideChecked(string settingKey, FormCollection form)
        {
            var rawOverrideKey = form.Keys.FirstOrDefault(k => k.IsCaseInsensitiveEqual(settingKey + "_OverrideForApp"));

            if (rawOverrideKey.HasValue())
            {
                var checkboxValue = ((string)form[rawOverrideKey]).EmptyNull().ToLower();
                return checkboxValue.Contains("on") || checkboxValue.Contains("true");
            }
            return false;
        }
        public bool IsOverrideChecked(object settings, string name, FormCollection form)
        {
            var key = settings.GetType().Name + "." + name;
            return IsOverrideChecked(key, form);
        }
        public void AddOverrideKey(object settings, string name)
        {
            var key = settings.GetType().Name + "." + name;
            Data.OverrideSettingKeys.Add(key);
        }
        public void CreateViewDataObject(string activeAppScopeConfiguration, string rootSettingClass = null)
        {
            _viewData[ViewDataKey] = new AppDependingSettingData()
            {
                ActiveAppScopeConfiguration = activeAppScopeConfiguration,
                RootSettingClass = rootSettingClass
            };
        }

        public async Task GetOverrideKeysAsync(object settings, object model, string appId, ISettingService settingService, bool isRootModel = true, ILocalizedMvcLocaleModel localized = null, int? index = null)
        {
            if (string.IsNullOrWhiteSpace(appId) || appId == SettingDefaults.AllAppsId)
                return;     // single app mode -> there are no overrides

            var data = Data;
            if (data == null)
                data = new AppDependingSettingData();

            var settingName = settings.GetType().Name;
            var properties = settings.GetType().GetProperties();
            var localizedEntityService = EngineContext.Current.Resolve<ILocalizedEntityService>();

            var modelType = model.GetType();

            foreach (var prop in properties)
            {
                var name = prop.Name;
                var modelProperty = modelType.GetProperty(name);

                if (modelProperty == null)
                    continue;   // setting is not configurable or missing or whatever... however we don't need the override info

                var key = String.Empty;
                var setting = String.Empty;

                if (localized == null)
                {
                    key = settingName + "." + name;
                    setting = settingService.GetSettingByKey<string>(key, appId: appId);
                }
                else
                {
                    key = "Locales[" + index.ToString() + "]." + name;
                    setting = await localizedEntityService.GetLocalizedValueAsync(localized.LanguageId, SettingDefaults.AllAppsId, settingName, name);
                }

                if (!String.IsNullOrEmpty(setting))
                    data.OverrideSettingKeys.Add(key);
            }

            if (isRootModel)
            {
                data.ActiveAppScopeConfiguration = appId;
                data.RootSettingClass = settingName;

                _viewData[ViewDataKey] = data;
            }
        }

        public async Task UpdateSettingsAsync(object settings, FormCollection form, string appId, ISettingService settingService, ILocalizedMvcLocaleModel localized = null)
        {
            var settingName = settings.GetType().Name;
            var properties = FastProperty.GetProperties(localized == null ? settings.GetType() : localized.GetType()).Values;

            foreach (var prop in properties)
            {
                var name = prop.Name;
                var key = settingName + "." + name;

                if (appId == SettingDefaults.AllAppsId || IsOverrideChecked(key, form))
                {
                    dynamic value = prop.GetValue(localized == null ? settings : localized);
                    await settingService.SetSettingAsync(key, value == null ? "" : value, appId, false);
                }
                else if (appId != SettingDefaults.AllAppsId)
                {
                    await settingService.DeleteSettingAsync(key, appId);
                }
            }
        }
    }
}

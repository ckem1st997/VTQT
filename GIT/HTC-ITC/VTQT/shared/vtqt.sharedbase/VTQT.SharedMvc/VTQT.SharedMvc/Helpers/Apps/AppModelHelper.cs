using System;
using System.Collections.Generic;
using System.Linq;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Services.Apps;
using VTQT.Services.Localization;
using VTQT.SharedMvc.Master.Models;

namespace VTQT.SharedMvc.Helpers
{
    public class AppModelHelper : IAppModelHelper
    {
        private readonly IAppService _appService;
        private readonly IWorkContext _workContext;
        private readonly IXBaseCacheManager _cacheManager;

        public AppModelHelper(
            IAppService appService,
            IWorkContext workContext,
            IXBaseCacheManager cacheManager)
        {
            _appService = appService;
            _workContext = workContext;
            _cacheManager = cacheManager;
        }

        public virtual List<AppModel> GetAllApiTypes()
        {
            var cacheKey = ModelCacheKeys.AppsModelCacheKey.FormatWith(_workContext.WorkingLanguage.Id);
            var result = _cacheManager.HybridProvider.Get(cacheKey, () =>
            {
                var models = _appService
                    .GetAllApps()
                    .Where(w => AppHelperBase.Apps.ApiTypes.Contains(w.AppType))
                    .Select(s => new AppModel
                    {
                        Id = s.Id,
                        AppType = s.AppType,
                        Name = s.GetLocalized(x => x.Name),
                        ShortName = s.GetLocalized(x => x.ShortName),
                        Description = s.Description,
                        Icon = s.Icon,
                        BackgroundColor = s.BackgroundColor,
                        Url = s.Url,
                        Hosts = s.Hosts,
                        SslEnabled = s.SslEnabled,
                        CdnUrl = s.CdnUrl,
                        DefaultLanguageId = s.DefaultLanguageId,
                        ShowOnMenu = s.ShowOnMenu,
                        DisplayOrder = s.DisplayOrder
                    })
                    .OrderBy(o => o.DisplayOrder)
                    .ThenBy(o => o.Name)
                    .ToList();

                return models;
            }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;

            return result;
        }

        public virtual List<AppModel> GetAllWebTypes()
        {
            var cacheKey = ModelCacheKeys.AppsModelCacheKey.FormatWith(_workContext.WorkingLanguage.Id);
            var result = _cacheManager.HybridProvider.Get(cacheKey, () =>
            {
                var models = _appService
                    .GetAllApps()
                    .Where(w => AppHelperBase.Apps.WebTypes.Contains(w.AppType))
                    .Select(s => new AppModel
                    {
                        Id = s.Id,
                        AppType = s.AppType,
                        Name = s.GetLocalized(x => x.Name),
                        ShortName = s.GetLocalized(x => x.ShortName),
                        Description = s.Description,
                        Icon = s.Icon,
                        BackgroundColor = s.BackgroundColor,
                        Url = s.Url,
                        Hosts = s.Hosts,
                        SslEnabled = s.SslEnabled,
                        CdnUrl = s.CdnUrl,
                        DefaultLanguageId = s.DefaultLanguageId,
                        ShowOnMenu = s.ShowOnMenu,
                        DisplayOrder = s.DisplayOrder
                    })
                    .OrderBy(o => o.DisplayOrder)
                    .ThenBy(o => o.Name)
                    .ToList();

                return models;
            }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;

            return result;
        }
    }
}

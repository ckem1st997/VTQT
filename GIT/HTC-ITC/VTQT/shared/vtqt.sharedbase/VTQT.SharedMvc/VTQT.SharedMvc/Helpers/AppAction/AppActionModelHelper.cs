using System;
using System.Collections.Generic;
using System.Linq;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Services.Localization;
using VTQT.Services.Security;
using VTQT.SharedMvc.Master.Models;

namespace VTQT.SharedMvc.Helpers
{
    public class AppActionModelHelper : IAppActionModelHelper
    {
        private readonly IAppActionService _appActionService;
        private readonly IWorkContext _workContext;
        private readonly IXBaseCacheManager _cacheManager;

        public AppActionModelHelper(
            IAppActionService appActionService,
            IWorkContext workContext,
            IXBaseCacheManager cacheManager)
        {
            _appActionService = appActionService;
            _workContext = workContext;
            _cacheManager = cacheManager;
        }

        public virtual List<AppActionModel> GetAppActions(string appId, bool showHidden = false)
        {
            return GetAppActions(appId, _workContext.WorkingLanguage.Id, showHidden);
        }

        public virtual List<AppActionTreeModel> GetAppActionTree(string appId, int? expandLevel, bool showHidden = false)
        {
            return GetAppActionTree(appId, _workContext.WorkingLanguage.Id, expandLevel, showHidden);
        }

        #region Helpers

        private List<AppActionModel> GetAppActions(string appId, string languageId, bool showHidden = false)
        {
            var cacheKey = ModelCacheKeys.AppActionsModelCacheKey.FormatWith(appId, languageId, showHidden);
            return _cacheManager.HybridProvider.Get(cacheKey, () =>
            {
                var models = _appActionService
                    .GetAll(showHidden)
                    .Where(w => w.AppId == appId)
                    .AsEnumerable()
                    .Select(s => new AppActionModel
                    {
                        Id = s.Id,
                        AppId = s.AppId,
                        ParentId = s.ParentId,
                        Name = s.GetLocalized(x => x.Name),
                        Description = s.GetLocalized(x => x.Description),
                        Controller = s.Controller,
                        Action = s.Action,
                        Icon = s.Icon,
                        ShowOnMenu = s.ShowOnMenu,
                        Active = s.Active,
                        DisplayOrder = s.DisplayOrder
                    }).ToList();

                return models;
            }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;
        }

        private List<AppActionTreeModel> GetAppActionTree(string appId, string languageId, int? expandLevel, bool showHidden = false)
        {
            var cacheKey = ModelCacheKeys.AppActionTreeModelCacheKey.FormatWith(appId, languageId, expandLevel, showHidden);
            return _cacheManager.HybridProvider.Get(cacheKey, () =>
            {
                var appActionModels = GetAppActions(appId, showHidden);
                var parents = appActionModels
                    .Where(w => string.IsNullOrWhiteSpace(w.ParentId))
                    .OrderBy(o => o.DisplayOrder);

                var models = new List<AppActionTreeModel>();
                var level = 1;
                foreach (var parent in parents)
                {
                    var tmpLevel = level;
                    var childs = GetChildAppActionTree(ref appActionModels, parent.Id, expandLevel, ++tmpLevel);
                    models.Add(new AppActionTreeModel
                    {
                        children = childs,
                        expanded = !expandLevel.HasValue || level <= expandLevel.Value,
                        folder = childs.Any(),
                        key = parent.Id.ToString(),
                        title = parent.Name,
                        tooltip = parent.Description ?? "",
                        AppId = parent.AppId,
                        ParentId = parent.ParentId,
                        Name = parent.Name,
                        Controller = parent.Controller ?? "",
                        Action = parent.Action ?? "",
                        Icon = parent.Icon ?? "",
                        ShowOnMenu = parent.ShowOnMenu,
                        Active = parent.Active,
                        DisplayOrder = parent.DisplayOrder
                    });
                }

                return models;
            }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;
        }

        private List<AppActionTreeModel> GetChildAppActionTree(ref List<AppActionModel> appActionModels, string parentId, int? expandLevel, int level)
        {
            var childs = appActionModels
                .Where(w => w.ParentId == parentId)
                .OrderBy(o => o.DisplayOrder);

            var result = new List<AppActionTreeModel>();
            if (childs.Any())
            {
                foreach (var child in childs)
                {
                    var tempLevel = level;
                    var childs2 = GetChildAppActionTree(ref appActionModels, child.Id, expandLevel, ++tempLevel);
                    result.Add(new AppActionTreeModel
                    {
                        children = childs2,
                        expanded = !expandLevel.HasValue || level <= expandLevel.Value,
                        folder = childs2.Any(),
                        key = child.Id.ToString(),
                        title = child.Name,
                        tooltip = child.Description ?? "",
                        AppId = child.AppId,
                        ParentId = child.ParentId,
                        Name = child.Name,
                        Controller = child.Controller ?? "",
                        Action = child.Action ?? "",
                        Icon = child.Icon ?? "",
                        ShowOnMenu = child.ShowOnMenu,
                        Active = child.Active,
                        DisplayOrder = child.DisplayOrder
                    });
                }
            }

            return result;
        }

        #endregion
    }
}

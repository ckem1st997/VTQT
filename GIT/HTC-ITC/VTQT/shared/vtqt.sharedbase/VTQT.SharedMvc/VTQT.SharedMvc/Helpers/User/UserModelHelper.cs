using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Caching;
using VTQT.Services.Security;
using VTQT.SharedMvc.Master;
using VTQT.SharedMvc.Master.Models;

namespace VTQT.SharedMvc.Helpers
{
    public class UserModelHelper : IUserModelHelper
    {
        private readonly IKeycloakService _keycloakService;
        private readonly IXBaseCacheManager _cacheManager;

        public UserModelHelper(
            IKeycloakService keycloakService,
            IXBaseCacheManager cacheManager)
        {
            _keycloakService = keycloakService;
            _cacheManager = cacheManager;
        }

        public virtual List<UserModel> GetAll(bool showHidden = false)
        {
            var cacheKey = ModelCacheKeys.UsersModelCacheKey.FormatWith(showHidden);
            return _cacheManager.HybridProvider.Get(cacheKey, () =>
            {
                var models = _keycloakService
                    .GetAllUsers(showHidden)
                    .Select(s => s.ToModel())
                    .ToList();

                return models;
            }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;
        }

        public virtual List<SelectListItem> GetMvcListItems(bool showHidden = false)
        {
            var cacheKey = ModelCacheKeys.UsersMvcListItemsModelCacheKey.FormatWith(showHidden);
            return _cacheManager.HybridProvider.Get(cacheKey, () =>
            {
                var models = _keycloakService
                    .GetAllUsers(showHidden)
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id,
                        Text = $"{s.FullName} - {s.Email} ({s.UserName})"
                    })
                    .ToList();

                return models;
            }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;

        }
    }
}

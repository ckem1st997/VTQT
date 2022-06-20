using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Core.Domain;
using VTQT.Core.Domain.Localization;
using VTQT.Core.Domain.Security;
using VTQT.Core.Infrastructure;
using VTQT.Services.Helpers;
using VTQT.Services.Localization;
using VTQT.Services.Security;
using VTQT.Web.Framework.Helpers;
using VTQT.Web.Framework.Security;

namespace VTQT.Web.Framework
{
    // TODO-XBase-WorkContext: workContext by Api/Mvc
    /// <summary>
    /// Represents work context for web application or web api
    /// </summary>
    public partial class WebWorkContext : IWorkContext
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly IKeycloakService _keycloakService;
        private readonly IPermissionService _permissionService;
        private readonly IWebHelper _webHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IXBaseCacheManager _cacheManager;

        #endregion

        #region Ctor

        public WebWorkContext(
            ILanguageService languageService,
            IKeycloakService keycloakService,
            IPermissionService permissionService,
            IWebHelper webHelper,
            IHttpContextAccessor httpContextAccessor,
            IXBaseCacheManager cacheManager)
        {
            _languageService = languageService;
            _keycloakService = keycloakService;
            _permissionService = permissionService;
            _webHelper = webHelper;
            _httpContextAccessor = httpContextAccessor;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Properties

        public bool IsAuthenticated => _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;

        private string _userId;
        public string UserId
        {
            get
            {
                if (!_userId.IsEmpty())
                    return _userId;

                var claim = _httpContextAccessor.HttpContext.User.FindFirst(JwtClaimTypes.Subject);
                if (claim == null)
                    throw new InvalidOperationException("sub claim is missing");

                _userId = claim.Value;

                return _userId;
            }
        }

        private string _userName;
        public string UserName
        {
            get
            {
                if (!_userName.IsEmpty())
                    return _userName;

                var claim = _httpContextAccessor.HttpContext.User.FindFirst(JwtClaimTypes.PreferredUserName);
                if (claim == null)
                    throw new InvalidOperationException("preferred_username claim is missing");

                _userName = claim.Value;

                return _userName;
            }
        }

        private string _languageId;
        public string LanguageId
        {
            get
            {
                if (!_languageId.IsEmpty())
                    return _languageId;

                var appProjectType = AppHelper.GetAppProjectType();
                if (appProjectType == AppProjectType.API)
                {
                    _languageId = _webHelper.GetHeaderValue(XBaseHeaderNames.LanguageId);
                    if (_languageId.IsEmpty())
                    {
                        //var language = _languageService.GetDefaultLanguage();
                        //_languageId = language?.Id;
                    }
                }
                else if (appProjectType == AppProjectType.MVC)
                {
                    return "1";
                }

                return _languageId;
            }
        }

        private LanguageSvcEntity _workingLanguage;
        public LanguageSvcEntity WorkingLanguage
        {
            get
            {
                //whether there is a cached value
                if (_workingLanguage != null)
                    return _workingLanguage;

                // TODO-XBase-Language
                _workingLanguage = _languageService.GetLanguageById("1").ToSvcEntity();

                return _workingLanguage;
            }
            set
            {
                //save passed language identifier
                //var customer = await GetCurrentCustomerAsync();
                //var store = await _storeContext.GetCurrentStoreAsync();
                //await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.LanguageIdAttribute, language?.Id ?? 0, store.Id);

                //then reset the cached value
                _workingLanguage = null;
            }
        }

        public User User
        {
            get
            {
                if (!IsAuthenticated)
                    return null;

                // Get User Info from Keycloak API
                var cacheKey = FrameworkCacheKeys.UserCacheKey.FormatWith(UserId);
                var result = _cacheManager.HybridProvider.Get(cacheKey, () => EngineContext.Current.Resolve<IKeycloakService>().GetUserById(UserId),
                    TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;

                return result;
            }
            set
            {
                var cacheKey = FrameworkCacheKeys.UserCacheKey.FormatWith(UserId);
                _cacheManager.HybridProvider.Set(cacheKey, value, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime));
            }
        }

        public List<AppActionSvcEntity> UserPermissions
        {
            get
            {
                var cacheKey = FrameworkCacheKeys.UserPermissionsCacheKey.FormatWith(UserId);
                var result = _cacheManager.HybridProvider.Get(cacheKey, GetAllUserPermissions,
                    TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;

                return result;
            }
            set
            {
                var cacheKey = FrameworkCacheKeys.UserPermissionsCacheKey.FormatWith(UserId);
                _cacheManager.HybridProvider.Set(cacheKey, value, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime));
            }
        }

        public UserToken UserToken
        {
            get
            {
                var cacheKey = FrameworkCacheKeys.UserTokenCacheKey.FormatWith(UserId);
                var result = _cacheManager.HybridProvider.Get(cacheKey, GetUserToken,
                    TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;

                return result;
            }
            set
            {
                var cacheKey = FrameworkCacheKeys.UserTokenCacheKey.FormatWith(UserId);
                _cacheManager.HybridProvider.Set(cacheKey, value, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime));
            }
        }

        public string AccessToken
        {
            get
            {
                return UserToken?.AccessToken;
            }
        }

        public string RefreshToken
        {
            get
            {
                return UserToken?.RefreshToken;
            }
        }

        private bool? _isEnglishCulture;
        public bool IsEnglishCulture
        {
            get
            {
                if (_isEnglishCulture.HasValue)
                    return _isEnglishCulture.Value;

                _isEnglishCulture = CheckEnglishCulture(WorkingLanguage.LanguageCulture);

                return _isEnglishCulture.Value;
            }
            set => _isEnglishCulture = value;
        }

        #endregion

        #region Helpers

        public bool CheckEnglishCulture(string cultureName)
        {
            return CultureHelper.EnglishCultures.Contains(cultureName);
        }

        public void InitUserCache()
        {
            User = _keycloakService.GetUserById(UserId);
        }

        public void InitUserPermissionsCache()
        {
            UserPermissions = GetAllUserPermissions();
        }

        public void InitUserTokenCache()
        {
            UserToken = GetUserToken();
        }

        public IEnumerable<AppControllerItem> GetAppApiControllers(string appApiAssemblyFilePath)
        {
            var assembly = Assembly.LoadFile(appApiAssemblyFilePath);
            var controllers =
                from type in assembly.GetTypes()
                let appController = type.GetCustomAttribute<AppApiControllerAttribute>(false)
                where typeof(ControllerBase).IsAssignableFrom(type) && appController != null
                select new
                {
                    Type = type,
                    AppController = appController
                };
            var workingLanguageId = WorkingLanguage.Id;
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            return controllers.Select(s =>
            {
                var item = new AppControllerItem
                {
                    Name = s.Type.Name,
                    Order = s.AppController.Order
                };

                string resourceValue = null;
                var langId = workingLanguageId;
                resourceValue = localizationService.GetResource(s.AppController.ResourceKey, langId, false, "" /* ResourceKey */, true);
                item.Description = resourceValue;

                return item;
            }).OrderBy(o => o.Order);
        }

        public IEnumerable<AppActionItem> GetAppApiActions(string appApiAssemblyFilePath, string appApiController)
        {
            var assembly = Assembly.LoadFile(appApiAssemblyFilePath);
            var controllerType =
                (from type in assembly.GetTypes()
                 where typeof(ControllerBase).IsAssignableFrom(type) && type.Name.Equals(appApiController, StringComparison.OrdinalIgnoreCase)
                 select type).Single();
            var actions =
                from action in controllerType.GetMethods()
                let appAction = action.GetCustomAttribute<AppApiActionAttribute>(false)
                where appAction != null && action.IsPublic
                select new
                {
                    action.Name,
                    AppAction = appAction
                };
            var workingLanguageId = WorkingLanguage.Id;
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            return actions.Distinct().Select(s =>
            {
                var item = new AppActionItem
                {
                    Name = s.Name,
                    Order = s.AppAction.Order
                };

                string resourceValue = null;
                var langId = workingLanguageId;
                resourceValue = localizationService.GetResource(s.AppAction.ResourceKey, langId, false, "" /* ResourceKey */, true);
                item.Description = resourceValue;

                return item;
            }).OrderBy(o => o.Order);
        }

        public string GetAppApiAssemblyFilePath(string appType)
        {
            return CommonHelper.MapPath($"{CommonHelper.XBaseConfig.AppAssembliesPath}/{appType}.dll");
        }

        #endregion

        #region Utilities

        private List<AppActionSvcEntity> GetAllUserPermissions()
        {
            var result = _permissionService.GetAllUserPermissions(UserId)
                .Select(s => new AppActionSvcEntity
                {
                    Id = s.Id,
                    AppId = s.AppId,
                    ParentId = s.ParentId,
                    Name = s.Name,
                    Description = s.Description,
                    Controller = s.Controller,
                    Action = s.Action,
                    Icon = s.Icon,
                    ShowOnMenu = s.ShowOnMenu,
                    Active = s.Active,
                    DisplayOrder = s.DisplayOrder
                }).ToList();

            return result;
        }

        private UserToken GetUserToken()
        {
            //var issuedAtStr = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == JwtClaimTypes.IssuedAt).Value;
            var issuedAtStr = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == nameof(JwtClaimTypes.IssuedAt)).Value;
            var issuedAt = long.Parse(issuedAtStr);
            var issuedAtUtc = DateTime.UnixEpoch.AddSeconds(issuedAt);

            var accessToken = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == OidcConstants.TokenTypes.AccessToken).Value;
            var expiresInStr = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == OidcConstants.AuthorizeResponse.ExpiresIn).Value;
            var expiresIn = long.Parse(expiresInStr);
            var accessTokenExpiredUtc = issuedAtUtc.AddSeconds(expiresIn);

            var refreshToken = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == OidcConstants.TokenTypes.RefreshToken).Value;

            return new UserToken
            {
                IssuedAt = issuedAt,
                IssuedAtUtc = issuedAtUtc,
                AccessToken = accessToken,
                ExpiresIn = expiresIn,
                AccessTokenExpiredUtc = accessTokenExpiredUtc,
                RefreshToken = refreshToken,
                RefreshExpiresIn = 0,
                RefreshTokenExpiredUtc = null
            };
        }

        #endregion
    }
}

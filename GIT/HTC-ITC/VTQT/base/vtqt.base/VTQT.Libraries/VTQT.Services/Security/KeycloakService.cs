using System;
using System.Collections.Generic;
using System.Linq;
using RestSharp;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Core.Domain.Security;
using VTQT.Utilities;

namespace VTQT.Services.Security
{
    public partial class KeycloakService : IKeycloakService
    {
        #region Constants

        public const string MasterRealm = "master";

        public const string TokenRoute = "/realms/{0}/protocol/openid-connect/token";

        public const string UsersRoute = "/admin/realms/{0}/users";
        public const string UsersCountRoute = "/admin/realms/{0}/users/count";
        public const string UserRoute = "/admin/realms/{0}/users/{1}";

        #endregion

        #region Fields

        private readonly IXBaseCacheManager _cacheManager;

        #endregion

        #region Ctor

        public KeycloakService(
            IXBaseCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        public virtual IPagedList<User> GetUsers(UserSearchContext ctx, string realm = null)
        {
            var accessToken = RequestClientCredentialsToken(MasterRealm, CommonHelper.SsoConfig.ServiceAccount.ClientId, CommonHelper.SsoConfig.ServiceAccount.ClientSecret).AccessToken;

            realm = GetRealm(realm);
            var resUsers = ApiHelper.ExecuteGeneric<IEnumerable<KeycloakUser>>(UsersRoute.FormatWith(realm), new
            {
                search = ctx.Keywords,
                first = ctx.PageIndex,
                max = ctx.PageSize,
            }, Method.GET, ApiHosts.Sso, new[]
            {
                new KeyValuePair<string, string>("Authorization", $"Bearer {accessToken}")
            }, DataFormat.None, false);
            if (resUsers == null)
                throw new XBaseException("Users response is null.");

            var resTotalCount = ApiHelper.ExecuteGeneric<dynamic>(UsersCountRoute.FormatWith(realm), new
            {
                search = ctx.Keywords
            }, Method.GET, ApiHosts.Sso, new[]
            {
                new KeyValuePair<string, string>("Authorization", $"Bearer {accessToken}")
            }, DataFormat.None, false);
            if (resTotalCount == null)
                throw new XBaseException("Users response is null.");

            var result = Enumerable.Empty<User>();
            var totalCount = (int)resTotalCount;
            if (resUsers.Any())
            {
                result = resUsers.Select(s => new User
                {
                    Id = s.id,
                    UserName = s.username,
                    FirstName = s.firstName,
                    LastName = s.lastName,
                    FullName = CommonHelper.GetInvariantFullName(s.firstName, s.lastName),
                    Email = s.email,
                    EmailConfirmed = s.emailVerified,
                    Active = s.enabled
                });
            }

            return new PagedList<User>(result, ctx.PageIndex, ctx.PageSize, totalCount);
        }

        public virtual User GetUserById(string id, string realm = null)
        {
            var accessToken = RequestClientCredentialsToken(MasterRealm, CommonHelper.SsoConfig.ServiceAccount.ClientId, CommonHelper.SsoConfig.ServiceAccount.ClientSecret).AccessToken;

            realm = GetRealm(realm);
            var resUser = ApiHelper.ExecuteGeneric<KeycloakUser>(UserRoute.FormatWith(realm, id), new
            {
                id = id
            }, Method.GET, ApiHosts.Sso, new[]
            {
                new KeyValuePair<string, string>("Authorization", $"Bearer {accessToken}")
            }, DataFormat.None, false);
            if (resUser == null)
                throw new XBaseException("User response is null.");

            var result = new User
            {
                Id = resUser.id,
                UserName = resUser.username,
                FirstName = resUser.firstName,
                LastName = resUser.lastName,
                FullName = CommonHelper.GetInvariantFullName(resUser.firstName, resUser.lastName),
                Email = resUser.email,
                EmailConfirmed = resUser.emailVerified,
                Active = resUser.enabled
            };
            return result;
        }

        public virtual IList<User> GetAllUsers(bool showHidden = false, string realm = null)
        {
            var key = MasterCacheKeys.Users.AllCacheKey.FormatWith(showHidden);
            var entities = _cacheManager.HybridProvider.Get(key, () =>
            {
                var accessToken = RequestClientCredentialsToken(MasterRealm, CommonHelper.SsoConfig.ServiceAccount.ClientId, CommonHelper.SsoConfig.ServiceAccount.ClientSecret).AccessToken;

                realm = GetRealm(realm);
                object data;
                if (!showHidden)
                    data = new
                    {
                        enabled = true,
                        max = int.MaxValue
                    };
                else
                    data = new
                    {
                        max = int.MaxValue
                    };
                var resUsers = ApiHelper.ExecuteGeneric<IEnumerable<KeycloakUser>>(UsersRoute.FormatWith(realm), data, Method.GET, ApiHosts.Sso, new[]
                {
                    new KeyValuePair<string, string>("Authorization", $"Bearer {accessToken}")
                }, DataFormat.None, false);
                if (resUsers == null)
                    throw new XBaseException("Users response is null.");

                var result = new List<User>();
                if (resUsers.Any())
                {
                    result = resUsers.Select(s => new User
                    {
                        Id = s.id,
                        UserName = s.username,
                        FirstName = s.firstName,
                        LastName = s.lastName,
                        FullName = CommonHelper.GetInvariantFullName(s.firstName, s.lastName),
                        Email = s.email,
                        EmailConfirmed = s.emailVerified,
                        Active = s.enabled
                    }).ToList();
                }

                return result;
            }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;

            return entities;
        }

        #endregion

        #region Helpers

        public virtual TokenResponse RequestClientCredentialsToken(string realm, string clientId, string clientSecret, string scope = null)
        {
            dynamic data = new
            {
                client_id = clientId,
                client_secret = clientSecret,
                grant_type = "client_credentials"
            };
            if (!string.IsNullOrWhiteSpace(scope))
                data.scope = scope;

            var res = ApiHelper.ExecuteDynamic(TokenRoute.FormatWith(realm), data, Method.POST, ApiHosts.Sso, null, DataFormat.None, false);
            if (res == null)
                throw new XBaseException("Token Response is null.");

            return new TokenResponse
            {
                AccessToken = res.access_token,
                ExpiresIn = res.expires_in,
                RefreshExpiresIn = res.refresh_expires_in,
                RefreshToken = res.refresh_token,
                TokenType = res.token_type,
                IdToken = res.id_token,
                Scope = res.scope
            };
        }

        public virtual TokenResponse RequestPasswordToken(string realm, string userName, string password, string scope = null)
        {
            return null;
        }

        public virtual TokenResponse RequestRefreshToken(string realm, string clientId, string clientSecret, string refreshToken)
        {
            dynamic data = new
            {
                client_id = clientId,
                client_secret = clientSecret,
                grant_type = "refresh_token",
                refresh_token = refreshToken
            };

            var res = ApiHelper.ExecuteDynamic(TokenRoute.FormatWith(realm), data, Method.POST, ApiHosts.Sso, null, DataFormat.None, false);
            if (res == null)
                throw new XBaseException("Token Response is null.");

            return new TokenResponse
            {
                AccessToken = res.access_token,
                ExpiresIn = res.expires_in,
                RefreshExpiresIn = res.refresh_expires_in,
                RefreshToken = res.refresh_token,
                TokenType = res.token_type,
                IdToken = res.id_token,
                Scope = res.scope
            };
        }

        private string GetRealm(string realm = null)
        {
            if (string.IsNullOrWhiteSpace(realm))
                realm = CommonHelper.SsoConfig.Realm;

            return realm;
        }

        #endregion
    }
}

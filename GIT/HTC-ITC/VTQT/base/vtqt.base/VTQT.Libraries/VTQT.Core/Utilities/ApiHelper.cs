using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IdentityModel;
using Newtonsoft.Json;
using RestSharp;
using VTQT.Core;
using VTQT.Core.Domain;
using VTQT.Core.Domain.Security;
using VTQT.Core.Infrastructure;

namespace VTQT.Utilities
{
    public static class ApiHelper
    {
        public static string GatewayApiHost = CommonHelper.GetAppSetting<string>($"Apis:{nameof(ApiHosts.Gateway)}:BaseUrl");
        public static string SsoApiHost = CommonHelper.GetAppSetting<string>($"Apis:{nameof(ApiHosts.Sso)}:BaseUrl");
        public static string EventApiHost = CommonHelper.GetAppSetting<string>($"Apis:{nameof(ApiHosts.Event)}:BaseUrl");

        public static string WarehouseApiHost = CommonHelper.GetAppSetting<string>($"Apis:{nameof(ApiHosts.Warehouse)}:BaseUrl");
        public static string AssetApiHost = CommonHelper.GetAppSetting<string>($"Apis:{nameof(ApiHosts.Asset)}:BaseUrl");
        public static string MasterApiHost = CommonHelper.GetAppSetting<string>($"Apis:{nameof(ApiHosts.Master)}:BaseUrl");
        public static string TicketApiHost = CommonHelper.GetAppSetting<string>($"Apis:{nameof(ApiHosts.Ticket)}:BaseUrl");
        public static string DashboardApiHost = CommonHelper.GetAppSetting<string>($"Apis:{nameof(ApiHosts.Dashboard)}:BaseUrl");

        public const int TokenExpiresThreshold = 30; // Seconds

        public static XBaseResult Execute(
            string api, object data = null, Method method = Method.POST,
            ApiHosts apiHost = ApiHosts.Gateway, ICollection<KeyValuePair<string, string>> headers = null, DataFormat dataFormat = DataFormat.Json,
            bool useAuthentication = true, bool autoRefreshToken = true)
        {
            try
            {
                var host = GetApiHost(apiHost);
                var client = new RestClient
                {
                    BaseUrl = new Uri(host)
                };
                var request = new RestRequest(api, method) { RequestFormat = DataFormat.Json };
                request.AddHeaders(headers, useAuthentication, autoRefreshToken);
                request.AddData(data, dataFormat);

                var response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<XBaseResult>(response.Content, JsonHelper.DefaultSerializerSettings);
                }

                return new XBaseResult
                {
                    success = false,
                    httpStatusCode = response.GetHttpStatusCode(),
                    message = response.ToErrorString()
                };
            }
            catch (Exception ex)
            {
                return new XBaseResult
                {
                    success = false,
                    httpStatusCode = (int)HttpStatusCode.InternalServerError,
                    message = ex.ToString()
                };
            }
        }

        public static async Task<XBaseResult> ExecuteAsync(
            string api, object data = null, Method method = Method.POST,
            ApiHosts apiHost = ApiHosts.Gateway, ICollection<KeyValuePair<string, string>> headers = null, DataFormat dataFormat = DataFormat.Json,
            bool useAuthentication = true, bool autoRefreshToken = true)
        {
            try
            {
                var host = GetApiHost(apiHost);
                var client = new RestClient
                {
                    BaseUrl = new Uri(host)
                };
                var request = new RestRequest(api, method) { RequestFormat = DataFormat.Json };
                request.AddHeaders(headers, useAuthentication, autoRefreshToken);
                request.AddData(data, dataFormat);

                var response = await client.ExecuteAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<XBaseResult>(response.Content, JsonHelper.DefaultSerializerSettings);
                }

                return new XBaseResult
                {
                    success = false,
                    httpStatusCode = response.GetHttpStatusCode(),
                    message = response.ToErrorString()
                };
            }
            catch (Exception ex)
            {
                return new XBaseResult
                {
                    success = false,
                    httpStatusCode = (int)HttpStatusCode.InternalServerError,
                    message = ex.ToString()
                };
            }
        }

        public static XBaseResult<T> Execute<T>(
            string api, object data = null, Method method = Method.POST,
            ApiHosts apiHost = ApiHosts.Gateway, ICollection<KeyValuePair<string, string>> headers = null, DataFormat dataFormat = DataFormat.Json,
            bool useAuthentication = true, bool autoRefreshToken = true)
            where T : class
        {
            try
            {
                var host = GetApiHost(apiHost);
                var client = new RestClient
                {
                    BaseUrl = new Uri(host)
                };
                var request = new RestRequest(api, method) { RequestFormat = DataFormat.Json };
                request.AddHeaders(headers, useAuthentication, autoRefreshToken);
                request.AddData(data, dataFormat);

                var response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var objResult = JsonConvert.DeserializeObject<XBaseResult<T>>(response.Content, JsonHelper.DefaultSerializerSettings);
                    return objResult;
                }

                return new XBaseResult<T>
                {
                    success = false,
                    httpStatusCode = response.GetHttpStatusCode(),
                    message = response.ToErrorString()
                };
            }
            catch (Exception ex)
            {
                return new XBaseResult<T>
                {
                    success = false,
                    httpStatusCode = (int)HttpStatusCode.InternalServerError,
                    message = ex.ToString()
                };
            }
        }

        public static async Task<XBaseResult<T>> ExecuteAsync<T>(
            string api, object data = null, Method method = Method.POST,
            ApiHosts apiHost = ApiHosts.Gateway, ICollection<KeyValuePair<string, string>> headers = null, DataFormat dataFormat = DataFormat.Json,
            bool useAuthentication = true, bool autoRefreshToken = true)
            where T : class
        {
            try
            {
                var host = GetApiHost(apiHost);
                var client = new RestClient
                {
                    BaseUrl = new Uri(host)
                };
                var request = new RestRequest(api, method) { RequestFormat = DataFormat.Json };
                request.AddHeaders(headers, useAuthentication, autoRefreshToken);
                request.AddData(data, dataFormat);

                var response = await client.ExecuteAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var objResult = JsonConvert.DeserializeObject<XBaseResult<T>>(response.Content, JsonHelper.DefaultSerializerSettings);
                    return objResult;
                }

                return new XBaseResult<T>
                {
                    success = false,
                    httpStatusCode = response.GetHttpStatusCode(),
                    message = response.ToErrorString()
                };
            }
            catch (Exception ex)
            {
                return new XBaseResult<T>
                {
                    success = false,
                    httpStatusCode = (int)HttpStatusCode.InternalServerError,
                    message = ex.ToString()
                };
            }
        }

        public static dynamic ExecuteDynamic(
            string api, object data = null, Method method = Method.POST,
            ApiHosts apiHost = ApiHosts.Gateway, ICollection<KeyValuePair<string, string>> headers = null, DataFormat dataFormat = DataFormat.Json,
            bool useAuthentication = true, bool autoRefreshToken = true)
        {
            try
            {
                var host = GetApiHost(apiHost);
                var client = new RestClient
                {
                    BaseUrl = new Uri(host)
                };
                var request = new RestRequest(api, method) { RequestFormat = DataFormat.Json };
                request.AddHeaders(headers, useAuthentication, autoRefreshToken);
                request.AddData(data, dataFormat);

                var response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    dynamic objResult = JsonConvert.DeserializeObject(response.Content, JsonHelper.DefaultSerializerSettings);
                    return objResult;
                }

                // TODO-XBase-Log
                return null;
            }
            catch (Exception ex)
            {
                // TODO-XBase-Log
                return null;
            }
        }

        public static async Task<dynamic> ExecuteDynamicAsync(
            string api, object data = null, Method method = Method.POST,
            ApiHosts apiHost = ApiHosts.Gateway, ICollection<KeyValuePair<string, string>> headers = null, DataFormat dataFormat = DataFormat.Json,
            bool useAuthentication = true, bool autoRefreshToken = true)
        {
            try
            {
                var host = GetApiHost(apiHost);
                var client = new RestClient
                {
                    BaseUrl = new Uri(host)
                };
                var request = new RestRequest(api, method) { RequestFormat = DataFormat.Json };
                request.AddHeaders(headers, useAuthentication, autoRefreshToken);
                request.AddData(data, dataFormat);

                var response = await client.ExecuteAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    dynamic objResult = JsonConvert.DeserializeObject(response.Content, JsonHelper.DefaultSerializerSettings);
                    return objResult;
                }

                // TODO-XBase-Log
                return null;
            }
            catch (Exception ex)
            {
                // TODO-XBase-Log
                return null;
            }
        }

        public static T ExecuteGeneric<T>(
            string api, object data = null, Method method = Method.POST,
            ApiHosts apiHost = ApiHosts.Gateway, ICollection<KeyValuePair<string, string>> headers = null, DataFormat dataFormat = DataFormat.Json,
            bool useAuthentication = true, bool autoRefreshToken = true)
            where T : class
        {
            try
            {
                var host = GetApiHost(apiHost);
                var client = new RestClient
                {
                    BaseUrl = new Uri(host)
                };
                var request = new RestRequest(api, method) { RequestFormat = DataFormat.Json };
                request.AddHeaders(headers, useAuthentication, autoRefreshToken);
                request.AddData(data, dataFormat);

                var response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var objResult = JsonConvert.DeserializeObject<T>(response.Content, JsonHelper.DefaultSerializerSettings);
                    return objResult;
                }

                // TODO-XBase-Log
                return null;
            }
            catch (Exception ex)
            {
                // TODO-XBase-Log
                return null;
            }
        }

        public static async Task<T> ExecuteGenericAsync<T>(
            string api, object data = null, Method method = Method.POST,
            ApiHosts apiHost = ApiHosts.Gateway, ICollection<KeyValuePair<string, string>> headers = null, DataFormat dataFormat = DataFormat.Json,
            bool useAuthentication = true, bool autoRefreshToken = true)
            where T : class
        {
            try
            {
                var host = GetApiHost(apiHost);
                var client = new RestClient
                {
                    BaseUrl = new Uri(host)
                };
                var request = new RestRequest(api, method) { RequestFormat = DataFormat.Json };
                request.AddHeaders(headers, useAuthentication, autoRefreshToken);
                request.AddData(data, dataFormat);

                var response = await client.ExecuteAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var objResult = JsonConvert.DeserializeObject<T>(response.Content, JsonHelper.DefaultSerializerSettings);
                    return objResult;
                }

                // TODO-XBase-Log
                return null;
            }
            catch (Exception ex)
            {
                // TODO-XBase-Log
                return null;
            }
        }

        #region Helpers

        public static string GetApiHost(ApiHosts apiHost)
        {
            var host = string.Empty;

            if (apiHost == ApiHosts.Gateway)
                host = GatewayApiHost;
            else if (apiHost == ApiHosts.Sso)
                host = SsoApiHost;
            else if (apiHost == ApiHosts.Event)
                host = EventApiHost;

            else if (apiHost == ApiHosts.Warehouse)
                host = WarehouseApiHost;
            else if (apiHost == ApiHosts.Asset)
                host = AssetApiHost;
            else if (apiHost == ApiHosts.Master)
                host = MasterApiHost;
            else if (apiHost == ApiHosts.Ticket)
                host = TicketApiHost;
            else if (apiHost == ApiHosts.Dashboard)
                host = DashboardApiHost;

            return host;
        }

        public static RestRequest AddHeaders(this RestRequest request,
            ICollection<KeyValuePair<string, string>> headers, bool useAuthentication, bool autoRefreshToken = true)
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var reqHeaders = headers != null && headers.Any()
                ? new List<KeyValuePair<string, string>>(headers)
                : new List<KeyValuePair<string, string>>();

            #region Authorization
            if (useAuthentication)
            {
                if (!reqHeaders.Any(a => a.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase)))
                {
                    var userToken = workContext.UserToken;
                    var accessToken = userToken.AccessToken;
                    if (autoRefreshToken
                        && userToken.AccessTokenExpiredUtc <= DateTime.UtcNow.AddSeconds(TokenExpiresThreshold))
                    {
                        var ssoConfig = CommonHelper.SsoConfig;
                        var tokenRes = RequestRefreshToken(ssoConfig.Realm, ssoConfig.ClientId, ssoConfig.ClientSecret, userToken.RefreshToken);

                        HandleRefreshTokenResponse(tokenRes, workContext);

                        accessToken = tokenRes.AccessToken;
                    }

                    reqHeaders.Add(new KeyValuePair<string, string>("Authorization", $"Bearer {accessToken}"));
                }
            }
            #endregion

            reqHeaders.Add(new KeyValuePair<string, string>(XBaseHeaderNames.LanguageId, workContext.LanguageId));

            request.AddHeaders(reqHeaders);

            return request;
        }

        public static RestRequest AddData(this RestRequest request, object data, DataFormat dataFormat)
        {
            if (data == null)
                return request;

            if (request.Method == Method.GET)
                request.AddObject(data);
            else
            {
                if (dataFormat == DataFormat.Json)
                {
                    request.AddJsonBody(data);
                }
                else if (dataFormat == DataFormat.Xml)
                {
                    request.AddXmlBody(data);
                }
                else
                {
                    request.AddObject(data);
                }
            }

            return request;
        }

        public static int GetHttpStatusCode(this IRestResponse response)
        {
            if (response.IsSuccessful)
                return (int)response.StatusCode;

            if (response.ResponseStatus == ResponseStatus.None
                || response.ResponseStatus == ResponseStatus.Error
                || response.ResponseStatus == ResponseStatus.Aborted)
                return (int)HttpStatusCode.InternalServerError;

            if (response.ResponseStatus == ResponseStatus.TimedOut)
                return (int)HttpStatusCode.RequestTimeout;

            return (int)response.StatusCode;
        }

        private static TokenResponse RequestRefreshToken(string realm, string clientId, string clientSecret, string refreshToken)
        {
            dynamic data = new
            {
                client_id = clientId,
                client_secret = clientSecret,
                grant_type = "refresh_token",
                refresh_token = refreshToken
            };

            var res = ExecuteDynamic("/realms/{0}/protocol/openid-connect/token".FormatWith(realm), data, Method.POST, ApiHosts.Sso, null, DataFormat.None, false);
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

        private static void HandleRefreshTokenResponse(TokenResponse tokenRes, IWorkContext workContext)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenRes.AccessToken);

            var issuedAtStr = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.IssuedAt).Value;
            var issuedAt = long.Parse(issuedAtStr);
            var issuedAtUtc = DateTime.UnixEpoch.AddSeconds(issuedAt);

            var accessTokenExpiredUtc = issuedAtUtc.AddSeconds(tokenRes.ExpiresIn);

            var newUserToken = new UserToken
            {
                IssuedAt = issuedAt,
                IssuedAtUtc = issuedAtUtc,
                AccessToken = tokenRes.AccessToken,
                ExpiresIn = tokenRes.ExpiresIn,
                AccessTokenExpiredUtc = accessTokenExpiredUtc,
                RefreshToken = tokenRes.RefreshToken,
                RefreshExpiresIn = 0,
                RefreshTokenExpiredUtc = null
            };
            workContext.UserToken = newUserToken;
        }

        #endregion
    }

    public static class RestSharpExtensions
    {
        public static string ToErrorString(this IRestResponse response)
        {
            var msg = response.ErrorException?.ToString();
            if (string.IsNullOrWhiteSpace(msg))
                msg = response.Content;

            return msg;
        }
    }

    public enum ApiHosts
    {
        Hrm = 100,
        Ticket = 90,
        Work = 80,
        Social = 70,
        Asset = 60,
        Warehouse = 50,

        Cdn = 40,
        Workflow = 30,
        Master = 20,
        Dashboard = 1000,
        Sso = 10,
        Event = 9999,
        Gateway = 0
    }
}

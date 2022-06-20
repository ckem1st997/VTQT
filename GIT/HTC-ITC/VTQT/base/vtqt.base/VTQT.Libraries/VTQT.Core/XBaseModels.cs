using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace VTQT.Core
{
    #region AppAction

    public class AppControllerItem
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Order { get; set; }
    }

    public class AppActionItem
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Order { get; set; }
    }

    #endregion

    #region SSO

    public class UserToken
    {
        public long IssuedAt { get; set; }

        public DateTime IssuedAtUtc { get; set; }

        public string AccessToken { get; set; }

        public long ExpiresIn { get; set; }

        public DateTime AccessTokenExpiredUtc { get; set; }

        // Offline Token
        public string RefreshToken { get; set; }

        public long RefreshExpiresIn { get; set; } = 0;

        public DateTime? RefreshTokenExpiredUtc { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string ClientId { get; set; }

        public string RefreshToken { get; set; }
    }

    #endregion

    #region Common

    public class GroupObjectItem
    {
        public string Key { get; set; }

        public IEnumerable<object> Data { get; set; }

        public GroupObjectItem()
        {
            Data = Enumerable.Empty<object>();
        }
    }

    #endregion

    #region XBaseResult

    [Serializable]
    public class XBaseResult<T> where T : class
    {
        public bool success { get; set; }

        public string code { get; set; }

        public int httpStatusCode { get; set; }

        public string title { get; set; }

        public string message { get; set; }

        public T data { get; set; }

        public int totalCount { get; set; }

        public bool isRedirect { get; set; }

        public string redirectUrl { get; set; }

        public Dictionary<string, IEnumerable<string>> errors { get; set; }

        public XBaseResult()
        {
            success = true;
            httpStatusCode = (int)HttpStatusCode.OK;
            errors = new Dictionary<string, IEnumerable<string>>();
        }

        public XBaseResult(XBaseResult<T> obj)
        {
            success = obj.success;
            code = obj.code;
            httpStatusCode = obj.httpStatusCode;
            title = obj.title;
            message = obj.message;
            data = obj.data;
            totalCount = obj.totalCount;
            isRedirect = obj.isRedirect;
            redirectUrl = obj.redirectUrl;
            errors = obj.errors;
        }
    }

    [Serializable]
    public class XBaseResult : XBaseResult<dynamic>
    {
    }

    public class XBaseResultCode
    {
        public const string Success = "100";
        public const string Info = "101";
        public const string Warning = "102";
        public const string Error = "103";
    }

    #endregion
}

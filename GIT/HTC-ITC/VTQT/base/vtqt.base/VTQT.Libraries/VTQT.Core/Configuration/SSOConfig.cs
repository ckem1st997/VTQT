using System.Collections.Generic;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace VTQT.Core.Configuration
{
    public partial class SsoConfig
    {
        public const string Sso = "Sso";

        public string Realm { get; set; }

        public string Authority { get; set; }

        public bool RequireHttpsMetadata { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string ResponseType { get; set; }

        // Custom-XBase: Set RedirectUri for SSO "redirect_uri"
        public string RedirectUri { get; set; }

        public string CallbackPath { get; set; }

        public string RemoteSignOutPath { get; set; }

        public string SignedOutRedirectUri { get; set; }

        public string SignedOutCallbackPath { get; set; }

        public string AccessDeniedPath { get; set; }

        public bool GetClaimsFromUserInfoEndpoint { get; set; }

        public bool SaveTokens { get; set; }

        public ServiceAccount ServiceAccount { get; set; }

        public IList<string> Scopes { get; set; }

        public SsoConfig()
        {
            RequireHttpsMetadata = false;
            ResponseType = OpenIdConnectResponseType.Code;
            SignedOutRedirectUri = "/";
            //AccessDeniedPath = "/MvcError/AccessDenied";
            AccessDeniedPath = "/MvcError/Forbidden";
            GetClaimsFromUserInfoEndpoint = true;
            SaveTokens = true;
            //Scopes = new List<string>();
            Scopes = new List<string>
            {
                "openid", "email", "profile", "offline_access", "roles"
            };
        }
    }

    public class ServiceAccount
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
    }
}

using System.Collections.Generic;
using VTQT.Core.Domain.Localization;
using VTQT.Core.Domain.Security;

namespace VTQT.Core
{
    /// <summary>
    /// Represents work context
    /// </summary>
    public interface IWorkContext
    {
        bool IsAuthenticated { get; }

        string UserId { get; }

        string UserName { get; }

        string LanguageId { get; }

        LanguageSvcEntity WorkingLanguage { get; set; }

        User User { get; set; }

        List<AppActionSvcEntity> UserPermissions { get; set; }

        UserToken UserToken { get; set; }

        string AccessToken { get; }

        string RefreshToken { get; }

        bool IsEnglishCulture { get; set; }

        bool CheckEnglishCulture(string cultureName);

        void InitUserCache();

        void InitUserPermissionsCache();

        void InitUserTokenCache();

        IEnumerable<AppControllerItem> GetAppApiControllers(string appApiAssemblyFilePath);

        IEnumerable<AppActionItem> GetAppApiActions(string appApiAssemblyFilePath, string appApiController);

        string GetAppApiAssemblyFilePath(string appType);
    }
}

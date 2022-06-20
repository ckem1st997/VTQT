using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Master;
using VTQT.Core.Domain.Security;

namespace VTQT.Services.Security
{
    public interface IPermissionService
    {
        Task UpdateRolePermissionsAsync(string roleId, IEnumerable<string> appActionIds, string appId);

        Task RemoveAllRolePermissionsAsync(string roleId, string appId);

        Task<IList<AppAction>> GetRolePermissionsAsync(string roleId);

        Task<IList<Role>> GetAuthorizedRolesAsync(string appActionId);

        Task UpdateUserPermissionsAsync(string userId, IEnumerable<string> appActionIds, string appId);

        Task RemoveAllUserPermissionsAsync(string userId, string appId);

        IList<AppAction> GetAllUserPermissions(string userId);

        Task<IList<AppAction>> GetUserPermissionsAsync(string userId, string appId);

        bool Authorize(AppAction appAction, IEnumerable<AppActionSvcEntity> permissions);
    }
}

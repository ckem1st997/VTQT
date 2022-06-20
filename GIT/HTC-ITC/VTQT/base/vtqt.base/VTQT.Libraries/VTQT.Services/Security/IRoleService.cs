using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Master;

namespace VTQT.Services.Security
{
    public partial interface IRoleService
    {
        Task<int> InsertAsync(Role entity);

        Task<int> UpdateAsync(Role entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        IList<Role> GetAll(bool showHidden = false);

        IPagedList<Role> Get(RoleSearchContext ctx);

        Task<Role> GetByIdAsync(string id);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        Task<bool> ExistsAsync(string code);

        Task<bool> ExistsAsync(string oldCode, string newCode);
    }
}

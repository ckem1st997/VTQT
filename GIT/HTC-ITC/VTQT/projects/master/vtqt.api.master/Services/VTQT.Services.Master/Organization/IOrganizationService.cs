using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.FbmOrganization;
using VTQT.SharedMvc.Master.Models;

namespace VTQT.Services.Master
{
    public partial interface IOrganizationService
    {
        IList<OrganizationUnit> GetAll(bool showHidden = false);

        IPagedList<OrganizationUnit> Get(OrganizationSearchContext ctx);

        Task<OrganizationUnit> GetByIdAsync(int id);

        IList<OrganizationTreeModel> GetOrganizationTree(int? expandLevel, bool showHidden = false);

        IList<OrganizationUnit> GetAvailable();

        string GetLastSelectedNodeTree(string appId, string userId, string path, string departmentId);
    }
}

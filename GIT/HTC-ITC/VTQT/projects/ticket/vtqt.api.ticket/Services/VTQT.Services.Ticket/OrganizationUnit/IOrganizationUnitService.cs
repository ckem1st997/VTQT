using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public partial interface IOrganizationUnitService
    {
        Task<int> InsertAsync(OrganizationUnit entity);

        Task<int> UpdateAsync(OrganizationUnit entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<OrganizationUnit> GetByIdAsync(string id);

        Task<OrganizationUnit> GetByCodeAsync(string code);

        Task<bool> ExistedAsync(string code);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<OrganizationUnit> Get(OrganizationUnitSearchContext ctx);

        IList<SelectListItem> GetMvcListItems(bool showHidden, string projectId);

        IList<SelectListItem> GetMvcListChildren(bool showHidden, string projectId);

        IList<SelectListItem> GetProcessingUnitByManagementUnitId(string unitId);

        IList<OrganizationUnit> GetAll(bool showHidden = false);
    }
}

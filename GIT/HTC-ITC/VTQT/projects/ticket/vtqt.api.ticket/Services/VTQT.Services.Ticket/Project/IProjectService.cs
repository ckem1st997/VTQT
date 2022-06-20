using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public partial interface IProjectService
    {
        Task<int> InsertAsync(Project entity);

        Task<int> UpdateAsync(Project entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Project> GetByIdAsync(string id);

        Task<Project> GetByCodeAsync(string code);

        Task<bool> ExistedAsync(string code);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<Project> Get(ProjectSearchContext ctx);

        IList<Project> GetAll(bool showHidden = false);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.FbmContract;

namespace VTQT.Services.Master
{
    public partial interface IProjectService
    {
        IList<Project> GetAll(bool showHidden = false);

        IPagedList<Project> Get(ProjectSearchContext ctx);

        Task<Project> GetByIdAsync(int id);

        IList<Project> GetAvailable();

        Task<Project> GetByCodeAsync(string code);
    }
}

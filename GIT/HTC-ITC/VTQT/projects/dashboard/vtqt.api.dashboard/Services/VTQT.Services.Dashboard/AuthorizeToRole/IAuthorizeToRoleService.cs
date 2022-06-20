using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Dashboard;

namespace VTQT.Services.Dashboard
{
    public interface IAuthorizeToRoleService
    {
        Task<long> InsertAsync(IEnumerable<AuthorizeToRole> entity);

        Task<long> UpdateAsync(IEnumerable<AuthorizeToRole> entity);

        Task<long> DeleteAsync(IEnumerable<string> ids);

        Task<AuthorizeToRole> GetByIdAsync(string id);
        Task<IList<AuthorizeToRole>> GetList();

        IList<AuthorizeToRole> GetAll(string idTypeValue,string idDelegator);
        Task<IPagedList<AuthorizeToRole>> GetAllQuery(AuthorizeToRoleSearchContext context);
        Task<IList<AuthorizeToRole>> GetObject();
        Task<int> GetCountQuery();
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Domain.Dashboard;

namespace VTQT.Services.Dashboard
{
    public interface IFTTHMB3132022Service
    {
        Task<long> InsertAsync(IEnumerable<FTTHMB3132022> entity);

        Task<long> UpdateAsync(IEnumerable<FTTHMB3132022> entity);

        Task<long> DeleteAsync(IEnumerable<string> ids);
        
        Task<FTTHMB3132022> GetByIdAsync(string id);

        IList<FTTHMB3132022> GetAll();
        Task<IList<FTTHMB3132022>> GetList();

        Task<IPagedList<FTTHMB3132022>> GetAllQuery(string key, int numberPage, IEnumerable<SelectListItem> listItem,string idStorageValue);
        Task<IList<FTTHMB3132022>> GetObject();
        Task<int> GetCountQuery();
        
    }
}
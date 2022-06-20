using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Domain.Dashboard;

namespace VTQT.Services.Dashboard
{
    public interface IFTTHService2022
    {
        Task<long> InsertAsync(IEnumerable<FTTH2022> entity);

        Task<long> UpdateAsync(IEnumerable<FTTH2022> entity);

        Task<long> DeleteAsync(IEnumerable<string> ids);
        
        Task<FTTH2022> GetByIdAsync(string id);

        IList<FTTH2022> GetAll();
        Task<IList<FTTH2022>> GetList();

        Task<IPagedList<FTTH2022>> GetAllQuery(string key, int numberPage, IEnumerable<SelectListItem> listItem,string idStorageValue);
        Task<IList<FTTH2022>> GetObject();
        Task<int> GetCountQuery();
        
    }
}
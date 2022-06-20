using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Domain.Dashboard;

namespace VTQT.Services.Dashboard
{
    public interface IFTTHService
    {
        Task<long> InsertAsync(IEnumerable<FTTH> entity);

        Task<long> UpdateAsync(IEnumerable<FTTH> entity);

        Task<long> DeleteAsync(IEnumerable<string> ids);
        
        Task<FTTH> GetByIdAsync(string id);
        Task<IList<FTTH>> GetList();

        IList<FTTH> GetAll();
        Task<IPagedList<FTTH>> GetAllQuery(string key, int numberPage, IEnumerable<SelectListItem> listItem, string idStorageValue);
        Task<IList<FTTH>> GetObject();
        Task<int> GetCountQuery();
        
    }
}
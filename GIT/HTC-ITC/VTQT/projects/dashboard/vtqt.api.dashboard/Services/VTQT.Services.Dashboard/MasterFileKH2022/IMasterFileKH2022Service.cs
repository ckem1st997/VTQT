using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Dashboard;

namespace VTQT.Services.Dashboard
{
    public interface IMasterFileKH2022Service
    {
        Task<long> InsertAsync(IEnumerable<MasterFileKH2022> entity);

        Task<long> UpdateAsync(IEnumerable<MasterFileKH2022> entity);

        Task<long> DeleteAsync(IEnumerable<string> ids);

        Task<MasterFileKH2022> GetByIdAsync(string id);

        IList<MasterFileKH2022> GetAll();
        Task<IPagedList<MasterFileKH2022>> GetAllQuery(string key, int numberPage, IEnumerable<SelectListItem> listItem);
        Task<IList<MasterFileKH2022>> GetObject();
        Task<int> GetCountQuery();
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Dashboard;
using VTQT.SharedMvc.Dashboard.Models;

namespace VTQT.Services.Dashboard
{
    public interface INameTableService
    {
        Task<long> InsertAsync(NameTableExist entity);

        Task<long> UpdateAsync(NameTableExist entity);

        Task<long> DeleteAsync(IEnumerable<string> ids);
        IList<SelectListItem> GetMvcListItems();

        IPagedList<NameTableExist> Get(NameTableSearchContext ctx);


    }
}

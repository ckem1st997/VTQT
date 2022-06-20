using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Dashboard;

namespace VTQT.Services.Dashboard
{
    public interface ISelectTableService
    {
        Task<long> InsertAsync(IEnumerable<VTQT.Core.Domain.Dashboard.SelectTable> entity);

        Task<long> UpdateAsync(IEnumerable<VTQT.Core.Domain.Dashboard.SelectTable> entity);

        Task<long> DeleteAsync(IEnumerable<string> ids);

        Task<VTQT.Core.Domain.Dashboard.SelectTable> GetByIdAsync(string id);

        IList<VTQT.Core.Domain.Dashboard.SelectTable> GetSelect(string NameTable);
        IPagedList<SelectTable> Get(SelectTableSearchContext ctx);

    }
}

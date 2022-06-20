using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface IStationService
    {
        Task<int> InsertAsync(Station entity);

        Task<int> UpdateAsync(Station entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Station> GetByIdAsync(string id);

        Task<Station> GetByCodeAsync(string code);

        Task<bool> ExistedAsync(string code);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<Station> Get(StationSearchContext ctx);

        IList<Station> GetAll(bool showHidden = false);

        IList<SelectListItem> GetMvcListItems(bool showHidden);
    }
}
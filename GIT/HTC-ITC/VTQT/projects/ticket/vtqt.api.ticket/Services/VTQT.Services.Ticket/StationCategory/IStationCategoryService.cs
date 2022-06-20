using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface IStationCategoryService
    {
        Task<int> InsertAsync(StationCategory entity);

        Task<int> UpdateAsync(StationCategory entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<StationCategory> GetByIdAsync(string id);

        IPagedList<StationCategory> Get(StationCategorySearchContext ctx);

        Task<bool> ExistsAsync(string code);

        Task<bool> ExistsAsync(string oldCode, string newCode);

        IList<StationCategory> GetAll(bool showHidden = false);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);
    }
}
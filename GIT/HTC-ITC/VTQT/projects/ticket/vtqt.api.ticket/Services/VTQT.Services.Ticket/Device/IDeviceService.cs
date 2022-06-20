using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface IDeviceService
    {
        Task<int> InsertAsync(Device entity);

        Task<int> UpdateAsync(Device entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Device> GetByIdAsync(string id);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<Device> Get(DeviceSearchContext ctx);

        IList<SelectListItem> GetMvcListItems(bool showHidden);
    }
}
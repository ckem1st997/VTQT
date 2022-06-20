using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface IDeviceCategoryService
    {
        Task<int> InsertAsync(DeviceCategory entity);

        Task<int> UpdateAsync(DeviceCategory entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<DeviceCategory> GetByIdAsync(string id);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<DeviceCategory> Get(DeviceCategorySearchContext ctx);

        IList<DeviceCategory> GetAll(bool showHidden = false);

        Task<bool> ExistsAsync(string code);

        Task<bool> ExistsAsync(string oldCode, string newCode);
        
        IList<SelectListItem> GetMvcListItems(bool showHidden);
    }
}
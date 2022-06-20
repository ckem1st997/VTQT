using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Services.Warehouse
{
    public partial interface ISerialWareHouseService
    {
        Task<int> InsertAsync(SerialWareHouse entity);

        Task<int> UpdateAsync(SerialWareHouse entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        IPagedList<SerialWareHouse> Get(SerialWareHouseSearchContext ctx);

        Task<SerialWareHouse> GetByIdAsync(string id);

        Task<bool> ExistsAsync(string Serial);

        Task<bool> ExistsAsync(string oldSerial, string newSerial);
    }
}

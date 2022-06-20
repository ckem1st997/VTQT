using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Services.Warehouse
{
    public partial interface IUnitService
    {
        Task<int> InsertAsync(Unit entity);

        Task<int> UpdateAsync(Unit entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        IList<Unit> GetSelect();

        IList<Unit> GetAll(bool showHidden = false);

        IPagedList<Unit> Get(UnitSearchContext ctx);

        Task<Unit> GetByIdAsync(string id);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        Task<bool> ExistsAsync(string unitName);

        Task<bool> ExistsAsync(string oldName, string newName);

        string GetUnitNameByWareHouseItemCode(string code);
        Unit GetUnitByWareHouseItemCode(string code);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.SharedMvc.Warehouse.Models.WareHouse;

namespace VTQT.Api.Warehouse.Helper
{
    public interface IWareHouseModelHelper
    {
        public Task<IList<WareHouseTreeModel>> GetWareHouseTree(int? expandLevel, bool showHidden = false);

        public Task<int> CheckUIQuantity(string WareHouseId, string ItemId);

        public Task<IList<WareHouseModel>> GetWareHouseDropdownTreeAsync(bool showHidden = false, bool showList = false);

    }
    
}


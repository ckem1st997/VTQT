using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTQT.Web.Warehouse.Models
{
    public class WareHouseItemCategoryExportModel:STTBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ItemType { get; set; }
        public string Inactive { get; set; }
    }
}

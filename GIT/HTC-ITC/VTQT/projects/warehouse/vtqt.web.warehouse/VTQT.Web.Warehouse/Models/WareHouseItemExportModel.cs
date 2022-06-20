using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTQT.Web.Warehouse.Models
{
    public class WareHouseItemExportModel : STTBase
    {
        public string Inactive { get; set; }
        public string Country { get; set; }
        public string VendorName { get; set; }
        public string Description { get; set; }
        public string ItemType { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}

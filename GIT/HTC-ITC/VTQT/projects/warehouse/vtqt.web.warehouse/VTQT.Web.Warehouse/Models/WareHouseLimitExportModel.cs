using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTQT.Web.Warehouse.Models
{
    public class WareHouseLimitExportModel : STTBase
    {
        public string WareHouse { get; set; }
        public string ItemType { get; set; }
        public string UnitName { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
    }
}

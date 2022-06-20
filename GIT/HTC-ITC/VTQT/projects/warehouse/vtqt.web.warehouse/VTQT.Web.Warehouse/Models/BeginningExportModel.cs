using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTQT.Web.Warehouse.Models
{
    public class BeginningExportModel: STTBase
    {

        public string Item { get; set; }
        public decimal Quantity { get; set; }
        public string UnitName { get; set; }
        public string WareHouse { get; set; }
    }

}

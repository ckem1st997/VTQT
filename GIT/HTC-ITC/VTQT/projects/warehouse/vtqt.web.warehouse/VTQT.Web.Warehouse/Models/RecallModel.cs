using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core.Domain.Warehouse;
using VTQT.SharedMvc.Warehouse.Models;

namespace VTQT.Web.Warehouse.Models
{
    public class RecallModel 
    {
        public int STT { get; set; }
        public  string Name { get; set; }
        public string UnitName { get; set; }

        public  string CodeItem { get; set; }
        public decimal Quantity { get; set; }
        
        public string Status { get; set; }

    }
}

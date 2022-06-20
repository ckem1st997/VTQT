using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTQT.Web.Warehouse.Models
{
    public class AuditExportModel : STTBase
    {
        public string VoucherCode { get; set; }
        public string VoucherDate { get; set; }
        public string WareHouseId { get; set; }
        public string Description { get; set; }
        public  string CreatedDate { get; set; }
        public  string CreatedBy { get; set; }
        public  string ModifiedDate { get; set; }
    }
}

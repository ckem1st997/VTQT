using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTQT.Web.Warehouse.Models
{
    public class WareHouseBookExportModel: STTBase
    {
        public string VoucherType { get; set; }
        public string VoucherCode { get; set; }
        public DateTime VoucherDate { get; set; }
        public string CreatedBy { get; set; }
        public string Deliver { get; set; }
        public string Receiver { get; set; }
        public string ReasonDescription { get; set; }
        public int Reason { get; set; }
        public string StrVoucherDate { get; set; }
        public string InwardReason { get; set; }
        public string OutwardReason { get; set; }
    }
}

using System;

namespace VTQT.Services.Warehouse
{
    public partial class VoucherWareHouseSearchContext
    {
        public string Keywords { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string WareHouseId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int SelectedVoucherType { get; set; }
        
        public int SelectedInwardReason { get; set; }
        
        public int SelectedOutwardReason { get; set; }
    }
}

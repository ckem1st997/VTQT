using System;
using System.Collections.Generic;
using System.Text;

namespace VTQT.Services.Warehouse
{
    public partial class WarehouseBalanceSearchContext
    {
        public string Keywords { get; set; }

        public int? fromQuantity { get; set; }
        public int? toQuantity { get; set; }
        public decimal? fromAmount { get; set; }
        public decimal? toAmount { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}

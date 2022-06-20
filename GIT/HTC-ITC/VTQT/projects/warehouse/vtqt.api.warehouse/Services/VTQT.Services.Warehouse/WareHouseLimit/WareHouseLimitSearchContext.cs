using System;

namespace VTQT.Services.Warehouse
{
    public partial class WareHouseLimitSearchContext
    {
        public string Keywords { get; set; }

        public  string WareHouesId { get; set; }

        public bool DateSoft { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public WareHouseLimitSearchContext()
        {
        }
    }
}
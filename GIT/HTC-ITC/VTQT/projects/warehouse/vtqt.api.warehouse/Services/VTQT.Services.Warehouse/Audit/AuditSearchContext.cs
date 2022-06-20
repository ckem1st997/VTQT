using System;

namespace VTQT.Services.Warehouse
{
    public partial class AuditSearchContext
    {
        public string WareHouesId { get; set; }

        public string Keywords { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string EmployeeId { get; set; }

        public string CustomerCode { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace VTQT.Services.Warehouse
{
    public partial class ReportSearchContext
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string WareHouseId { get; set; }

        public string WareHouseItemId { get; set; }

        public int ReportType { get; set; }

        public string RouteKey { get; set; }

        public int DepartmentId { get; set; }

        public int ProjectId { get; set; }

        public string Proposer { get; set; }

        public string StrFromDate { get; set; }

        public string StrToDate { get; set; }


        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}


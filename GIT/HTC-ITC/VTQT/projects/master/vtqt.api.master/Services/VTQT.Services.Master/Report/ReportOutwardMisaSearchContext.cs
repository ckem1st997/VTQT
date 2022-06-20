using System;

namespace VTQT.Services.Master
{
    public class ReportOutwardMisaSearchContext
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string WareHouseId { get; set; }

        public string WareHouseItemId { get; set; }

        public string Proposer { get; set; }

        public string DepartmentId { get; set; }

        public string ProjectId { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
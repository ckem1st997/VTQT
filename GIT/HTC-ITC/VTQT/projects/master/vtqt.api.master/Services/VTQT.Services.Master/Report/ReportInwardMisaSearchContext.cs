using System;

namespace VTQT.Services.Master
{
    public class ReportInwardMisaSearchContext
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string WareHouseId { get; set; }

        public string WareHouseItemId { get; set; }

        public string Proposer { get; set; }

        public int DepartmentId { get; set; }

        public string ProjectId { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public  string AccountYes { get; set; }

        public string AccountMore { get; set; }
    }
}
using System;

namespace VTQT.Services.Master
{
    public class AssetMaintenanceReportSearchContext
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int AssetType { get; set; }

        public string OrganizationUnitId { get; set; }

        public string StationCode { get; set; }

        public string AssetId { get; set; }
    }
}

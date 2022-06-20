namespace VTQT.Web.Asset.Models
{
    public class AssetMaintenanceReportExcelModel
    {
        public int Stt { get; set; }

        public string AssetName { get; set; }

        public string OrganizationUnitName { get; set; }

        public string MaintenanceLocation { get; set; }

        public string Serial { get; set; }

        public string MaintenanceDate { get; set; }

        public string CurrentUsageStatus { get; set; }

        public string ReasonDescription { get; set; }

        public string Content { get; set; }
    }
}

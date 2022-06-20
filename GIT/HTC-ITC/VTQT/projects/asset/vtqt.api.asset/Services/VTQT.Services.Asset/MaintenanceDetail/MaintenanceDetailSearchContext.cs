namespace VTQT.Services.Asset
{
    public partial class MaintenanceDetailSearchContext
    {
        public string Keywords { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string AssetCategoryId { get; set; }

        public string OrganizationId { get; set; }

        public string StationCode { get; set; }

        public string ProjectCode { get; set; }       

        public int AssetType { get; set; }
    }
}

namespace VTQT.Web.Asset.Models
{
    public class AssetExcelModel
    {
        public int Stt { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int Status { get; set; }

        public string CategoryName { get; set; }

        public string WareHouseItemCode { get; set; }

        public string WareHouseItemName { get; set; }

        public decimal OriginQuantity { get; set; }

        public decimal RecallQuantity { get; set; }

        public decimal BrokenQuantity { get; set; }

        public decimal SoldQuantity { get; set; }

        public string OriginUsageStatus { get; set; }

        public decimal OriginValue { get; set; }

        public string CurrentUsageStatus { get; set; }

        public decimal CurrentValue { get; set; }

        public int DepreciationDuration { get; set; }

        public int DepreciationUnit { get; set; }

        public int WarrantyDuration { get; set; }

        public int WarrantyUnit { get; set; }

        public string WarrantyCondition { get; set; }

        public string VendorName { get; set; }

        public string Country { get; set; }

        public int ManufactureYear { get; set; }

        public string Description { get; set; }

        public int AssetType { get; set; }

        public string OrganizationUnitName { get; set; }

        public string EmployeeName { get; set; }

        public string StationCode { get; set; }

        public string StationName { get; set; }

        public string ProjectCode { get; set; }

        public string ProjectName { get; set; }

        public string CustomerCode { get; set; }

        public string CustomerName { get; set; }

        public string CreatedDate { get; set; }

        public string AllocationDate { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        public string SelectedDepreciationUnit { get; set; }

        public string SelectedWarrantyUnit { get; set; }

        public string UnitName { get; set; }

        public string MaintenancedDate { get; set; }

        public decimal BalanceQuantity { get; set; }

        public string StationCategory { get; set; }

        public string StationArea { get; set; }

        public string StationAddress { get; set; }

        public double StationLongitude { get; set; }

        public double StationLatitude { get; set; }

        public string CustomerAddress { get; set; }

        public string Serial { get; set; }
    }
}

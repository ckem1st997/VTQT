using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTQT.Web.Asset.Models
{
    public class ReportAssetExportModel : STTBase
    {
        public string Area { get; set; }
        public string StationCode { get; set; }
        public string StationName { get; set; }
        public string Note { get; set; }
        public string LongItude { get; set; }
        public string LatItude { get; set; }
        public string Name { get; set; }
        public string Quantity { get; set; }
        public string UnitName { get; set; }
        public string CurrentUsageStatus { get; set; }

        public string Code { get; set; }
        
        public int Status { get; set; }

        public string CategoryId { get; set; }

        public string WareHouseItemCode { get; set; }

        public string WareHouseItemName { get; set; }

        public Decimal OriginQuantity { get; set; }

        public Decimal RecallQuantity { get; set; }

        public Decimal BrokenQuantity { get; set; }

        public Decimal SoldQuantity { get; set; }

        public string OriginUsageStatus { get; set; }

        public Decimal OriginValue { get; set; }
        
        public Decimal CurrentValue { get; set; }

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

        public string OrganizationUnitId { get; set; }

        public string OrganizationUnitName { get; set; }

        public string EmployeeId { get; set; }

        public string EmployeeName { get; set; }
        

        public string ProjectCode { get; set; }

        public string ProjectName { get; set; }

        public string CustomerCode { get; set; }

        public string CustomerName { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? AllocationDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }


        public string SelectedDepreciationUnit { get; set; }

        public string SelectedWarrantyUnit { get; set; }

        public string UnitId { get; set; }
        
        public DateTime? MaintenancedDate { get; set; }

        public Decimal BalanceQuantity { get; set; }

        public string StationCategory { get; set; }

        public string StationArea { get; set; }

        public string StationAddress { get; set; }

        public double StationLongitude { get; set; }

        public double StationLatitude { get; set; }

        public string CustomerAddress { get; set; }

        public string Serial { get; set; }
    }
}
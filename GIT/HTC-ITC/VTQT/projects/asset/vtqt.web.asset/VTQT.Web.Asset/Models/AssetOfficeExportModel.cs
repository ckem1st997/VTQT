using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTQT.Web.Asset.Models
{
    public class AssetOfficeExportModel : STTBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string UnitName { get; set; }
        public string OrganizationUnitName { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string WareHouseItemCode { get; set; }
        public string WareHouseItemName { get; set; }
        public decimal OriginQuantity { get; set; }
        public decimal RecallQuantity { get; set; }
        public decimal BrokenQuantity { get; set; }
        public decimal SoldQuantity { get; set; }
        public decimal MinusQuantity { get; set; }
        public string OriginUsageStatus { get; set; }
        public decimal OriginValue { get; set; }
        public decimal CurrentValue { get; set; }
        public string CurrentUsageStatus { get; set; }
        public int DepreciationDuration { get; set; }
        public string SelectedDepreciationUnit { get; set; }
        public string WarrantyDuration { get; set; }
        public string SelectedWarrantyUnit { get; set; }
        public int WarrantyCondition { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CreatedDate { get; set; }
        public string DateBaoDuong { get; set; }

    }
}

using System;
using VTQT.Web.Framework;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public partial class ReportResponseModel
    {
        [XBaseResourceDisplayName("WareHouse.WareHouseItems.Fields.Code")]
        public string WarehouseItemCode { get; set; }

        [XBaseResourceDisplayName("WareHouse.WareHouseItems.Fields.Name")]
        public string WarehouseItemName { get; set; }

        [XBaseResourceDisplayName("WareHouse.Report.Fields.Inward")]
        public decimal InwardQuantity { get; set; }

        [XBaseResourceDisplayName("WareHouse.Report.Fields.Outward")]
        public decimal OutwardQuantity { get; set; }

        [XBaseResourceDisplayName("WareHouse.Report.Fields.Balance")]
        public decimal BalanceQuantity { get; set; }

        [XBaseResourceDisplayName("WareHouse.WarehouseBalance.Fields.Quantity")]
        public decimal TotalQuantity { get; set; }

        [XBaseResourceDisplayName("WareHouse.Units.Fields.UnitName")]
        public string UnitName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Generic")]
        public string Generic { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Purpose")]
        public string Purpose { get; set; }

        [XBaseResourceDisplayName("Common.Fields.DepartmentName")]
        public string DepartmentName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Proposer")]
        public string Proposer { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Description")]
        public string Description { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Note")]
        public string Note { get; set; }

        [XBaseResourceDisplayName("Warehouse.Report.Fields.BeginningQuantity")]
        public decimal BeginningQuantity { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ProjectName")]
        public string ProjectName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.DateTime")]
        public DateTime Date { get; set; }
    }
}
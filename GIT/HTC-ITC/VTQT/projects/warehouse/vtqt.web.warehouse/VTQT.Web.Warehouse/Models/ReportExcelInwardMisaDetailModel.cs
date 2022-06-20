namespace VTQT.Web.Warehouse.Models
{
    public class ReportExcelInwardMisaDetailModel
    {
        public int STT { get; set; }

        public  string VoucherDateTime { get; set; }
        public  string VoucherCode { get; set; }

        public string Voucher { get; set; }

        public  string VendorCode { get; set; }

        public  string ProjectId { get; set; }
        public  string WareHouseItemId { get; set; }

        public  string AccountMore { get; set; }
        public  string AccountYes { get; set; }
        public  decimal Quantity { get; set; }

        public  decimal Price { get; set; }

        public  decimal Amount { get; set; }
        public string WareHouseItemCode { get; set; }

        public string WareHouseItemName { get; set; }

        public decimal Beginning { get; set; }

        public decimal Import { get; set; }

        public decimal Export { get; set; }

        public decimal Balance { get; set; }

        public string UnitName { get; set; }

        public string Category { get; set; }

        public string Purpose { get; set; }

        public string Proposer { get; set; }

        public string DepartmentName { get; set; }

        public string ProjectName { get; set; }

        public string NoteRender { get; set; }

        public string Description { get; set; }

        public string Moment { get; set; }

        public string VoucherCodeImport { get; set; }

        public string VoucherCodeExport { get; set; }
    }
}
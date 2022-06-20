namespace VTQT.Web.Warehouse.Models
{
    public class ReportExcelDetailModel
    {
        public int STT { get; set; }

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

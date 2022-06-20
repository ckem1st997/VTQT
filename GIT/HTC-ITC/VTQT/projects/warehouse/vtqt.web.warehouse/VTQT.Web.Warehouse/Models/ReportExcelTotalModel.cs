namespace VTQT.Web.Warehouse.Models
{
    public class ReportExcelTotalModel
    {
        public int STT { get; set; }

        public string WareHouseItemCode { get; set; }

        public string WareHouseItemName { get; set; }

        public decimal Beginning { get; set; }

        public decimal Import { get; set; }

        public decimal Export { get; set; }

        public decimal Balance { get; set; }

        public string UnitName { get; set; }
    }
}

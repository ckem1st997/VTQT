namespace VTQT.Web.Asset.Models
{
    public class AuditDetailRecallModel
    {
        public int STT { get; set; }
        public string ItemName { get; set; }
        public string UnitName { get; set; }
        public decimal Quantity { get; set; }
        public decimal AuditQuantity { get; set; }
        public string Conclude { get; set; }
    }
}
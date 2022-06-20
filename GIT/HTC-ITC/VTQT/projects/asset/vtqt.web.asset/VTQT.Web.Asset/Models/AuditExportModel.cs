namespace VTQT.Web.Asset.Models
{
    public class AuditExportModel : STTBase
    {
        public string AssetType { get; set; }
        public string VoucherCode { get; set; }
        public string VoucherDate { get; set; }
        public string Description { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedDate { get; set; }

        public  string AuditLocation { get; set; }
    }
}
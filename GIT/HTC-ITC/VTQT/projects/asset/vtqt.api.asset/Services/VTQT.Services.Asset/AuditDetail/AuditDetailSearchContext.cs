namespace VTQT.Services.Asset
{
    public class AuditDetailSearchContext
    {
        public string AuditId { get; set; }

        public string Keywords { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
namespace VTQT.Services.Asset
{
    public partial class HistorySearchContext
    {
        public string Keywords { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string AssetId { get; set; }

        public string AssetAttachmentId { get; set; }
    }
}

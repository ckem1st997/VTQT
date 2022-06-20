namespace VTQT.Services.Ticket
{
    public class ChannelSearchContext
    {
        public string Keywords { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int? Status { get; set; }

        public string LanguageId { get; set; }
    }
}
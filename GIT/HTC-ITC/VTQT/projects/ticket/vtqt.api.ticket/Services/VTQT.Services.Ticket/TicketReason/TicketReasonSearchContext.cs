namespace VTQT.Services.Ticket
{
    public partial class TicketReasonSearchContext
    {
        public string Keywords { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int? Status { get; set; }
    }
}

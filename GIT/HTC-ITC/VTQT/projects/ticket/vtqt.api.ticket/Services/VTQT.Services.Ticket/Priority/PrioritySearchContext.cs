namespace VTQT.Services.Ticket
{
    public class PrioritySearchContext
    {
        public string Keywords { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int? Status { get; set; }
    }
}
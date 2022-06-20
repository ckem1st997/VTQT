using System;

namespace VTQT.Services.Ticket
{
    public partial class ProblemTicketSearchContext
    {
        public string Keywords { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string LanguageId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? FinishDate { get; set; }

        public string ProjectId { get; set; }

        public string CreatedBy { get; set; }
        public string Priority { get; set; }
        public string Trangthai { get; set; }
        public string Vungsuco { get; set; }

        public  string TicketId { get; set; }
    }
}

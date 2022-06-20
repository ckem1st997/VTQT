using System;

namespace VTQT.Services.Ticket
{
    public partial class StationTicketSearchContext
    {
        public string Keywords { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string StatusId { get; set; }

        public string ProjectId { get; set; }

        public string Assignee { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? FinishDate { get; set; }

        public string PriorityId { get; set; }

        public string TicketAreaId { get; set; }

        public string TicketProvinceId { get; set; }
    }
}

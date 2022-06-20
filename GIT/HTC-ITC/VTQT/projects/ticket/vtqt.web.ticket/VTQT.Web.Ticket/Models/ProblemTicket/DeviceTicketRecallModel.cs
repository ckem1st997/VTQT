using System;

namespace VTQT.Web.Ticket.Models.ProblemTicket
{
    public class DeviceTicketRecallModel : BaseStt
    {
        public string DeviceId { get; set; }

        public string DeviceName { get; set; }

        public string CategoryId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? FinishDate { get; set; }

        public string PhenomenaId { get; set; }
    }
}
using System;

namespace VTQT.Web.Ticket.Models.ProblemTicket
{
    public class NetworkLinkTicketRecallModel : BaseStt
    {
        public string TicketId { get; set; }

        public string CableId { get; set; }

        public string NetworkLinkId { get; set; }

        public string NetworkLinkName { get; set; }

        public string CategoryId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? FinishDate { get; set; }

        public string PhenomenaId { get; set; }
    }
}
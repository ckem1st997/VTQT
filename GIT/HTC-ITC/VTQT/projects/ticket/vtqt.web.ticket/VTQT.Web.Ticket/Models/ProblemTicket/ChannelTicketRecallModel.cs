using System;

namespace VTQT.Web.Ticket.Models.ProblemTicket
{
    public class ChannelTicketRecallModel : BaseStt
    {
        public string CableId { get; set; }

        public string ChannelId { get; set; }

        public string ChannelName { get; set; }

        public string CategoryId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? FinishDate { get; set; }

        public string PhenomenaId { get; set; }
    }
}
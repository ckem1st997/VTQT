namespace VTQT.Web.Ticket.Models.ProblemTicket
{
    public class ProblemTicketRecallModel : BaseStt
    {
        public string TicketArea { get; set; }

        public string TicketProvince { get; set; }

        public int? ChannelCapacity { get; set; }

        public string EcalatePosition { get; set; }

        public string ProcessingUnit { get; set; }
        public string KindOfReason { get; set; }

        public string DetailReason { get; set; }

        public string ProcessingDepartment { get; set; }

        public bool SlaOver { get; set; }

        public bool ImportantTicket { get; set; }

        public decimal? Sla { get; set; }

        public int? HourTimeMinus { get; set; }

        public int? MinuteTimeMinus { get; set; }

        public int? SecondTimeMinus { get; set; }

        public string TimeMinus { get; set; }
    }
}
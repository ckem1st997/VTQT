namespace VTQT.Web.Ticket.Models.ProblemTicket
{
    public class ApprovalTicketRecallModel : BaseStt
    {
        public bool Confirm { get; set; }

        public string ReasonDetail { get; set; }

        public string Approver { get; set; }

        public string Progress { get; set; }
    }
}
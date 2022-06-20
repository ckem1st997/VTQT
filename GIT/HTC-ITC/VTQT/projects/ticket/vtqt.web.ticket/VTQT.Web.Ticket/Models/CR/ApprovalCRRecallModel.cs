namespace VTQT.Web.Ticket.Models.CR
{
    public class ApprovalCRRecallModel: BaseStt
    {
        public bool Confirm { get; set; }

        public string ReasonDetail { get; set; }

        public string Approver { get; set; }

        public string Progress { get; set; }
    }
}
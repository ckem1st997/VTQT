namespace VTQT.Web.Ticket.Models.CR
{
    public class ConfirmCRRecallModel : BaseStt
    {
        public bool Confirm { get; set; } // bit(1)
        public string ReasonDetail { get; set; } // varchar(500)

        public string Approver { get; set; } // varchar(100)

        public string Progress { get; set; }
    }
}
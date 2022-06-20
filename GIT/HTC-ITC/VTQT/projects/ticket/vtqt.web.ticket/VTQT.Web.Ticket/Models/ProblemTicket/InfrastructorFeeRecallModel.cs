namespace VTQT.Web.Ticket.Models.ProblemTicket
{
    public class InfrastructorFeeRecallModel : BaseStt
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Fee { get; set; }

        public string WareHouseItemCode { get; set; }

        public string WareHouseItemName { get; set; }
    }
}
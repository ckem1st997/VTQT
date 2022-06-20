namespace VTQT.Web.Ticket.Models.CR
{
    public class InfrastructorFeeCRRecallModel : BaseStt
    {
        public string Code { get; set; } // varchar(50)

        public string Name { get; set; } // varchar(255)

        public string Description { get; set; } // varchar(500)

        public decimal Fee { get; set; } // decimal(10,0)

        public string WareHouseItemCode { get; set; } // varchar(50)

        public string WareHouseItemName { get; set; } // varchar(255)
    }
}
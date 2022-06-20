namespace VTQT.Web.Ticket.Models
{
    public class StatusExportModel: BaseStt
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string StatusCategoryId { get; set; }
        public string ProjectId { get; set; }
        public string Inactive { get; set; }
    }
}
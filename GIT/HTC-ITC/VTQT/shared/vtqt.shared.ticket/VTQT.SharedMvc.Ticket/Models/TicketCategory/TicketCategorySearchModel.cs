using VTQT.Core.Domain;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Ticket.Models
{
    public class TicketCategorySearchModel : BaseSearchModel
    {
        public ActiveStatus Status { get; set; }
    }
}

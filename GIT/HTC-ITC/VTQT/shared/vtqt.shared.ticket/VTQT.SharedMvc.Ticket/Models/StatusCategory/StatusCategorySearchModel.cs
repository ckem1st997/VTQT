using VTQT.Core.Domain;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Ticket.Models
{
    public partial class StatusCategorySearchModel: BaseSearchModel
    {
        public ActiveStatus Status { get; set; }
    }
}

using VTQT.Core.Domain;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Master.Models
{
    public class RoleSearchModel : BaseSearchModel
    {
        [XBaseResourceDisplayName("Common.Fields.Status")]
        public ActiveStatus Status { get; set; }
    }
}

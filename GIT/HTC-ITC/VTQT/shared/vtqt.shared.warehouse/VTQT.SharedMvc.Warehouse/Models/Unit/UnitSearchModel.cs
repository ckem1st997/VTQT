using VTQT.Core.Domain;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public class UnitSearchModel : BaseSearchModel
    {
        [XBaseResourceDisplayName("Common.Fields.ActiveStatus")]
        public ActiveStatus ActiveStatus { get; set; }
    }
}

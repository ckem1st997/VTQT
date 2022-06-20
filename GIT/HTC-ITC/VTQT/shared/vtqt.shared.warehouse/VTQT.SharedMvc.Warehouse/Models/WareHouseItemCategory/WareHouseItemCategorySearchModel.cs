using VTQT.Core.Domain;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public class WareHouseItemCategorySearchModel : BaseSearchModel
    {
        [XBaseResourceDisplayName("Common.Fields.Status")]
        public ActiveStatus Status { get; set; }
    }
}

using VTQT.Core.Domain;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public class WareHouseItemSearchModel : BaseSearchModel
    {
        public ActiveStatus Status { get; set; } 
    }
}

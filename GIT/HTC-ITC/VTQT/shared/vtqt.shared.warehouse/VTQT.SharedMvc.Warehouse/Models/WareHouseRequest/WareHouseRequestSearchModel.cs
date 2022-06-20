using VTQT.Core.Domain.Warehouse.Enum;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public class WareHouseRequestSearchModel : BaseSearchModel
    {
        public WareHouseRequestStatus WareHouseRequestStatus { get; set; }
        public  Status Status { get; set; }

        public string WareHouesId { get; set; }
    }
}
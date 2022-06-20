using VTQT.Core.Domain;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public class VendorSearchModel : BaseSearchModel
    {
        public ActiveStatus ActiveStatus { get; set; }
    }
}

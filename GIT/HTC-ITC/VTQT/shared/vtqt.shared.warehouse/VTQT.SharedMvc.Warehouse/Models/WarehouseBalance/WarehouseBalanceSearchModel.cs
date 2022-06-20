using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public partial class WarehouseBalanceSearchModel : BaseSearchModel
    {
        public int? fromQuantity { get; set; }
        public int? toQuantity { get; set; }
        public decimal? fromAmount { get; set; }
        public decimal? toAmount { get; set; }

        public WarehouseBalanceSearchModel()
        {

        }
    }
}

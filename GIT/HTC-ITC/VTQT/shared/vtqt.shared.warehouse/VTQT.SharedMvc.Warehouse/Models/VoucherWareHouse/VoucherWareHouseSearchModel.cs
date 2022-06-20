using System;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public class VoucherWareHouseSearchModel : BaseSearchModel
    {
        public string WareHouseId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.FromDate")]
        public DateTime? FromDate { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ToDate")]
        public DateTime? ToDate { get; set; }
        
        [XBaseResourceDisplayName("WareHouse.VoucherWareHouses.Fields.VoucherType")]
        public int SelectedTypeVoucher { get; set; }
        
        [XBaseResourceDisplayName("Common.Fields.SelectedInwardReason")]
        public int SelectedInwardReason { get; set; }
        
        [XBaseResourceDisplayName("Common.Fields.SelectedOutwardReason")]
        public int SelectedOutwardReason { get; set; }

        public string StrFromDate { get; set; }

        public string StrToDate { get; set; }
    }
}

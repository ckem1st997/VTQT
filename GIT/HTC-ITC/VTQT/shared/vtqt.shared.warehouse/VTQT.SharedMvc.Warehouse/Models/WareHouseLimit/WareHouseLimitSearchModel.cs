using System;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public class WareHouseLimitSearchModel : BaseSearchModel
    {
        public bool DateSoft { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string WareHouesId { get; set; }

        public WareHouseLimitSearchModel()
        {
            DateSoft = true;
        }
    }
}

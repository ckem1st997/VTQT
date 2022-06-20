using System;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public class BeginningWareHouseSearchModel : BaseSearchModel
    {
        public bool DateSoft { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string WareHouesId { get; set; }

        public BeginningWareHouseSearchModel()
        {
            DateSoft = true;
        }
    }
}

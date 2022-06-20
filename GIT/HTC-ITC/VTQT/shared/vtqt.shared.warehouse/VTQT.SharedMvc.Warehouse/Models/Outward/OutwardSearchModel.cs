using System;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public partial class OutwardSearchModel : BaseSearchModel
    {
        public int? Reason { get; set; }

        public bool DateSoft { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public OutwardSearchModel()
        {
            DateSoft = true;
        }
    }
}
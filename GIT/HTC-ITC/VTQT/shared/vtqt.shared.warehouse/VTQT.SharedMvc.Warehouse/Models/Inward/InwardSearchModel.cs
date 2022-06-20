using System;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public class InwardSearchModel : BaseSearchModel
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}

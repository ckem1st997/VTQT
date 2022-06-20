using System;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public class AuditDetailSearchModel : BaseSearchModel
    {
        public string AuditId { get; set; }

        public string WareHouesId { get; set; }

        public DateTime VoucherDate { get; set; }
    }
}
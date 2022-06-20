using System;
using VTQT.Core.Domain;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Dashboard.Models
{
    public partial class StorageValueSearchModel : BaseSearchModel
    {
        public string TypeValueId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Status")]
        public ActiveStatus Status { get; set; }
    }
}
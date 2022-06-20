using System;
using System.Collections.Generic;
using System.Text;

namespace VTQT.Services.Master
{
   public partial class ReportAssetInfrastructorSearchContext
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string OrganizationUnitId { get; set; }

        public string ItemCode { get; set; }

        public string KeyWords { get; set; }
        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}

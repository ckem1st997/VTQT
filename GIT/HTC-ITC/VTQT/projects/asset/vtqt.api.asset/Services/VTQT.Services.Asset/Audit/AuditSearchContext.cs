using System;

namespace VTQT.Services.Asset
{
    public partial class AuditSearchContext
    {
        public int AssetType { get; set; }
        public string OrganizationId { get; set; }

        public string Keywords { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string EmployeeId { get; set; }
    }
}
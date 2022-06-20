using System;

namespace VTQT.Services.Asset
{
    public partial class AssetAttachmentSearchContext
    {
        public string Keywords { get; set; }

        public int AssetType { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string OrganizationId { get; set; }

        public string EmployeeId { get; set; }

        public string CustomerCode { get; set; }

        public string ProjectCode { get; set; }

        public string StationCode { get; set; }

        public string CategoryId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string LanguageId { get; set; }
    }
}
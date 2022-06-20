using System;

namespace VTQT.Services.Dashboard
{
    public partial class StorageValueSearchContext
    {
        public string Keywords { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int Date { get; set; }

        public string TypeValueId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
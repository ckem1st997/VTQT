using System;
using System.Collections.Generic;
using System.Text;

namespace VTQT.Services.Asset
{
    public partial class ChartSearchContext
    {
        public string Keywords { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
        public int Date { get; set; }
        public string OrganizationId { get; set; }
    }
}
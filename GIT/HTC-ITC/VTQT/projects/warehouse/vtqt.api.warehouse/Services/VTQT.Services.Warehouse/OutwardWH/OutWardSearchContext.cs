using System;
using System.Collections.Generic;
using System.Text;

namespace VTQT.Services.Warehouse
{
    public partial class OutWardSearchContext
    {
        public string Keywords { get; set; }

        public int? Reason { get; set; }

        public bool DateSoft { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public OutWardSearchContext()
        {
        }
    }
}

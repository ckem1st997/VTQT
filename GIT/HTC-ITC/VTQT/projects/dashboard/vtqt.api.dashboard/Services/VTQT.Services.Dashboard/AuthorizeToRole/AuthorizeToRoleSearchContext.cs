using System;
using System.Collections.Generic;
using System.Text;

namespace VTQT.Services.Dashboard
{
    public class AuthorizeToRoleSearchContext
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
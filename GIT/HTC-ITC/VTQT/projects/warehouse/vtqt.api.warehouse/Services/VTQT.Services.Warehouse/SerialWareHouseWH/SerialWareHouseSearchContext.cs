using System;
using System.Collections.Generic;
using System.Text;

namespace VTQT.Services.Warehouse
{
    public partial class SerialWareHouseSearchContext
    {
        public string Keywords { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}

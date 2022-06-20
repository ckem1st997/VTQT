using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTQT.Web.Warehouse.Models
{
    public class SelectItem
    {

        public object id { get; set; }
        public string text { get; set; }

        public bool selected { get; set; } = false;
    }
}

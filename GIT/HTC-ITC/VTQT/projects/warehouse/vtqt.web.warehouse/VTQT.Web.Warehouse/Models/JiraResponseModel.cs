using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTQT.Web.Warehouse.Models
{
    public class JiraResponseModel : JiraGetValue
    {

        public string RequestType
        { get; set; }        
        public string ColorStatus
        { get; set; }

        public DateTime Created { get; set; }

        public bool Warn { get; set; }
    }
}

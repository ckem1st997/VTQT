using System.Collections.Generic;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Master.Models
{
    public partial class ReportTreeModel : FancytreeItem
    {
        public ReportTreeModel()
        {
            children = new List<ReportTreeModel>();
        }

        public string Name { get; set; } // varchar(255)

        public string Description { get; set; } // varchar(255)

        public string AppId { get; set; } // varchar(36)

        public int Type { get; set; } // int

        public string Route { get; set; } // varchar(255)

        public string ReportTemplate { get; set; } // varchar(255)

        public string ExcelTemplate { get; set; } // varchar(255)

        public int Order { get; set; } // int

        public bool Inactive { get; set; } // bit(1)

        public new IList<ReportTreeModel> children { get; set; }
    }
}

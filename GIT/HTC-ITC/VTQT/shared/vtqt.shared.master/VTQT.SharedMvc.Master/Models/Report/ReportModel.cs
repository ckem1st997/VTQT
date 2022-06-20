using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Master.Models
{
    public partial class ReportModel : BaseEntityModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string AppId { get; set; }

        public int Type { get; set; }

        public string Route { get; set; }

        public string ReportTemplate { get; set; }

        public string ExcelTemplate { get; set; }

        public int Order { get; set; }

        public bool Inactive { get; set; }
    }
}

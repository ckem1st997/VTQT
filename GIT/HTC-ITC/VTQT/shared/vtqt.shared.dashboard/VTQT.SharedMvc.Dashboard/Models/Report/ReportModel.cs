using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Dashboard.Models
{
    public class ReportModel : BaseEntityModel
    {
        public string ParentId { get; set; } // varchar(36)
        public string Name { get; set; } // varchar(255)
        public string Description { get; set; } // varchar(4000)
        public string Path { get; set; } // varchar(1000)
        public bool Inactive { get; set; } // bit(1)
    }
}
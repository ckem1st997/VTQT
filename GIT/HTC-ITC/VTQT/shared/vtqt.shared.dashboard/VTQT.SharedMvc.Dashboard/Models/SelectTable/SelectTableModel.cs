using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Dashboard.Models
{
    public class SelectTableModel : BaseEntityModel
    {
        public string TextShow { get; set; } // varchar(255)
        public string ValueShow { get; set; } // varchar(255)
        public string TableShow { get; set; } // varchar(255)
    }
}
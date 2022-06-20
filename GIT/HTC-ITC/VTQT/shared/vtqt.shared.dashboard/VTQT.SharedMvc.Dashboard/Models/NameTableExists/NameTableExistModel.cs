using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Dashboard.Models
{
    public class NameTableExistModel : BaseEntityModel
    {
        public string Name { get; set; } // varchar(50)
        public string NameDes { get; set; } // varchar(255)
    }
}
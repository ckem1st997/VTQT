using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Master.Models
{
    public class OrganizationModel : BaseIntEntity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public string Path { get; set; }
    }
}

using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Master.Models
{
    public class CustomerModel : BaseIntEntity
    {
        public string Code { get; set; }

        public string Name { get; set; }
    }
}

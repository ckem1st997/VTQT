using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Master.Models
{
    public partial class UserModel : BaseEntityModel
    {
        [XBaseResourceDisplayName("Common.Fields.UserName")]
        public string UserName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.FirstName")]
        public string FirstName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.LastName")]
        public string LastName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.FullName")]
        public string FullName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Email")]
        public string Email { get; set; }

        [XBaseResourceDisplayName("Common.Fields.EmailConfirmed")]
        public bool EmailConfirmed { get; set; }

        [XBaseResourceDisplayName("Common.Fields.PhoneNumber")]
        public string PhoneNumber { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Active")]
        public bool Active { get; set; }

        public IList<SelectListItem> AvailableApps { get; set; }

        public UserModel()
        {
            Active = true;
            AvailableApps = new List<SelectListItem>();
        }
    }
}

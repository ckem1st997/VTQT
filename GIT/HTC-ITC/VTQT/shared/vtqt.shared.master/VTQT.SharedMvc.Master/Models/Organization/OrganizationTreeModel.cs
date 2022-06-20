using System.Collections.Generic;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Master.Models
{
    public partial class OrganizationTreeModel : FancytreeItem
    {
        public OrganizationTreeModel()
        {
            children = new List<OrganizationTreeModel>();
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public int? ParentId { get; set; }

        public string Path { get; set; }

        public string Description { get; set; }

        public bool Inactive { get; set; }

        public new IList<OrganizationTreeModel> children { get; set; }
    }
}
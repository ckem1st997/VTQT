using System;
using System.Collections.Generic;
using System.Text;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Dashboard.Models
{
    public partial class TypeValuesTreeModel : FancytreeItem
    {
        public TypeValuesTreeModel() => this.children = (IList<TypeValuesTreeModel>)new List<TypeValuesTreeModel>();

        public string Code { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string ParentId { get; set; }

        public string Path { get; set; }

        public string Description { get; set; }

        public bool Inactive { get; set; }

        public IList<TypeValuesTreeModel> children { get; set; }
    }
}

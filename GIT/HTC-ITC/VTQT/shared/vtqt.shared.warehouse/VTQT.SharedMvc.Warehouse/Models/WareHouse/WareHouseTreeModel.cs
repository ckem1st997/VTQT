using System;
using System.Collections.Generic;
using System.Text;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models.WareHouse
{
    public partial class WareHouseTreeModel : FancytreeItem
    {
        public WareHouseTreeModel()
        {
            children = new List<WareHouseTreeModel>();
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string ParentId { get; set; }

        public string Path { get; set; }

        public string Description { get; set; }

        public bool Inactive { get; set; }


        public new IList<WareHouseTreeModel> children { get; set; }
    }
}
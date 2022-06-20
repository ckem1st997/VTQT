using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTQT.Web.Asset.Models
{
    public class AssetCategoryExportModel : STTBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Inactive { get; set; }
    }
}

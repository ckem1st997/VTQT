using Microsoft.AspNetCore.Http;
using VTQT.Web.Framework;

namespace VTQT.Web.Dashboard.Models
{
    public class AddTableModel
    {
        [XBaseResourceDisplayName("Common.Fields.AddTable.NameTable", "NameTable")]
        public string NameTable { get; set; }
        [XBaseResourceDisplayName("Common.Fields.AddTable.Description", "Description")]

        public string Description { get; set; }
        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.NumberHeader", "NumberHeader")]
        public int NumberHeader { get; set; }
        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.SaveValue", "SaveValue")]
        public string SaveValue { get; set; }
        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.SheeActive", "SheeActive")]
        public string SheeActive { get; set; }
        
        
        [XBaseResourceDisplayName("Common.Fields.AddTable.ListColumn", "ListColumn")]
        public string ListColumn { get; set; }
        
        [XBaseResourceDisplayName("Common.Fields.AddTable.ActiveInput", "ActiveInput")]
        public bool ActiveInput { get; set; }
        public IFormFile FormFile { get; set; }
        public string Data { get; set; }
    }
}
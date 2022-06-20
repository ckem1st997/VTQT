using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.Web.Dashboard.Models
{
    public class UpdateImportModels: BaseEntityModel
    {
        
        public UpdateImportModels()
        {
            this.AvailableNameTable = new List<SelectListItem>();
            this.AvailableTypeValue = new List<SelectListItem>();
            this.ListItems = new List<SelectListItem>();
        }
        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.NumberHeader", "NumberHeader")]
        public int NumberHeader { get; set; }
        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.NameTableRefense", "NameTableRefense")]
        public string NameTableRefense { get; set; }
        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.SheeActive", "SheeActive")]
        public string SheeActive { get; set; }
        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.NameColumn", "NameColumn")]
        public string NameColumn { get; set; }
       
        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.OptionSelectColumn", "OptionSelectColumn")]
        public string OptionSelectColumn { get; set; }     
        public IFormFile FormFile { get; set; }
        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.SaveValue", "SaveValue")]
        public string SaveValue { get; set; }
        // [XBaseResourceDisplayName("Common.Fields.StorageValueModel.TypeValueId", "TypeValueId")]
        // public string TypeValueId { get; set; }
        public string TypeValueName { get; set; }
        public string DataSave { get; set; }
        public int StartWith { get; set; }
        public string UpdateSelectColumn { get; set; }
        public string CoulmnMapping { get; set; }
        public IList<SelectListItem> AvailableNameTable { get; set; }
        public IList<SelectListItem> AvailableTypeValue { get; set; }
        public IList<SelectListItem> ListItems { get; set; }

    }
}
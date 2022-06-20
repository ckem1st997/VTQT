using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Dashboard.Models
{
    public partial class StorageValueModel : BaseEntityModel
    {
        /// <summary>
        /// Tên dữ liệu
        /// </summary>
        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.Name")]
        public string Name { get; set; } // varchar(100)

        /// <summary>
        /// Ngày
        /// </summary>
        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.TimeDay")]
        public int TimeDay { get; set; } // int

        /// <summary>
        /// Tháng
        /// </summary>
        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.TimeMouth")]
        public int TimeMouth { get; set; } // int

        /// <summary>
        /// Năm
        /// </summary>
        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.TimeYear")]
        public int TimeYear { get; set; } // int

        /// <summary>
        /// Trạng thái
        /// </summary>
        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.Status")]
        public int Status { get; set; } // int

        /// <summary>
        /// Ngày tạo
        /// </summary>
        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.CreatedDate")]
        public DateTime CreatedDate { get; set; } // datetime

        /// <summary>
        /// Người tạo
        /// </summary>
        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.CreatedBy")]
        public string CreatedBy { get; set; } // varchar(100)

        /// <summary>
        /// Ngày sửa gần nhất
        /// </summary>
        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.ModifiedDate")]
        public DateTime ModifiedDate { get; set; } // timestamp

        /// <summary>
        /// Người sử gần nhất
        /// </summary>
        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.ModifiedBy")]
        public string ModifiedBy { get; set; } // varchar(100)

        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.TypeValueId")]
        public string TypeValueId { get; set; } // varchar(36)

        public string TypeValueName { get; set; } // varchar(36)

        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.SaveValue")]
        public string SaveValue { get; set; } // longtext

        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.VoucherDate")]
        public DateTime VoucherDate { get; set; } // timestamp

        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.VoucherBy")]
        public string VoucherBy { get; set; } // varchar(255)

        public IFormFile FormFile { get; set; }

        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.DataSave")]
        public byte[] DataSave { get; set; } // longblob

        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.FileName")]
        public string FileName { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.FileLength")]

        public decimal? FileLength { get; set; } // decimal(10,0)

        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.FileContent")]

        public string FileContent { get; set; } // varchar(100)

        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.FileType")]

        public string FileType { get; set; } // varchar(50)

        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.VoucherBy")]
        public string VoucherByName { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.ModifiedBy")]
        public string ModifiedByName { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.NumberHeader")]
        public int NumberHeader { get; set; } // int

        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.NameTableRefense")]
        public string NameTableRefense { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.SheeActive")]
        public string SheeActive { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.UrlTemplate")]
        public string UrlTemplate { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.NameColumn")]
        public string NameColumn { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.TableHeaderRow")]
        public string TableHeaderRow { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.TableSheetNames")]
        public string TableSheetNames { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.ActiveGetAllData")]
        public bool? ActiveGetAllData { get; set; } // bit(1)

        [XBaseResourceDisplayName("Common.Fields.StorageValueModel.OptionSelectColumn")]
        public string OptionSelectColumn { get; set; } // varchar(255)



        public IList<SelectListItem> AvailableTypeValue { get; set; }

        public IList<SelectListItem> AvailableUsers { get; set; }

        public IList<SelectListItem> AvailableNameTable
        {
            get;
            set;
        }

        public StorageValueModel()
        {
            AvailableNameTable = new List<SelectListItem>();
            AvailableTypeValue = new List<SelectListItem>();
            AvailableUsers = new List<SelectListItem>();
        }
    }

    public partial class StorageValueValidator : AbstractValidator<StorageValueModel>
    {
        public StorageValueValidator(ILocalizationService localization)
        {
            RuleFor(x => x.Name).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"), localization.GetResource("Common.Fields.StorageValueModel.Name")));

            RuleFor(x => x.TypeValueId).NotEmpty()
               .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"), localization.GetResource("Common.Fields.StorageValueModel.TypeValueId")));

            RuleFor(x => x.VoucherBy).NotEmpty()
               .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"), localization.GetResource("Common.Fields.StorageValueModel.VoucherBy")));
        }
    }
}
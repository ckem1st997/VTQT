using FluentValidation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Dashboard.Models
{
    public partial class TypeValueModel : BaseEntityModel
    {
        /// <summary>
        /// Tên loại dữ liệu
        /// </summary>
        [XBaseResourceDisplayName("Common.Fields.TypeValueModel.Name")]
        public string Name { get; set; } // varchar(100)

        /// <summary>
        /// Mô tả
        /// </summary>
        [XBaseResourceDisplayName("Common.Fields.TypeValueModel.Description")]
        public string Description { get; set; } // varchar(255)

        /// <summary>
        /// fk Id của kiểu cha
        /// </summary>
        [XBaseResourceDisplayName("Common.Fields.TypeValueModel.ParentId")]
        public string ParentId { get; set; } // varchar(36)

        /// <summary>
        /// HearachyID dạng path để hỗ trợ query dữ liệu dạng tree
        /// </summary>
        [XBaseResourceDisplayName("Common.Fields.TypeValueModel.Path")]
        public string Path { get; set; } // varchar(255)

        /// <summary>
        /// Ngừng theo dõi
        /// </summary>
        [XBaseResourceDisplayName("Common.Fields.TypeValueModel.Inactive")]
        public bool Inactive { get; set; } // bit(1)

        public  bool IsAuthorize { get; set; }

        public IList<SelectListItem> AvailableTypeValue { get; set; }

        public TypeValueModel()
        {
            IsAuthorize = false;
            Inactive = false;
            AvailableTypeValue = new List<SelectListItem>();
        }
    }

    public partial class TypeValueValidator : AbstractValidator<TypeValueModel>
    {
        public TypeValueValidator(ILocalizationService localization)
        {
            RuleFor(x => x.Name).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"), localization.GetResource("Common.Fields.TypeValueModel.Name")));
        }
    }
}
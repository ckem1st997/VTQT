using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Master.Models
{
    public partial class RoleModel : BaseEntityModel, ILocalizedMvcModel<RoleLocalizedModel>
    {
        /// <summary>
        /// Tên vai trò (SystemName)
        /// </summary>
        [XBaseResourceDisplayName("Master.Roles.Fields.Name")]
        public string Name { get; set; } // varchar(255)
        /// <summary>
        /// Tên hiển thị (đa ngôn ngữ)
        /// </summary>
        [XBaseResourceDisplayName("Common.Fields.DisplayName")]
        public string DisplayName { get; set; } // varchar(255)
        /// <summary>
        /// Mô tả (đa ngôn ngữ)
        /// </summary>
        [XBaseResourceDisplayName("Common.Fields.Description")]
        public string Description { get; set; } // longtext
        /// <summary>
        /// Kích hoạt
        /// </summary>
        [XBaseResourceDisplayName("Common.Fields.Active")]
        public bool Active { get; set; } // tinyint(1)
        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        [XBaseResourceDisplayName("Common.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; } // int

        public IList<RoleLocalizedModel> Locales { get; set; }

        public IList<SelectListItem> AvailableApps { get; set; }

        public RoleModel()
        {
            Active = true;
            DisplayOrder = 1;
            Locales = new List<RoleLocalizedModel>();
            AvailableApps = new List<SelectListItem>();
        }
    }

    public partial class RoleLocalizedModel : ILocalizedMvcLocaleModel
    {
        public string LanguageId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.DisplayName")]
        public string DisplayName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Description")]
        public string Description { get; set; }
    }

    public partial class RoleValidator : AbstractValidator<RoleModel>
    {
        public RoleValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Master.Roles.Fields.Name")));
            RuleFor(x => x.Name).SetValidator(new MaximumLengthValidator<RoleModel>(255))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Master.Roles.Fields.Name"), 255));

            RuleFor(x => x.DisplayName).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Common.Fields.DisplayName")));
            RuleFor(x => x.DisplayName).SetValidator(new MaximumLengthValidator<RoleModel>(255))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Common.Fields.DisplayName"), 255));

            RuleFor(x => x.Description).SetValidator(new MaximumLengthValidator<RoleModel>(500))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Common.Fields.Description"), 500));

            RuleFor(x => x.DisplayOrder).NotNull()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Common.Fields.DisplayOrder")));
        }
    }
}

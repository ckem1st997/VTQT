using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public partial class WareHouseItemCategoryModel : BaseEntityModel, ILocalizedMvcModel<WareHouseItemCategoryLocalizedModel>
    {
        [XBaseResourceDisplayName("Warehouse.WareHouseItemCategory.Fields.Code")]
        public string Code { get; set; }

        [XBaseResourceDisplayName("Warehouse.WareHouseItemCategory.Fields.Name")]
        public string Name { get; set; }

        [XBaseResourceDisplayName("Warehouse.WareHouseItemCategory.Fields.Parent")]
        [UIHint("DropDownList")]
        public string ParentId { get; set; }

        public string Path { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Description")]
        public string Description { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Active")]
        public bool Inactive { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }

        public IList<WareHouseItemCategoryLocalizedModel> Locales { get; set; }

        public WareHouseItemCategoryModel()
        {
            AvailableCategories = new List<SelectListItem>();

            Inactive = false;

            Locales = new List<WareHouseItemCategoryLocalizedModel>();
        }
    }

    public partial class WareHouseItemCategoryLocalizedModel : ILocalizedMvcLocaleModel
    {
        public string LanguageId { get; set; }

        public string Name { get; set; }
    }

    public partial class WareHouseItemCategoryValidator : AbstractValidator<WareHouseItemCategoryModel>
    {
        public WareHouseItemCategoryValidator(ILocalizationService localization)
        {
            RuleFor(x => x.Code).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"),
                                           localization.GetResource("Common.Fields.WareHouseItemCategory.Code")));
            RuleFor(x => x.Code).SetValidator(new MaximumLengthValidator<WareHouseItemCategoryModel>(100))
                .WithMessage(string.Format(localization.GetResource("Common.Validators.Characters.MaxLength"),
                                           localization.GetResource("Common.Fields.WareHouseItemCategory.Code"), 100));

            RuleFor(x => x.Name).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"),
                                           localization.GetResource("Common.Fields.Name")));
            RuleFor(x => x.Name).SetValidator(new MaximumLengthValidator<WareHouseItemCategoryModel>(100))
                .WithMessage(string.Format(localization.GetResource("Common.Validators.Characters.MaxLength"),
                                           localization.GetResource("Common.Fields.Name"), 100));

            RuleFor(x => x.ParentId).SetValidator(new MaximumLengthValidator<WareHouseItemCategoryModel>(36))
                .WithMessage(string.Format(localization.GetResource("Common.Validators.Characters.MaxLength"),
                                           localization.GetResource("WareHouse.WareHouseItemCategories.Fields.ParentId"), 36));

            RuleFor(x => x.Path).SetValidator(new MaximumLengthValidator<WareHouseItemCategoryModel>(255))
                .WithMessage(string.Format(localization.GetResource("Common.Validators.Characters.MaxLength"),
                                           localization.GetResource("WareHouse.WareHouseItemCategories.Fields.Path"), 255));

            RuleFor(x => x.Description).SetValidator(new MaximumLengthValidator<WareHouseItemCategoryModel>(255))
                .WithMessage(string.Format(localization.GetResource("Common.Validators.Characters.MaxLength"),
                                           localization.GetResource("Common.Fields.Description"), 255));
        }
    }
}

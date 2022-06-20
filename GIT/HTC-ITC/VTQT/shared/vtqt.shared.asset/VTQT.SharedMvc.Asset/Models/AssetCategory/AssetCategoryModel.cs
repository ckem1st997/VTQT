using FluentValidation;
using FluentValidation.Validators;
using System.Collections.Generic;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Asset.Models
{
    public partial class AssetCategoryModel : BaseEntityModel, ILocalizedMvcModel<AssetCategoryLocalizedModel>
    {
        [XBaseResourceDisplayName("Asset.AssetCategories.Fields.Code")]
        public string Code { get; set; }

        [XBaseResourceDisplayName("Asset.AssetCategories.Fields.Name")]
        public string Name { get; set; }

        [XBaseResourceDisplayName("Asset.AssetCategories.Fields.Parent")]
        public string ParentId { get; set; }

        public string Path { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Description")]
        public string Description { get; set; }

        [XBaseResourceDisplayName("Common.Fields.DepreciationDuration")]
        public int DepreciationDuration { get; set; }

        [XBaseResourceDisplayName("Common.Fields.DepreciationUnit")]
        public int DepreciationUnit { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WarrantyDuration")]
        public int WarrantyDuration { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WarrantyUnit")]
        public int WarrantyUnit { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Active")]
        public bool Inactive { get; set; }

        public string SelectedDepreciationUnit { get; set; }

        public string SelectedWarrantyUnit { get; set; }

        public IList<AssetCategoryLocalizedModel> Locales { get; set; }

        public AssetCategoryModel()
        {
            Inactive = false;

            Locales = new List<AssetCategoryLocalizedModel>();            
        }
    }

    public partial class AssetCategoryLocalizedModel : ILocalizedMvcLocaleModel
    {
        public string Name { get; set; }

        public string LanguageId { get; set; }
    }

    public partial class AssetCategoryValidator : AbstractValidator<AssetCategoryModel>
    {
        public AssetCategoryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Code).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                           localizationService.GetResource("Asset.AssetCategories.Fields.Code")));
            RuleFor(x => x.Code).SetValidator(new MaximumLengthValidator<AssetCategoryModel>(20))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("Asset.AssetCategories.Fields.Code"), 20));

            RuleFor(x => x.Name).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                           localizationService.GetResource("Asset.AssetCategories.Fields.Name")));
            RuleFor(x => x.Name).SetValidator(new MaximumLengthValidator<AssetCategoryModel>(100))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("Asset.AssetCategories.Fields.Name"), 100));

            RuleFor(x => x.Description).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                           localizationService.GetResource("Common.Fields.Description")));
            RuleFor(x => x.Description).SetValidator(new MaximumLengthValidator<AssetCategoryModel>(255))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("Common.Fields.Description"), 255));
        }
    }
}

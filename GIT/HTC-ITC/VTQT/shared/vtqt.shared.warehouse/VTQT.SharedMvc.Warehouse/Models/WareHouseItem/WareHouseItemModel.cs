using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public partial class WareHouseItemModel : BaseEntityModel, ILocalizedMvcModel<WareHouseItemLocalizedModel>
    {
        [XBaseResourceDisplayName("WareHouse.WareHouseItems.Fields.Code")]
        public string Code { get; set; }

        [XBaseResourceDisplayName("WareHouse.WareHouseItems.Fields.Name")]
        public string Name { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WareHouseItemCategory")]
        public string CategoryID { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Description")]
        public string Description { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Vendor")]
        public string VendorID { get; set; }

        [XBaseResourceDisplayName("WareHouse.Vendors.Fields.Name")]
        public string VendorName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Country")]
        public string Country { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Unit")]
        public string UnitId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Active")]
        public bool Inactive { get; set; }

        public string WareHouseItemCategoryName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WareHouseItemCategory")]
        public WareHouseItemCategoryModel WareHouseItemCategoryModel { get; set; }

        public IList<SelectListItem> AvailableWareHouseItemCategories { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Vendor")]
        public VendorModel VendorModel { get; set; }

        public IList<SelectListItem> AvailableVendors { get; set; }

        public IList<WareHouseItemLocalizedModel> Locales { get; set; }

        public IList<WareHouseItemUnitModel> WareHouseItemUnits { get; set; }

        [XBaseResourceDisplayName("Common.Unit")]
        public UnitModel UnitModel { get; set; }

        public IList<SelectListItem> AvailableUnits { get; set; }

        public WareHouseItemModel()
        {
            Inactive = false;

            Locales = new List<WareHouseItemLocalizedModel>();

            AvailableWareHouseItemCategories = new List<SelectListItem>();

            AvailableVendors = new List<SelectListItem>();

            AvailableUnits = new List<SelectListItem>();

            WareHouseItemUnits = new List<WareHouseItemUnitModel>();
        }
    }

    public partial class WareHouseItemLocalizedModel : ILocalizedMvcLocaleModel
    {
        public string LanguageId { get; set; }

        public string Name { get; set; }
    }

    public partial class WareHouseItemValidator : AbstractValidator<WareHouseItemModel>
    {
        public WareHouseItemValidator(ILocalizationService localization)
        {
            RuleFor(x => x.Name).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"),
                                           localization.GetResource("Common.Fields.Name")));
            RuleFor(x => x.Name).SetValidator(new MaximumLengthValidator<WareHouseItemModel>(100))
                .WithMessage(string.Format(localization.GetResource("Common.Validators.Characters.MaxLength"),
                                           localization.GetResource("Common.Fields.Name"), 100));

            RuleFor(x => x.Description).SetValidator(new MaximumLengthValidator<WareHouseItemModel>(255))
                .WithMessage(string.Format(localization.GetResource("Common.Validators.Characters.MaxLength"),
                                           localization.GetResource("Common.Fields.Description"), 255));

            RuleFor(x => x.Country).SetValidator(new MaximumLengthValidator<WareHouseItemModel>(255))
                .WithMessage(string.Format(localization.GetResource("Common.Validators.Characters.MaxLength"),
                                           localization.GetResource("Common.Fields.Country"), 255));

            RuleFor(x => x.UnitId).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.SelectFields.Required"),
                                           localization.GetResource("WareHouse.WareHouseItems.Fields.UnitId")));               

            RuleFor(x => x.VendorName).SetValidator(new MaximumLengthValidator<WareHouseItemModel>(255))
               .WithMessage(string.Format(localization.GetResource("Common.Validators.Characters.MaxLength"),
                                          localization.GetResource("WareHouse.WareHouseItems.Fields.VendorName"), 255));
        }
    }
}

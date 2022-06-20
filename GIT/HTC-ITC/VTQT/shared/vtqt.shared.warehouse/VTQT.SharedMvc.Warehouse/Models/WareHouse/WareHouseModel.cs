using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public partial class WareHouseModel : BaseEntityModel, ILocalizedMvcModel<WareHouseLocalizedModel>
    {
        [XBaseResourceDisplayName("Warehouse.WareHouses.Fields.Code")]
        public string Code { get; set; }

        [XBaseResourceDisplayName("Warehouse.WareHouses.Fields.Name")]
        public string Name { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Address")]
        public string Address { get; set; }

        [XBaseResourceDisplayName("Warehouse.WareHouses.Fields.Parent")]
        public string ParentId { get; set; }

        public string Path { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Description")]
        public string Description { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Active")]
        public bool Inactive { get; set; }

        [XBaseResourceDisplayName("Warehouse.WareHouses.Fields.GetListRole")]
        public string GetListRole { get; set; }

        [XBaseResourceDisplayName("Warehouse.WareHouses.Fields.SetListRole")]
        public string SetListRole { get; set; }

        public IList<WareHouseLocalizedModel> Locales { get; set; }

        public IList<SelectListItem> AvailableWareHouses { get; set; }

        public WareHouseModel()
        {
            Inactive = false;
            Locales = new List<WareHouseLocalizedModel>();
        }
    }

    public partial class WareHouseLocalizedModel : ILocalizedMvcLocaleModel
    {
        public string LanguageId { get; set; }

        public string Name { get; set; }
    }

    public partial class WareHouseValidator : AbstractValidator<WareHouseModel>
    {
        public WareHouseValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Code).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Warehouse.WareHouses.Fields.code")));
            RuleFor(x => x.Code).SetValidator(new MaximumLengthValidator<WareHouseModel>(255))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Warehouse.WareHouses.Fields.code"), 255));

            RuleFor(x => x.Name).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Warehouse.WareHouses.Fields.Name")));
            RuleFor(x => x.Name).SetValidator(new MaximumLengthValidator<WareHouseModel>(255))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Warehouse.WareHouses.Fields.Name"), 255));

            RuleFor(x => x.Address).SetValidator(new MaximumLengthValidator<WareHouseModel>(255))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Common.Fields.Address"), 255));

            RuleFor(x => x.Description).SetValidator(new MaximumLengthValidator<WareHouseModel>(255))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Common.Fields.Description"), 255));

            RuleFor(x => x.ParentId).SetValidator(new MaximumLengthValidator<WareHouseModel>(36))
                  .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                             localizationService.GetResource("WareHouse.WareHouse.Fields.ParentId"), 36));

            RuleFor(x => x.Path).SetValidator(new MaximumLengthValidator<WareHouseModel>(255))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("WareHouse.WareHouse.Fields.Path"), 255));
        }
    }
}

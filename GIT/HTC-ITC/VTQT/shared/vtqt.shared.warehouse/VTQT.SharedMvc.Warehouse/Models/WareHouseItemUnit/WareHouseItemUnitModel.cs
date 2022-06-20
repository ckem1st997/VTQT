using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public partial class WareHouseItemUnitModel : BaseEntityModel, ILocalizedMvcModel<WareHouseItemUnitLocalizedModel>
    {
        [XBaseResourceDisplayName("WareHouse.WareHouseItemUnit.Fields.ItemId")]
        public string ItemId { get; set; }

        [XBaseResourceDisplayName("WareHouse.WareHouseItemUnit.Fields.UnitId")]
        public string UnitId { get; set; }

        [XBaseResourceDisplayName("WareHouse.WareHouseItemUnit.Fields.UnitName")]
        public string UnitName { get; set; }

        [XBaseResourceDisplayName("WareHouse.WareHouseItemUnit.Fields.ConvertRate")]
        public int ConvertRate { get; set; }

        public bool IsPrimary { get; set; }

        [XBaseResourceDisplayName("WareHouse.WareHouseItemUnit.Fields.Note")]
        public  string Note { get; set; }

        public IList<WareHouseItemUnitLocalizedModel> Locales { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Unit")]
        public UnitModel UnitModel { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WareHouseItem")]
        public WareHouseItemModel WareHouseItemModel { get; set; }

        public IList<SelectListItem> AvailableUnits { get; set; }

        public WareHouseItemUnitModel()
        {
            AvailableUnits = new List<SelectListItem>();
            Locales = new List<WareHouseItemUnitLocalizedModel>();
        }
    }

    public partial class WareHouseItemUnitLocalizedModel : ILocalizedMvcLocaleModel
    {
        public string UnitName { get; set; }

        public string LanguageId { get; set; }
    }

    public partial class WareHouseItemUnitValidator : AbstractValidator<WareHouseItemUnitModel>
    {
        public WareHouseItemUnitValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.UnitName).SetValidator(new MaximumLengthValidator<WareHouseItemUnitModel>(255))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("WareHouse.WareHouseItemUnits.Fields.UnitName"), 255));

            RuleFor(x => x.ItemId).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.SelectFields.Required"),
                                           localizationService.GetResource("WareHouse.WareHouseItemUnits.Fields.ItemId")));           

            RuleFor(x => x.UnitId).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.SelectFields.Required"),
                                           localizationService.GetResource("WareHouse.WareHouseItemUnits.Fields.UnitId")));            

            RuleFor(x => x.ConvertRate).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                           localizationService.GetResource("WareHouse.WareHouseItemUnits.Fields.ConvertRate")));
        }
    }
}


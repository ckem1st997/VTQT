using FluentValidation;
using FluentValidation.Validators;
using System.Collections.Generic;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public partial class UnitModel : BaseEntityModel, ILocalizedMvcModel<UnitLocalizedModel>
    {
        [XBaseResourceDisplayName("WareHouse.Units.Fields.UnitName")]
        public string UnitName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Active")]
        public bool Inactive { get; set; }

        public IList<UnitLocalizedModel> Locales { get; set; }

        public UnitModel()
        {
            Inactive = false;

            Locales = new List<UnitLocalizedModel>();
        }
    }

    public partial class UnitLocalizedModel : ILocalizedMvcLocaleModel
    {
        public string UnitName { get; set; }

        public string LanguageId { get; set; }
    }

    public partial class UnitValidator : AbstractValidator<UnitModel>
    {
        public UnitValidator(ILocalizationService localization)
        {
            RuleFor(x => x.UnitName).SetValidator(new MaximumLengthValidator<UnitModel>(255))
                .WithMessage(string.Format(localization.GetResource("Common.Validators.Characters.MaxLenght"),
                                           localization.GetResource("WareHouse.Units.Fields.UnitName"), 255));
        }
    }
}

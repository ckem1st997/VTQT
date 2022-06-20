using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Validators;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Master.Models
{
    public partial class CurrencyModel : BaseEntityModel, ILocalizedMvcModel<CurrencyLocalizedModel>
    {
        [XBaseResourceDisplayName("Common.Fields.CurrencyName")]
        public string Name { get; set; }

        [XBaseResourceDisplayName("Common.Fields.CurrencyCode")]
        public string CurrencyCode { get; set; }

        [XBaseResourceDisplayName("Common.Fields.DisplayLocale")]
        public string DisplayLocale { get; set; }

        [XBaseResourceDisplayName("Common.Fields.CustomFormatting")]
        public string CustomFormatting { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Rate")]
        public decimal Rate { get; set; }

        [XBaseResourceDisplayName("Common.Fields.LimitedToApps")]
        public bool LimitedToApps { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Published")]
        public bool Published { get; set; }

        [XBaseResourceDisplayName("Common.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<CurrencyLocalizedModel> Locales { get; set; }

        public CurrencyModel()
        {
            Rate = 1;
            LimitedToApps = false;
            Published = true;
            DisplayOrder = 1;
            Locales = new List<CurrencyLocalizedModel>();
        }
    }

    public partial class CurrencyLocalizedModel : ILocalizedMvcLocaleModel
    {
        public string LanguageId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.CurrencyName")]
        public string Name { get; set; }
    }

    public partial class CurrencyValidator : AbstractValidator<CurrencyModel>
    {
        public CurrencyValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Common.Fields.CurrencyName")));
            RuleFor(x => x.Name).SetValidator(new MaximumLengthValidator<CurrencyModel>(50))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Common.Fields.CurrencyName"), 50));

            RuleFor(x => x.CurrencyCode).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Common.Fields.CurrencyCode")));
            RuleFor(x => x.CurrencyCode).SetValidator(new MaximumLengthValidator<CurrencyModel>(5))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Common.Fields.CurrencyCode"), 5));

            RuleFor(x => x.DisplayLocale).SetValidator(new MaximumLengthValidator<CurrencyModel>(50))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Common.Fields.DisplayLocale"), 50));

            RuleFor(x => x.CustomFormatting).SetValidator(new MaximumLengthValidator<CurrencyModel>(50))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Common.Fields.CustomFormatting"), 50));

            RuleFor(x => x.Rate).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Common.Fields.Rate")));

            RuleFor(x => x.DisplayOrder).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Common.Fields.DisplayOrder")));
        }
    }
}

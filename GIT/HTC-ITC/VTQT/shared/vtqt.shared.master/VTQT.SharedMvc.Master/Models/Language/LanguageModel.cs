using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Master.Models
{
    public partial class LanguageModel : BaseEntityModel, ILocalizedMvcModel<LanguageLocalizedModel>
    {
        [XBaseResourceDisplayName("Common.Fields.LanguageName")]
        public string Name { get; set; }

        [XBaseResourceDisplayName("Common.Fields.LanguageCulture")]
        public string LanguageCulture { get; set; }

        [XBaseResourceDisplayName("Common.Fields.UniqueSeoCode")]
        public string UniqueSeoCode { get; set; }

        [XBaseResourceDisplayName("Common.Fields.FlagImageFileName")]
        public string FlagImageFileName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Rtl")]
        public bool Rtl { get; set; }

        [XBaseResourceDisplayName("Common.Fields.LimitedToApps")]
        public bool LimitedToApps { get; set; }

        [XBaseResourceDisplayName("Common.Currency")]
        [UIHint("DropDownList")]
        public string DefaultCurrencyId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Published")]
        public bool Published { get; set; }

        [XBaseResourceDisplayName("Common.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [XBaseResourceDisplayName("Common.Currency")]
        public CurrencyModel Currency { get; set; }
        public IList<SelectListItem> AvailableCultures { get; set; }
        public IList<SelectListItem> AvailableTwoLetterLanguageCodes { get; set; }
        public IList<SelectListItem> AvailableFlags { get; set; }

        public IList<SelectListItem> AvailableCurrencies { get; set; }

        public IList<LanguageLocalizedModel> Locales { get; set; }

        public LanguageModel()
        {
            Rtl = true;
            LimitedToApps = false;
            Published = true;
            DisplayOrder = 1;
            AvailableTwoLetterLanguageCodes = new List<SelectListItem>();
            AvailableFlags = new List<SelectListItem>();
            AvailableCultures = new List<SelectListItem>();
            AvailableCurrencies = new List<SelectListItem>();
            Locales = new List<LanguageLocalizedModel>();
        }
    }

    public partial class LanguageLocalizedModel : ILocalizedMvcLocaleModel
    {
        public string LanguageId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.LanguageName")]
        public string Name { get; set; }
    }

    public partial class LanguageValidator : AbstractValidator<LanguageModel>
    {
        public LanguageValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Common.Fields.LanguageName")));
            RuleFor(x => x.Name).SetValidator(new MaximumLengthValidator<LanguageModel>(100))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Common.Fields.LanguageName"), 100));

            RuleFor(x => x.LanguageCulture).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Common.Fields.LanguageCulture")));
            RuleFor(x => x.LanguageCulture).SetValidator(new MaximumLengthValidator<LanguageModel>(20))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Common.Fields.LanguageCulture"), 20));

            RuleFor(x => x.UniqueSeoCode).SetValidator(new MaximumLengthValidator<LanguageModel>(2))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Common.Fields.UniqueSeoCode"), 2));

            RuleFor(x => x.FlagImageFileName).SetValidator(new MaximumLengthValidator<LanguageModel>(50))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Common.Fields.FlagImageFileName"), 50));

            RuleFor(x => x.DisplayOrder).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Common.Fields.DisplayOrder")));
        }
    }
}

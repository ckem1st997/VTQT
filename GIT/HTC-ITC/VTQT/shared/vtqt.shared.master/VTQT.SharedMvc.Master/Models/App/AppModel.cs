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
    public partial class AppModel : BaseEntityModel, ILocalizedMvcModel<AppLocalizedModel>
    {
        [XBaseResourceDisplayName("Common.Fields.AppType")]
        [UIHint("DropDownList")]
        public string AppType { get; set; }

        [XBaseResourceDisplayName("Common.Fields.AppName")]
        public string Name { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ShortName")]
        public string ShortName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Description")]
        public string Description { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Icon")]
        public string Icon { get; set; }

        [XBaseResourceDisplayName("Common.Fields.BackgroundColor")]
        public string BackgroundColor { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Url")]
        public string Url { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Hosts")]
        public string Hosts { get; set; }

        [XBaseResourceDisplayName("Common.Fields.SslEnabled")]
        public bool SslEnabled { get; set; }

        [XBaseResourceDisplayName("Common.Fields.CdnUrl")]
        public string CdnUrl { get; set; }

        [XBaseResourceDisplayName("Common.Language")]
        [UIHint("DropDownList")]
        public string DefaultLanguageId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ShowOnMenu")]
        public bool ShowOnMenu { get; set; }

        [XBaseResourceDisplayName("Common.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [XBaseResourceDisplayName("Common.Language")]
        public LanguageModel Language { get; set; }

        public IList<SelectListItem> AvailableAppTypes { get; set; }

        public IList<SelectListItem> AvailableLanguages { get; set; }

        public IList<AppLocalizedModel> Locales { get; set; }

        public AppModel()
        {
            SslEnabled = false;
            ShowOnMenu = true;
            DisplayOrder = 1;
            AvailableAppTypes = new List<SelectListItem>();
            AvailableLanguages = new List<SelectListItem>();
            Locales = new List<AppLocalizedModel>();
        }
    }

    public partial class AppLocalizedModel : ILocalizedMvcLocaleModel
    {
        public string LanguageId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.AppName")]
        public string Name { get; set; }
    }

    public partial class AppValidator : AbstractValidator<AppModel>
    {
        public AppValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.AppType).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.SelectFields.Required"), localizationService.GetResource("Common.Fields.AppType")));

            RuleFor(x => x.Name).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Common.Fields.AppName")));
            RuleFor(x => x.Name).SetValidator(new MaximumLengthValidator<AppModel>(255))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Common.Fields.AppName"), 255));

            RuleFor(x => x.ShortName).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Common.Fields.ShortName")));
            RuleFor(x => x.ShortName).SetValidator(new MaximumLengthValidator<AppModel>(255))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Common.Fields.ShortName"), 255));

            RuleFor(x => x.Url).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Common.Fields.Url")));
            RuleFor(x => x.Url).SetValidator(new MaximumLengthValidator<AppModel>(400))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Common.Fields.Url"), 400));

            RuleFor(x => x.Hosts).SetValidator(new MaximumLengthValidator<AppModel>(1000))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Common.Fields.Hosts"), 1000));

            RuleFor(x => x.CdnUrl).SetValidator(new MaximumLengthValidator<AppModel>(400))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Common.Fields.CdnUrl"), 400));

            RuleFor(x => x.DefaultLanguageId).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.SelectFields.Required"), localizationService.GetResource("Common.Language")));

            RuleFor(x => x.DisplayOrder).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Common.Fields.DisplayOrder")));
        }
    }
}

using System.ComponentModel.DataAnnotations;
using FluentValidation;
using FluentValidation.Validators;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Master.Models
{
    public partial class LocaleStringResourceModel : BaseEntityModel
    {
        [XBaseResourceDisplayName("Common.Fields.ResourceName")]
        public string ResourceName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ResourceValue")]
        public string ResourceValue { get; set; }

        [XBaseResourceDisplayName("Common.Language")]
        [UIHint("DropDownList")]
        public string LanguageId { get; set; }

        public LocaleStringResourceModel()
        {

        }
    }

    public partial class LocaleStringResourceValidator : AbstractValidator<LocaleStringResourceModel>
    {
        public LocaleStringResourceValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.ResourceName).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Common.Fields.ResourceName")));
            RuleFor(x => x.ResourceName).SetValidator(new MaximumLengthValidator<LocaleStringResourceModel>(200))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Common.Fields.ResourceName"), 200));

            RuleFor(x => x.ResourceValue).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Common.Fields.ResourceValue")));
        }
    }
}

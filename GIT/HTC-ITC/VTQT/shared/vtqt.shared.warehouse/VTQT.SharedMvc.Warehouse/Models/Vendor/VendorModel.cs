using FluentValidation;
using FluentValidation.Validators;
using System.Collections.Generic;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public partial class VendorModel : BaseEntityModel, ILocalizedMvcModel<VendorLocalizedModel>
    {
        [XBaseResourceDisplayName("WareHouse.Vendors.Fields.Code")]
        public string Code { get; set; }

        [XBaseResourceDisplayName("WareHouse.Vendors.Fields.Name")]
        public string Name { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Address")]
        public string Address { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Phone")]
        public string Phone { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Email")]
        public string Email { get; set; }

        [XBaseResourceDisplayName("WareHouse.Vendors.Fields.ContactPerson")]
        public string ContactPerson { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Active")]
        public bool Inactive { get; set; }

        public IList<VendorLocalizedModel> Locales { get; set; }

        public VendorModel()
        {
            Inactive = false;
            Locales = new List<VendorLocalizedModel>();
        }
    }

    public partial class VendorLocalizedModel : ILocalizedMvcLocaleModel
    {
        public string LanguageId { get; set; }

        public string Name { get; set; }
    }

    public partial class VendorValidator : AbstractValidator<VendorModel>
    {
        public VendorValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Code).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                               localizationService.GetResource("Common.Fields.Vendor.Code")));
            RuleFor(x => x.Code).SetValidator(new MaximumLengthValidator<VendorModel>(20))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("Common.Fields.Vendor.Code"), 20));

            RuleFor(x => x.Name).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                           localizationService.GetResource("Common.Fields.Name")));
            RuleFor(x => x.Name).SetValidator(new MaximumLengthValidator<VendorModel>(255))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("Common.Fields.Name"), 255));

            RuleFor(x => x.Address).SetValidator(new MaximumLengthValidator<VendorModel>(255))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("Common.Fields.Address"), 255));

            RuleFor(x => x.Phone).SetValidator(new MaximumLengthValidator<VendorModel>(20))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("WareHouse.Vendors.Fields.Phone"), 20));
            RuleFor(x => x.Phone).Matches(@"^([\+]?[84|1|7|49|44|81|82|86|886][-]?|[0])?[1-9][0-9]{8,18}$");

            RuleFor(x => x.Email).SetValidator(new MaximumLengthValidator<VendorModel>(50))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("Common.Fields.Email"), 50));
            RuleFor(x => x.Email).EmailAddress()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Objects.InValid"),
                                           localizationService.GetResource("Common.Fields.Email")));

            RuleFor(x => x.ContactPerson).SetValidator(new MaximumLengthValidator<VendorModel>(100))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("WareHouse.Vendors.Fields.ContactPerson"), 100));
        }
    }
}

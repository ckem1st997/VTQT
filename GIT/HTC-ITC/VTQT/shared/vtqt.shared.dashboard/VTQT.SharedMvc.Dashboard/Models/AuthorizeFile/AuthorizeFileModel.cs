using FluentValidation;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Dashboard.Models
{
    public class AuthorizeFileModel : BaseEntityModel
    {
        [XBaseResourceDisplayName("Common.Fields.AuthorizeFile.AuthorizedId")]
        public string AuthorizedId { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Common.Fields.AuthorizeFile.DelegatorId")]
        public string DelegatorId { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Common.Fields.AuthorizeFile.Name")]
        public string Name { get; set; } // varchar(50)

        [XBaseResourceDisplayName("Common.Fields.AuthorizeFile.NameAuthorize")]
        public string NameAuthorize { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Common.Fields.AuthorizeFile.FileId")]
        public string FileId { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Common.Fields.AuthorizeFile.NameFile")]
        public string NameFile { get; set; } // varchar(255)
    }

    public partial class AuthorizeFileValidator : AbstractValidator<AuthorizeFileModel>
    {
        public AuthorizeFileValidator(ILocalizationService localization)
        {
            RuleFor(x => x.DelegatorId).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"), localization.GetResource("Common.Fields.AuthorizeFile.DelegatorId")));

            RuleFor(x => x.AuthorizedId).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"), localization.GetResource("Common.Fields.AuthorizeFile.AuthorizedId")));

            RuleFor(x => x.Name).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"), localization.GetResource("Common.Fields.AuthorizeFile.Name")));

            RuleFor(x => x.NameAuthorize).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"), localization.GetResource("Common.Fields.AuthorizeFile.NameAuthorize")));
        }
    }
}
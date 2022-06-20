using FluentValidation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Dashboard.Models
{
    public class AuthorizeToRoleModel : BaseEntityModel
    {
        [XBaseResourceDisplayName("Common.Fields.AuthorizeToRoleModel.DelegatorId")]
        public string DelegatorId { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Common.Fields.AuthorizeToRoleModel.AuthorizedId")]
        public string AuthorizedId { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Common.Fields.AuthorizeToRoleModel.ListFileAuthorizedId")]
        public string ListFileAuthorizedId { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Common.Fields.AuthorizeToRoleModel.CreateDate")]
        public DateTime? CreateDate { get; set; } // datetime

        [XBaseResourceDisplayName("Common.Fields.AuthorizeToRoleModel.TypeValueId")]
        public string TypeValueId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.AuthorizeToRoleModel.Name")]
        public string Name { get; set; } // varchar(50)

        [XBaseResourceDisplayName("Common.Fields.AuthorizeToRoleModel.NameAuthorize")]
        public string NameAuthorize { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Common.Fields.AuthorizeToRoleModel.ListNameFile")]
        public string ListNameFile { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Common.Fields.AuthorizeToRoleModel.NameTypeValue")]
        public string NameTypeValue { get; set; }

        public IList<SelectListItem> AvailableUsers { get; set; }

        public IList<SelectListItem> AvailableTypeValues { get; set; }

        public IList<SelectListItem> AvailableFiles { get; set; }

        public AuthorizeToRoleModel()
        {
            AvailableFiles = new List<SelectListItem>();
            AvailableTypeValues = new List<SelectListItem>();
            AvailableUsers = new List<SelectListItem>();
        }
    }

    public partial class AuthorizeToRoleValidator : AbstractValidator<AuthorizeToRoleModel>
    {
        public AuthorizeToRoleValidator(ILocalizationService localization)
        {
            RuleFor(x => x.DelegatorId).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"), localization.GetResource("Common.Fields.AuthorizeToRoleModel.DelegatorId")));

            RuleFor(x => x.AuthorizedId).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"), localization.GetResource("Common.Fields.AuthorizeToRoleModel.AuthorizedId")));

            RuleFor(x => x.ListFileAuthorizedId).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"), localization.GetResource("Common.Fields.AuthorizeToRoleModel.ListFileAuthorizedId")));

            RuleFor(x => x.TypeValueId).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"), localization.GetResource("Common.Fields.AuthorizeToRoleModel.TypeValueId")));
        }
    }
}
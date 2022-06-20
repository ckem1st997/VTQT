using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public partial class AuditCouncilModel : BaseEntityModel
    {
        [XBaseResourceDisplayName("Common.Fields.AuditId")]
        public string AuditId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Audit.EmployeeId")]
        public string EmployeeId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Audit.EmployeeName")]
        public string EmployeeName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Audit.Role")]
        public string Role { get; set; }

        public AuditModel AuditModel { get; set; }

        public IList<SelectListItem> AvailableUsers { get; set; }

        public AuditCouncilModel()
        {
            AvailableUsers = new List<SelectListItem>();
        }
    }

    public partial class AuditCouncilValidator : AbstractValidator<AuditCouncilModel>
    {
        public AuditCouncilValidator(ILocalizationService localization)
        {
            RuleFor(x => x.AuditId).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.AuditId"),
                                           localization.GetResource("WareHouse.AuditCouncils.Fields.AuditId")));
            RuleFor(x => x.AuditId).SetValidator(new MaximumLengthValidator<AuditCouncilModel>(36))
                .WithMessage(string.Format(localization.GetResource("Common.Validators.Characters.MaxLength"),
                                           localization.GetResource("WareHouse.AuditCouncils.Fields.AuditId"), 36));
            RuleFor(x => x.EmployeeName).NotEmpty()
               .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"), localization.GetResource("Warehouse.WareHouses.Fields.EmployeeName")));
            RuleFor(x => x.EmployeeName).SetValidator(new MaximumLengthValidator<AuditCouncilModel>(100))
                .WithMessage(string.Format(localization.GetResource("Common.Validators.Characters.MaxLength"), localization.GetResource("Warehouse.WareHouses.Fields.EmployeeName"), 255));

            RuleFor(x => x.Role).NotEmpty()
               .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"), localization.GetResource("Warehouse.WareHouses.Fields.Role")));
            RuleFor(x => x.Role).SetValidator(new MaximumLengthValidator<AuditCouncilModel>(100))
                .WithMessage(string.Format(localization.GetResource("Common.Validators.Characters.MaxLength"), localization.GetResource("Warehouse.WareHouses.Fields.Role"), 255));
        }
    }
}

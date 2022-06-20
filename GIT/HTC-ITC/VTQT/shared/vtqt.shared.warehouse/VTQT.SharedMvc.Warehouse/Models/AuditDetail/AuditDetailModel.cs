using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public sealed partial class AuditDetailModel : BaseEntityModel
    {
        [XBaseResourceDisplayName("Common.Fields.AuditId")]
        public string AuditId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WareHouseItem")]
        public string ItemId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WareHouseItem")]
        public string ItemName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Unit")]
        public string UnitId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Unit")]
        public string UnitName { get; set; }

        [XBaseResourceDisplayName("Common.Audit.Quantity")]
        public decimal Quantity { get; set; }

        [XBaseResourceDisplayName("Common.Audit.AuditQuantity")]
        public decimal AuditQuantity { get; set; }

        [XBaseResourceDisplayName("WareHouse.AuditDetails.Fields.Conclude")]
        public string Conclude { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WareHouseItem")]
        public WareHouseItemModel WareHouseItem { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Audit")]
        public AuditModel Audit { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Serial")]
        public string Serial { get; set; }

        public IList<SelectListItem> AvailableItems { get; set; }

        public IList<SelectListItem> AvailableUsers { get; set; }

        public IList<AuditDetailSerialModel> AuditDetailSerials { get; set; }

        public IList<SelectListItem> AvailableUnits { get; set; }

        public AuditDetailModel()
        {
            AvailableItems = new List<SelectListItem>();

            AvailableUsers = new List<SelectListItem>();

            AuditDetailSerials = new List<AuditDetailSerialModel>();

            AvailableUnits = new List<SelectListItem>();
        }
    }

    public partial class AuditDetailValidator : AbstractValidator<AuditDetailModel>
    {
        public AuditDetailValidator(ILocalizationService localization)
        {
            RuleFor(x => x.AuditId).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"),
                                           localization.GetResource("WareHouse.AuditDetails.Fields.AuditId")));
            RuleFor(x => x.AuditId).SetValidator(new MaximumLengthValidator<AuditDetailModel>(36))
                .WithMessage(string.Format(localization.GetResource("Common.Validators.Characters.MaxLength"),
                                           localization.GetResource("WareHouse.AuditDetails.Fields.AuditId"), 36));

            RuleFor(x => x.ItemId).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.SelectFields.Required"), localization.GetResource("WareHouse.AuditDetails.Fields.ItemId")));


            RuleFor(x => x.Quantity).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"),
                                           localization.GetResource("WareHouse.AuditDetails.Fields.Quantity")));

            RuleFor(x => x.Conclude).SetValidator(new MaximumLengthValidator<AuditDetailModel>(255))
                .WithMessage(string.Format(localization.GetResource("Common.Validators.Characters.MaxLength"),
                                           localization.GetResource("WareHouse.AuditDetails.Fields.Conclude"), 255));
        }
    }
}

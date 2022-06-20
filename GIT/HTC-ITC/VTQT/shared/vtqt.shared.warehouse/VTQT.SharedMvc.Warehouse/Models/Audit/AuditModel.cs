using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public sealed partial class AuditModel : BaseEntityModel
    {
        [XBaseResourceDisplayName("Common.Fields.Audit.VoucherCode")]
        public string VoucherCode { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Audit.Voucher")]
        public string Voucher { get; set; }

        [XBaseResourceDisplayName("Common.Audit.VoucherDate")]
        public DateTime VoucherDate { get; set; }

        [XBaseResourceDisplayName("Common.Audit.VoucherDate")]
        public string StringVoucherDate { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WareHouse")]
        [UIHint("DropDownList")]
        public string WareHouseId { get; set; }

        [XBaseResourceDisplayName("Common.Audit.Description")]
        public string Description { get; set; }

        [XBaseResourceDisplayName("Common.Audit.CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [XBaseResourceDisplayName("Common.Audit.CreatedBy")]
        [UIHint("DropDownList")]
        public string CreatedBy { get; set; }

        [XBaseResourceDisplayName("Common.Audit.ModifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [XBaseResourceDisplayName("Common.Audit.ModifiedBy")]
        [UIHint("DropDownList")]
        public string ModifiedBy { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WareHouse")]
        public WareHouseModel WareHouse { get; set; }

        public IList<SelectListItem> AvailableAuditTimes { get; set; }

        public List<AuditDetailModel> AuditDetails { get; set; }

        public List<AuditCouncilModel> AuditCouncils { get; set; }

        public IList<SelectListItem> AvailableWareHouses { get; set; }

        public IList<SelectListItem> AvailableCreatedBy { get; set; }

        public IList<string> DeleteDetailIds { get; set; }

        public AuditModel()
        {
            AvailableAuditTimes = new List<SelectListItem>();

            AuditDetails = new List<AuditDetailModel>();

            AuditCouncils = new List<AuditCouncilModel>();

            AvailableWareHouses = new List<SelectListItem>();

            AvailableCreatedBy = new List<SelectListItem>();

            DeleteDetailIds = new List<string>();
        }
    }

    public partial class AuditValidator : AbstractValidator<AuditModel>
    {
        public AuditValidator(ILocalizationService localization)
        {
            RuleFor(x => x.VoucherCode).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"), localization.GetResource("Common.Fields.VoucherCode")));

            RuleFor(x => x.VoucherDate).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.InputFields.Required"),
                                           localization.GetResource("WareHouse.Audits.Fields.VoucherDate")));

            RuleFor(x => x.WareHouseId).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.SelectFields.Required"),
                                           localization.GetResource("WareHouse.Audits.Fields.WareHouseId")));

            RuleFor(x => x.Description).SetValidator(new MaximumLengthValidator<AuditModel>(255))
                .WithMessage(string.Format(localization.GetResource("Common.Validators.Characters.MaxLength"),
                                           localization.GetResource("Common.Fields.Description"), 255));           
        }
    }
}

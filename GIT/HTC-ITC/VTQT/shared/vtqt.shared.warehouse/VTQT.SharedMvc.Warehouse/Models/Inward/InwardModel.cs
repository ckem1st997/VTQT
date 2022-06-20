using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VTQT.Core.Domain.Warehouse.Enum;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public sealed partial class InwardModel : BaseEntityModel
    {
        [XBaseResourceDisplayName("Common.Fields.VoucherCode")]
        public string VoucherCode { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Inward.Voucher")]
        public string Voucher { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Date")]
        public DateTime VoucherDate { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WareHouse")]
        [UIHint("DropDownList")]
        public string WareHouseID { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Deliver")]
        public string Deliver { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Receiver")]
        public string Receiver { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Vendor")]
        [UIHint("DropDownList")]
        public string VendorId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Reason")]
        public InwardReason Reason { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Inward.DeliverPhone")]
        public string DeliverPhone { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Inward.DeliverAddress")]
        public string DeliverAddress { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Inward.DeliverDepartment")]
        public string DeliverDepartment { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Inward.ReceiverPhone")]
        public string ReceiverPhone { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Inward.ReceiverAddress")]
        public string ReceiverAddress { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Inward.ReceiverDepartment")]
        public string ReceiverDepartment { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ReasonDescription")]
        public string ReasonDescription { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Description")]
        public string Description { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Reference")]
        public List<Reference> References { get; set; }

        [XBaseResourceDisplayName("Common.Fields.CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [XBaseResourceDisplayName("Common.Fields.CreatedBy")]
        [UIHint("DropDownList")]
        public string CreatedBy { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ModifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ModifiedBy")]
        [UIHint("DropDownList")]
        public string ModifiedBy { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Vendor")]
        public VendorModel Vendor { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WareHouse")]
        public WareHouseModel WareHouse { get; set; }

        [XBaseResourceDisplayName("Common.Fields.AccObjectId")]

        public string AccObjectId { get; set; }

        public IList<InwardDetailModel> InwardDetails { get; set; }

        public IList<SelectListItem> AvailableReasons { get; set; }

        public IList<SelectListItem> AvailableVendors { get; set; }

        public IList<SelectListItem> AvailableWareHouses { get; set; }

        public IList<SelectListItem> AvailableCreatedBy { get; set; }
        public IList<SelectListItem> AvailableAccObject { get; set; }

        public IList<string> DeleteDetailIds { get; set; }

        public string Reference { get; set; }

        public InwardModel()
        {
            InwardDetails = new List<InwardDetailModel>();
            AvailableReasons = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
            AvailableWareHouses = new List<SelectListItem>();
            AvailableCreatedBy = new List<SelectListItem>();
            DeleteDetailIds = new List<string>();
            References = new List<Reference>();
            AvailableAccObject=new List<SelectListItem>();
        }
    }

    public partial class InwardValidator : AbstractValidator<InwardModel>
    {
        public InwardValidator(ILocalizationService ls)
        {
            RuleFor(x => x.VoucherCode).NotEmpty()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.InputFields.Required"), ls.GetResource("Common.Fields.VoucherCode")));

            RuleFor(x => x.VoucherDate).NotEmpty()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.InputFields.Required"), ls.GetResource("WareHouse.Inwards.Fields.VoucherDate")));

            RuleFor(x => x.VoucherDate).NotEmpty()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.InputFields.Required"), ls.GetResource("WareHouse.Inwards.Fields.VoucherDate")));

            RuleFor(x => x.WareHouseID).NotEmpty()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.SelectFields.Required"), ls.GetResource("WareHouse.Inwards.Fields.WareHouse")));

            RuleFor(x => x.Deliver).SetValidator(new MaximumLengthValidator<InwardModel>(255))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.Inwards.Fields.Deliver"), 255));

            RuleFor(x => x.Receiver).SetValidator(new MaximumLengthValidator<InwardModel>(255))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.Inwards.Fields.Receiver"), 255));

            RuleFor(x => x.Reason).NotNull()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.InputFields.Required"), ls.GetResource("WareHouse.Inwards.Fields.Reason")));
            RuleFor(x => x.Reason).IsInEnum()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Objects.InValid"), ls.GetResource("WareHouse.Inwards.Fields.Reason")));

            RuleFor(x => x.ReasonDescription).SetValidator(new MaximumLengthValidator<InwardModel>(255))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.Inwards.Fields.ReasonDescription"), 255));

            RuleFor(x => x.Description).SetValidator(new MaximumLengthValidator<InwardModel>(255))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("Common.Fields.Description"), 255));

            RuleFor(x => x.DeliverPhone).SetValidator(new MaximumLengthValidator<InwardModel>(255))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("Common.Fields.DeliverPhone"), 255));

            RuleFor(x => x.DeliverAddress).SetValidator(new MaximumLengthValidator<InwardModel>(500))
               .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("Common.Fields.DeliverAddress"), 500));

            RuleFor(x => x.DeliverDepartment).SetValidator(new MaximumLengthValidator<InwardModel>(255))
               .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("Common.Fields.DeliverDepartment"), 255));

            RuleFor(x => x.ReceiverPhone).SetValidator(new MaximumLengthValidator<InwardModel>(255))
              .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("Common.Fields.ReceiverPhone"), 255));

            RuleFor(x => x.ReceiverAddress).SetValidator(new MaximumLengthValidator<InwardModel>(500))
              .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("Common.Fields.ReceiverAddress"), 500));

            RuleFor(x => x.ReceiverDepartment).SetValidator(new MaximumLengthValidator<InwardModel>(255))
              .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("Common.Fields.ReceiverDepartment"), 255));
        }
    }
}
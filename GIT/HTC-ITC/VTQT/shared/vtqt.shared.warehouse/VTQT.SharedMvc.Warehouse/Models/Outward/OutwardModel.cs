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
    public partial class OutwardModel : BaseEntityModel
    {
        [XBaseResourceDisplayName("WareHouse.Outward.Fields.VoucherCode")]
        public string VoucherCode { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Inward.VoucherCodeReality")]
        public string VoucherCodeReality { get; set; }

        [XBaseResourceDisplayName("WareHouse.Outward.Fields.VoucherDate")]
        public DateTime VoucherDate { get; set; }

        [XBaseResourceDisplayName("WareHouse.Outward.Fields.WareHouseId")]
        [UIHint("DropDownList")]
        public string WareHouseID { get; set; }

        [XBaseResourceDisplayName("WareHouse.Outward.Fields.ToWareHouseId")]
        [UIHint("DropDownList")]
        public string ToWareHouseId { get; set; }

        [XBaseResourceDisplayName("WareHouse.Outward.Fields.Deliver")]
        public string Deliver { get; set; }

        [XBaseResourceDisplayName("WareHouse.Outward.Fields.Receiver")]
        public string Receiver { get; set; }

        [XBaseResourceDisplayName("WareHouse.Outward.Fields.Reason")]
        public OutwardReason Reason { get; set; }

        [XBaseResourceDisplayName("WareHouse.Outward.Fields.DeliverPhone")]
        public string DeliverPhone { get; set; }

        [XBaseResourceDisplayName("WareHouse.Outward.Fields.DeliverAddress")]
        public string DeliverAddress { get; set; }

        [XBaseResourceDisplayName("WareHouse.Outward.Fields.DeliverDepartment")]
        public string DeliverDepartment { get; set; }

        [XBaseResourceDisplayName("WareHouse.Outward.Fields.ReceiverPhone")]
        public string ReceiverPhone { get; set; }

        [XBaseResourceDisplayName("WareHouse.Outward.Fields.ReceiverAddress")]
        public  string ReceiverAddress { get; set; }

        [XBaseResourceDisplayName("WareHouse.Outward.Fields.ReceiverDepartment")]
        public string ReceiverDepartment { get; set; }


        [XBaseResourceDisplayName("WareHouse.Outward.Fields.ReasonDescription")]
        public string ReasonDescription { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Description")]
        public string Description { get; set; }

        [XBaseResourceDisplayName("common.fields.reference")]
        public List<Reference> References { get; set; }

        [XBaseResourceDisplayName("WareHouse.BeginningWareHouses.Fields.CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [XBaseResourceDisplayName("WareHouse.BeginningWareHouses.Fields.CreatedBy")]
        [UIHint("DropDownList")]
        public string CreatedBy { get; set; }

        [XBaseResourceDisplayName("WareHouse.BeginningWareHouses.Fields.ModifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [XBaseResourceDisplayName("WareHouse.BeginningWareHouses.Fields.ModifiedBy")]
        [UIHint("DropDownList")]
        public string ModifiedBy { get; set; }

        [XBaseResourceDisplayName("WareHouse.Outward.Fields.WareHouseId")]
        public WareHouseModel WareHouse { get; set; }

        [XBaseResourceDisplayName("WareHouse.Outward.Fields.ToWareHouseId")]
        public WareHouseModel ToWareHouse { get; set; }

        [XBaseResourceDisplayName("WareHouse.Outward.Fields.ReceiverCode")]
        public string ReceiverCode { get; set; }


        [XBaseResourceDisplayName("Common.Fields.AccObjectId")]

        public string AccObjectId { get; set; }


        public List<OutwardDetailModel> OutwardDetails { get; set; }

        public IList<SelectListItem> AvailableReasons { get; set; }

        public IList<SelectListItem> AvailableWareHouses { get; set; }

        public IList<SelectListItem> AvailableToWareHouses { get; set; }

        public IList<SelectListItem> AvailableCreatedBy { get; set; }

        public IList<string> DeleteDetailIds { get; set; }

        [XBaseResourceDisplayName("common.fields.reference")]
        public string Reference { get; set; }

        public IList<SelectListItem> AvailableAccObject { get; set; }


        public OutwardModel()
        {
            Deliver = "Thủ kho";
            OutwardDetails = new List<OutwardDetailModel>();
            AvailableReasons = new List<SelectListItem>();
            AvailableWareHouses = new List<SelectListItem>();
            AvailableToWareHouses = new List<SelectListItem>();
            AvailableCreatedBy = new List<SelectListItem>();
            AvailableAccObject = new List<SelectListItem>();
            DeleteDetailIds = new List<string>();
            References = new List<Reference>();
        }
    }

    public partial class OutwardValidator : AbstractValidator<OutwardModel>
    {
        public OutwardValidator(ILocalizationService ls)
        {
            RuleFor(x => x.VoucherCode).NotEmpty()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.InputFields.Required"), ls.GetResource("Common.Fields.VoucherCode")));

            RuleFor(x => x.VoucherDate).NotEmpty()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.InputFields.Required"), ls.GetResource("WareHouse.Outward.Fields.VoucherCode")));

            RuleFor(x => x.WareHouseID).NotEmpty()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.SelectFields.Required"), ls.GetResource("WareHouse.BeginningWareHouses.Fields.WareHouseId")));
            RuleFor(x => x.WareHouseID).SetValidator(new MaximumLengthValidator<OutwardModel>(36))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.BeginningWareHouses.Fields.WareHouseId"), 36));

            RuleFor(x => x.ToWareHouseId).SetValidator(new MaximumLengthValidator<OutwardModel>(36))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.Outward.Fields.ToWareHouseId"), 36));

            RuleFor(x => x.Deliver).SetValidator(new MaximumLengthValidator<OutwardModel>(255))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.Outward.Fields.Deliver"), 255));

            RuleFor(x => x.Receiver).SetValidator(new MaximumLengthValidator<OutwardModel>(255))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.Outward.Fields.Receiver"), 255));

            RuleFor(x => x.Reason).NotNull()
               .WithMessage(string.Format(ls.GetResource("Common.Validators.SelectFields.Required"), ls.GetResource("WareHouse.Outward.Fields.Reason")));
            RuleFor(x => x.Reason).IsInEnum()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Objects.InValid"), ls.GetResource("WareHouse.Outward.Fields.Reason")));

            RuleFor(x => x.ReasonDescription).SetValidator(new MaximumLengthValidator<OutwardModel>(255))
               .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.Outward.Fields.ReasonDescription"), 255));

            RuleFor(x => x.Description).SetValidator(new MaximumLengthValidator<OutwardModel>(255))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("Common.Fields.Description"), 255));

            RuleFor(x => x.DeliverPhone).SetValidator(new MaximumLengthValidator<OutwardModel>(255))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("Common.Fields.DeliverPhone"), 255));

            RuleFor(x => x.DeliverAddress).SetValidator(new MaximumLengthValidator<OutwardModel>(500))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("Common.Fields.DeliverAddress"), 500));

            RuleFor(x => x.DeliverDepartment).SetValidator(new MaximumLengthValidator<OutwardModel>(255))
               .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("Common.Fields.DeliverDepartment"), 255));

            RuleFor(x => x.ReceiverPhone).SetValidator(new MaximumLengthValidator<OutwardModel>(255))
              .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("Common.Fields.ReceiverPhone"), 255));

            RuleFor(x => x.ReceiverAddress).SetValidator(new MaximumLengthValidator<OutwardModel>(500))
              .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("Common.Fields.ReceiverAddress"), 500));

            RuleFor(x => x.ReceiverDepartment).SetValidator(new MaximumLengthValidator<OutwardModel>(255))
             .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("Common.Fields.ReceiverDepartment"), 255));
        }
    }
}

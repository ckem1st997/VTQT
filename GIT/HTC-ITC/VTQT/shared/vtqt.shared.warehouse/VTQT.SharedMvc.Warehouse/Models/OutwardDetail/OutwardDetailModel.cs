using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public partial class OutwardDetailModel : BaseEntityModel
    {
        [XBaseResourceDisplayName("WareHouse.OutwardDetail.Fields.OutwardId")]
        public string OutwardId { get; set; }

        [XBaseResourceDisplayName("WareHouse.BeginningWareHouses.Fields.ItemId")]
        public string ItemId { get; set; }

        [XBaseResourceDisplayName("WareHouse.BeginningWareHouses.Fields.ItemId")]
        public string ItemName { get; set; }

        [XBaseResourceDisplayName("WareHouse.OutwardDetail.Fields.UnitId")]
        public string UnitId { get; set; }

        [XBaseResourceDisplayName("WareHouse.OutwardDetail.Fields.UnitId")]
        public string UnitName { get; set; }

        [XBaseResourceDisplayName("WareHouse.OutwardDetail.Fields.UIQuantity")]
        public decimal UIQuantity { get; set; }

        [XBaseResourceDisplayName("WareHouse.OutwardDetail.Fields.UIPrice")]
        public decimal UIPrice { get; set; }

        [XBaseResourceDisplayName("WareHouse.OutwardDetail.Fields.Amount")]
        public decimal Amount { get; set; }

        [XBaseResourceDisplayName("WareHouse.OutwardDetail.Fields.Quantity")]
        public decimal Quantity { get; set; }

        [XBaseResourceDisplayName("WareHouse.OutwardDetail.Fields.Price")]
        public decimal Price { get; set; }

        [XBaseResourceDisplayName("WareHouse.OutwardDetail.Fields.DepartmentId")]
        public string DepartmentId { get; set; }

        [XBaseResourceDisplayName("WareHouse.OutwardDetail.Fields.DepartmentName")]
        public string DepartmentName { get; set; }

        [XBaseResourceDisplayName("WareHouse.OutwardDetail.Fields.EmployeeId")]
        public string EmployeeId { get; set; }

        [XBaseResourceDisplayName("WareHouse.OutwardDetail.Fields.EmployeeName")]
        public string EmployeeName { get; set; }

        [XBaseResourceDisplayName("WareHouse.OutwardDetail.Fields.StationId")]
        public string StationId { get; set; }

        [XBaseResourceDisplayName("WareHouse.OutwardDetail.Fields.StationName")]
        public string StationName { get; set; }

        [XBaseResourceDisplayName("WareHouse.OutwardDetail.Fields.ProjectId")]
        public string ProjectId { get; set; }

        [XBaseResourceDisplayName("WareHouse.OutwardDetail.Fields.ProjectName")]
        public string ProjectName { get; set; }

        [XBaseResourceDisplayName("WareHouse.OutwardDetail.Fields.CustomerId")]
        public string CustomerId { get; set; }

        [XBaseResourceDisplayName("WareHouse.OutwardDetail.Fields.CustomerName")]
        public string CustomerName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Outward")]
        public OutwardModel Outward { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WareHouseItem")]
        public WareHouseItemModel WareHouseItem { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Unit")]
        public UnitModel Unit { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Serial")]
        public string Serial { get; set; }

        [XBaseResourceDisplayName("Common.Fields.AccountMore")]
        public  string AccountMore { get; set; }

        [XBaseResourceDisplayName("Common.Fields.AccountYes")]
        public  string AccountYes { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Status")]
        public string Status { get; set; }


        [XBaseResourceDisplayName("Common.Fields.AccObjectId")]
        public string AccObjectId { get; set; }


        public IList<SelectListItem> AvailableAccountMores { get; set; }

        public IList<SelectListItem> AvailableAccountYes { get; set; }

        public IList<SerialWareHouseModel> SerialWareHouses { get; set; }

        public IList<SelectListItem> AvailableItems { get; set; }

        public IList<SelectListItem> AvailableUnits { get; set; }

        public IList<SelectListItem> AvailableOrganizations { get; set; }

        public IList<SelectListItem> AvailableUsers { get; set; }

        public IList<SelectListItem> AvailableStations { get; set; }

        public IList<SelectListItem> AvailableProjects { get; set; }

        public IList<SelectListItem> AvailableCustomers { get; set; }

        public IList<SelectListItem> AvailableAccObject { get; set; }


        public OutwardDetailModel()
        {
            SerialWareHouses = new List<SerialWareHouseModel>();
            AvailableItems = new List<SelectListItem>();
            AvailableUnits = new List<SelectListItem>();
            AvailableOrganizations = new List<SelectListItem>();
            AvailableUsers = new List<SelectListItem>();
            AvailableStations = new List<SelectListItem>();
            AvailableProjects = new List<SelectListItem>();
            AvailableCustomers = new List<SelectListItem>();
            AvailableAccountMores = new List<SelectListItem>();
            AvailableAccountYes = new List<SelectListItem>();
            AvailableAccObject = new List<SelectListItem>();
        }
    }

    public partial class OutwardDetailValidator : AbstractValidator<OutwardDetailModel>
    {
        public OutwardDetailValidator(ILocalizationService ls)
        {
            RuleFor(x => x.OutwardId).NotEmpty()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.SelectFields.Required"), ls.GetResource("WareHouse.OutwardDetail.Fields.OutwardId")));
            RuleFor(x => x.OutwardId).SetValidator(new MaximumLengthValidator<OutwardDetailModel>(36))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.OutwardDetail.Fields.OutwardId"), 36));

            RuleFor(x => x.ItemId).NotEmpty()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.SelectFields.Required"), ls.GetResource("WareHouse.BeginningWareHouses.Fields.ItemId")))
                .SetValidator(new MaximumLengthValidator<OutwardDetailModel>(36))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.BeginningWareHouses.Fields.ItemId"), 36));

            RuleFor(x => x.UnitId).NotEmpty()
               .WithMessage(string.Format(ls.GetResource("Common.Validators.SelectFields.Required"), ls.GetResource("WareHouse.OutwardDetail.Fields.UnitId")))
               .SetValidator(new MaximumLengthValidator<OutwardDetailModel>(36))
               .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.OutwardDetail.Fields.UnitId"), 36));

            RuleFor(x => x.UIQuantity).NotNull()
               .WithMessage(string.Format(ls.GetResource("Common.Validators.InputFields.Required"), ls.GetResource("WareHouse.OutwardDetail.Fields.UIQuantity")))
               .GreaterThanOrEqualTo(1)
               .WithMessage(string.Format(ls.GetResource("Common.Validators.Objects.GreaterThan"), ls.GetResource("WareHouse.OutwardDetail.Fields.UIQuantity"), 0));

            RuleFor(x => x.UIPrice).NotNull()
              .WithMessage(string.Format(ls.GetResource("Common.Validators.InputFields.Required"), ls.GetResource("WareHouse.OutwardDetail.Fields.UIPrice")))
              .GreaterThanOrEqualTo(0)
              .WithMessage(string.Format(ls.GetResource("Common.Validators.Objects.GreaterThanOrEqualTo"), ls.GetResource("WareHouse.OutwardDetail.Fields.UIPrice"), 0));

            RuleFor(x => x.Amount).NotNull()
              .WithMessage(string.Format(ls.GetResource("Common.Validators.InputFields.Required"), ls.GetResource("WareHouse.OutwardDetail.Fields.Amount")))
              .GreaterThanOrEqualTo(0)
              .WithMessage(string.Format(ls.GetResource("Common.Validators.Objects.GreaterThanOrEqualTo"), ls.GetResource("WareHouse.OutwardDetail.Fields.Amount"), 0));

            RuleFor(x => x.Quantity).NotNull()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.InputFields.Required"), ls.GetResource("WareHouse.OutwardDetail.Fields.Quantity")));
            RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0)
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Objects.GreaterThanOrEqualTo"), ls.GetResource("WareHouse.OutwardDetail.Fields.Quantity"), 0));

            RuleFor(x => x.Price).NotNull()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.InputFields.Required"), ls.GetResource("WareHouse.OutwardDetail.Fields.Price")));
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0)
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Objects.GreaterThanOrEqualTo"), ls.GetResource("WareHouse.OutwardDetail.Fields.Price"), 0));

            RuleFor(x => x.DepartmentId).SetValidator(new MaximumLengthValidator<OutwardDetailModel>(36))
               .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.OutwardDetail.Fields.DepartmentId"), 36));
            RuleFor(x => x.DepartmentName).SetValidator(new MaximumLengthValidator<OutwardDetailModel>(255))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.OutwardDetail.Fields.DepartmentName"), 255));

            RuleFor(x => x.EmployeeId).SetValidator(new MaximumLengthValidator<OutwardDetailModel>(36))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.OutwardDetail.Fields.EmployeeId"), 36));
            RuleFor(x => x.EmployeeName).SetValidator(new MaximumLengthValidator<OutwardDetailModel>(255))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.OutwardDetail.Fields.EmployeeName"), 255));

            RuleFor(x => x.StationId).SetValidator(new MaximumLengthValidator<OutwardDetailModel>(36))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.OutwardDetail.Fields.StationId"), 36));
            RuleFor(x => x.StationName).SetValidator(new MaximumLengthValidator<OutwardDetailModel>(255))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.OutwardDetail.Fields.StationName"), 255));

            RuleFor(x => x.ProjectId).SetValidator(new MaximumLengthValidator<OutwardDetailModel>(36))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.OutwardDetail.Fields.ProjectId"), 36));
            RuleFor(x => x.ProjectName).SetValidator(new MaximumLengthValidator<OutwardDetailModel>(255))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.OutwardDetail.Fields.ProjectName"), 255));

            RuleFor(x => x.CustomerId).SetValidator(new MaximumLengthValidator<OutwardDetailModel>(36))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.OutwardDetail.Fields.CustomerId"), 36));
            RuleFor(x => x.CustomerName).SetValidator(new MaximumLengthValidator<OutwardDetailModel>(255))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.OutwardDetail.Fields.CustomerName"), 255));

            RuleFor(x => x.AccountMore).NotEmpty()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.SelectFields.Required"), ls.GetResource("WareHouse.OutwardDetail.Fields.AccountMore")));

            RuleFor(x => x.AccountYes).NotEmpty()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.SelectFields.Required"), ls.GetResource("WareHouse.OutwardDetail.Fields.AccountYes")));
        }
    }
}

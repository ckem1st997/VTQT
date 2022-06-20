using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public sealed partial class InwardDetailModel : BaseEntityModel
    {
        [XBaseResourceDisplayName("Common.Fields.Inward")]
        public string InwardId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WareHouseItem")]
        public string ItemId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WareHouseItem")]
        public string ItemName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Unit")]
        public string UnitId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Unit")]
        public string UnitName { get; set; }

        [XBaseResourceDisplayName("WareHouse.InwardDetails.Fields.UIQuantity")]
        public decimal UIQuantity { get; set; }

        [XBaseResourceDisplayName("WareHouse.InwardDetails.Fields.UIPrice")]
        public decimal UIPrice { get; set; }

        [XBaseResourceDisplayName("WareHouse.InwardDetails.Fields.Amount")]
        [Column(TypeName = "decimal(18, 0)")]
        public decimal Amount { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Quantity")]
        public decimal Quantity { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Price")]
        public decimal Price { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Department")]
        public string DepartmentId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.DepartmentName")]
        public string DepartmentName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Employee")]
        public string EmployeeId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.EmployeeName")]
        public string EmployeeName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Station")]
        public string StationId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.StationName")]
        public string StationName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Project")]
        public string ProjectId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ProjectName")]
        public string ProjectName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Customer")]
        public string CustomerId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.CustomerName")]
        public string CustomerName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Serial")]
        public string Serial { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Inward")]
        public InwardModel Inward { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WareHouseItem")]
        public WareHouseItemModel WareHouseItem { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Unit")]
        public UnitModel Unit { get; set; }

        [XBaseResourceDisplayName("Common.Fields.AccountMore")]
        public string AccountMore { get; set; }

        [XBaseResourceDisplayName("Common.Fields.AccountYes")]
        public string AccountYes { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Status")]
        public  string Status { get; set; }


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


        public InwardDetailModel()
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

    public partial class InwardDetailValidator : AbstractValidator<InwardDetailModel>
    {
        public InwardDetailValidator(ILocalizationService ls)
        {
            RuleFor(x => x.InwardId).NotEmpty()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.SelectFields.Required"), ls.GetResource("WareHouse.InwardDetails.Fields.InwardId")));

            RuleFor(x => x.ItemId).NotEmpty()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.SelectFields.Required"), ls.GetResource("WareHouse.InwardDetails.Fields.ItemId")));

            RuleFor(x => x.UnitId).NotEmpty()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.SelectFields.Required"), ls.GetResource("WareHouse.InwardDetails.Fields.UnitId")));

            RuleFor(x => x.UIQuantity).NotNull()
                           .WithMessage(string.Format(ls.GetResource("Common.Validators.InputFields.Required"), ls.GetResource("WareHouse.OutwardDetail.Fields.UIQuantity")))
                           .GreaterThanOrEqualTo(1)
                           .WithMessage(string.Format(ls.GetResource("Common.Validators.Objects.GreaterThan"), ls.GetResource("WareHouse.OutwardDetail.Fields.UIQuantity"), 0));

            RuleFor(x => x.Quantity).NotNull()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.InputFields.Required"), ls.GetResource("WareHouse.InwardDetails.Fields.Quantity")));
            RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0)
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Objects.GreaterThanOrEqualTo"), ls.GetResource("WareHouse.InwardDetails.Fields.Quantity"), 0));

            RuleFor(x => x.Price).NotNull()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.InputFields.Required"), ls.GetResource("WareHouse.InwardDetails.Fields.Price")));
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0)
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Objects.GreaterThanOrEqualTo"), ls.GetResource("WareHouse.InwardDetails.Fields.Price"), 0));

            RuleFor(x => x.DepartmentName).SetValidator(new MaximumLengthValidator<InwardDetailModel>(255))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.InwardDetails.Fields.DepartmentName"), 255));           

            RuleFor(x => x.EmployeeName).SetValidator(new MaximumLengthValidator<InwardDetailModel>(255))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.InwardDetails.Fields.EmployeeName"), 255));

            RuleFor(x => x.StationName).SetValidator(new MaximumLengthValidator<InwardDetailModel>(255))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.InwardDetails.Fields.StationName"), 255));           

            RuleFor(x => x.ProjectName).SetValidator(new MaximumLengthValidator<InwardDetailModel>(255))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.InwardDetails.Fields.ProjectName"), 255));        

            RuleFor(x => x.CustomerName).SetValidator(new MaximumLengthValidator<InwardDetailModel>(255))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"), ls.GetResource("WareHouse.InwardDetails.Fields.CustomerName"), 255));

            RuleFor(x => x.AccountMore).NotEmpty()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.SelectFields.Required"), ls.GetResource("WareHouse.InwardDetails.Fields.AccountMore")));

            RuleFor(x => x.AccountYes).NotEmpty()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.SelectFields.Required"), ls.GetResource("WareHouse.InwardDetails.Fields.AccountYes")));
        }
    }
}

using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Asset.Models
{
    public partial class AssetModel : BaseEntityModel, ILocalizedMvcModel<AssetLocalizedModel>
    {
        [XBaseResourceDisplayName("Asset.Assets.Fields.Code")]
        public string Code { get; set; }

        [XBaseResourceDisplayName("Asset.Assets.Fields.Name")]
        public string Name { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Status")]
        public int Status { get; set; }

        [XBaseResourceDisplayName("Common.Fields.AssetCategory")]
        public string CategoryId { get; set; }

        [XBaseResourceDisplayName("WareHouse.WareHouseItems.Fields.Code")]
        public string WareHouseItemCode { get; set; }

        [XBaseResourceDisplayName("WareHouse.WareHouseItems.Fields.Name")]
        public string WareHouseItemName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.OriginQuantity")]
        public int OriginQuantity { get; set; }

        [XBaseResourceDisplayName("Common.Fields.RecallQuantity")]
        public int RecallQuantity { get; set; }

        [XBaseResourceDisplayName("Common.Fields.BrokenQuantity")]
        public int BrokenQuantity { get; set; }

        [XBaseResourceDisplayName("Common.Fields.SoldQuantity")]
        public int SoldQuantity { get; set; }

        [XBaseResourceDisplayName("Common.Fields.OriginStatus")]
        public string OriginUsageStatus { get; set; }

        [XBaseResourceDisplayName("Common.Fields.OriginValue")]
        public decimal OriginValue { get; set; }

        [XBaseResourceDisplayName("Common.Fields.CurrentStatus")]
        public string CurrentUsageStatus { get; set; }

        [XBaseResourceDisplayName("Common.Fields.CurrentValue")]
        public decimal CurrentValue { get; set; }

        [XBaseResourceDisplayName("Common.Fields.DepreciationDuration")]
        public int DepreciationDuration { get; set; }

        [XBaseResourceDisplayName("Common.Fields.DepreciationUnit")]
        public int DepreciationUnit { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WarrantyDuration")]
        public int WarrantyDuration { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WarrantyUnit")]
        public int WarrantyUnit { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WarrantyCondition")]
        public string WarrantyCondition { get; set; }

        [XBaseResourceDisplayName("WareHouse.Vendors.Fields.Name")]
        public string VendorName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Country")]
        public string Country { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ManufactureYear")]
        public int ManufactureYear { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Description")]
        public string Description { get; set; }

        [XBaseResourceDisplayName("Asset.Assets.Fields.AssetType")]
        public int AssetType { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Department")]
        public string OrganizationUnitId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.DepartmentName")]
        public string OrganizationUnitName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Employee")]
        public string EmployeeId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.EmployeeName")]
        public string EmployeeName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Station")]
        public string StationCode { get; set; }

        [XBaseResourceDisplayName("Common.Fields.StationName")]
        public string StationName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Project")]
        public string ProjectCode { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ProjectName")]
        public string ProjectName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Customer")]
        public string CustomerCode { get; set; }

        [XBaseResourceDisplayName("Common.Fields.CustomerName")]
        public string CustomerName { get; set; }

        [XBaseResourceDisplayName("Asset.Assets.Fields.CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [XBaseResourceDisplayName("Common.Fields.CreatedBy")]
        public string CreatedBy { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ModifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ModifiedBy")]
        public string ModifiedBy { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Reference")]
        public IList<Reference> Reference { get; set; }

        [XBaseResourceDisplayName("Common.Fields.DepreciationUnit")]
        public string SelectedDepreciationUnit { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WarrantyUnit")]
        public string SelectedWarrantyUnit { get; set; }

        [XBaseResourceDisplayName("Common.Fields.MantainDate")]
        public DateTime? MantainDate { get; set; }

        [XBaseResourceDisplayName("Common.Fields.BalanceQuantity")]
        public int BalanceQuantity { get; set; }

        [XBaseResourceDisplayName("Common.Fields.StationCategory")]
        public string StationCategory { get; set; }

        [XBaseResourceDisplayName("Common.Fields.StationArea")]
        public string StationArea { get; set; }

        [XBaseResourceDisplayName("Common.Fields.StationAddress")]
        public string StationAddress { get; set; }

        [XBaseResourceDisplayName("Common.Fields.StationLongitude")]
        public double StationLongitude { get; set; }

        [XBaseResourceDisplayName("Common.Fields.StationLatitude")]
        public double StationLatitude { get; set; }

        [XBaseResourceDisplayName("Common.Fields.CustomerAddress")]
        public string CustomerAddress { get; set; }

        public IList<SelectListItem> AvailableDurations { get; set; }

        public IList<SelectListItem> AvailableAssetStatus { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }

        public IList<SelectListItem> AvailableItems { get; set; }

        public IList<SelectListItem> AvailableCustomers { get; set; }

        public IList<SelectListItem> AvailableProjects { get; set; }

        public IList<SelectListItem> AvailableStations { get; set; }

        public IList<SelectListItem> AvailableOrganizations { get; set; }

        public IList<SelectListItem> AvailableUsers { get; set; }

        public IList<AssetLocalizedModel> Locales { get; set; }

        public AssetModel()
        {
            Locales = new List<AssetLocalizedModel>();

            Reference = new List<Reference>();

            AvailableAssetStatus = new List<SelectListItem>();

            AvailableCategories = new List<SelectListItem>();

            AvailableCustomers = new List<SelectListItem>();

            AvailableDurations = new List<SelectListItem>();

            AvailableItems = new List<SelectListItem>();

            AvailableProjects = new List<SelectListItem>();

            AvailableStations = new List<SelectListItem>();

            AvailableOrganizations = new List<SelectListItem>();

            AvailableUsers = new List<SelectListItem>();
        }
    }

    public partial class AssetLocalizedModel : ILocalizedMvcLocaleModel
    {
        public string Name { get; set; }

        public string LanguageId { get; set; }
    }

    public partial class AssetValidator : AbstractValidator<AssetModel>
    {
        public AssetValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                           localizationService.GetResource("Asset.Assets.Fields.Name")));
            RuleFor(x => x.Name).SetValidator(new MaximumLengthValidator<AssetModel>(100))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("Asset.Assets.Fields.Name"), 100));

            RuleFor(x => x.Description).SetValidator(new MaximumLengthValidator<AssetModel>(255))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("Common.Fields.Description"), 255));

            RuleFor(x => x.WarrantyCondition).SetValidator(new MaximumLengthValidator<AssetModel>(150))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("Common.Fields.WarrantyCondition"), 150));

            RuleFor(x => x.VendorName).SetValidator(new MaximumLengthValidator<AssetModel>(255))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("WareHouse.Vendors.Fields.Name"), 255));

            RuleFor(x => x.Country).SetValidator(new MaximumLengthValidator<AssetModel>(255))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("Common.Fields.Country"), 255));

            RuleFor(x => x.OrganizationUnitName).SetValidator(new MaximumLengthValidator<AssetModel>(100))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("Common.Fields.DepartmentName"), 100));

            RuleFor(x => x.EmployeeName).SetValidator(new MaximumLengthValidator<AssetModel>(100))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("Common.Fields.EmployeeName"), 100));

            RuleFor(x => x.StationName).SetValidator(new MaximumLengthValidator<AssetModel>(100))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("Common.Fields.StationName"), 100));

            RuleFor(x => x.ProjectCode).SetValidator(new MaximumLengthValidator<AssetModel>(50))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("Common.Fields.Project"), 50));

            RuleFor(x => x.ProjectName).SetValidator(new MaximumLengthValidator<AssetModel>(100))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("Common.Fields.ProjectName"), 100));

            RuleFor(x => x.CustomerCode).SetValidator(new MaximumLengthValidator<AssetModel>(50))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("Common.Fields.Customer"), 50));

            RuleFor(x => x.CustomerName).SetValidator(new MaximumLengthValidator<AssetModel>(100))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("Common.Fields.CustomerName"), 100));

            RuleFor(x => x.DepreciationDuration).GreaterThan(0)
               .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                          localizationService.GetResource("Common.Fields.DepreciationDuration")));

            RuleFor(x => x.WarrantyDuration).GreaterThan(0)
               .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                          localizationService.GetResource("Common.Fields.WarrantyDuration")));

            RuleFor(x => x.OriginValue).GreaterThan(0)
               .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                          localizationService.GetResource("Common.Fields.OriginValue")));

            RuleFor(x => x.CurrentValue).GreaterThan(0)
               .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                          localizationService.GetResource("Common.Fields.CurrentValue")));

            RuleFor(x => x.OriginQuantity).GreaterThan(0)
               .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                          localizationService.GetResource("Common.Fields.OriginQuantity")));

            RuleFor(x => x.ManufactureYear).GreaterThan(0)
               .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                          localizationService.GetResource("Common.Fields.ManufactureYear")));

            RuleFor(x => x.SelectedDepreciationUnit).NotEmpty()
               .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                          localizationService.GetResource("Common.Fields.DepreciationUnit")));

            RuleFor(x => x.SelectedWarrantyUnit).NotEmpty()
               .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                          localizationService.GetResource("Common.Fields.WarrantyUnit")));
        }
    }
}

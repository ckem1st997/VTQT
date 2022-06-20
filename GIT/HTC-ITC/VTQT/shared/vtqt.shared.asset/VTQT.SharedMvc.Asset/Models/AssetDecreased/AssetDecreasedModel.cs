using FluentValidation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Asset.Models
{
    public partial class AssetDecreasedModel:BaseEntityModel
    {
        [XBaseResourceDisplayName("Common.Fields.AssetId")]
        public string AssetId { get; set; }

        [XBaseResourceDisplayName("Asset.AssetDecreased.Fields.Quantity")]
        public int Quantity { get; set; }

        [XBaseResourceDisplayName("Asset.AssetDecreased.Fields.DecreaseDate")]
        public DateTime DecreaseDate { get; set; }

        [XBaseResourceDisplayName("Asset.AssetDecreased.Fields.DecreaseReason")]
        public string DecreaseReason { get; set; }

        [XBaseResourceDisplayName("Asset.AssetDecreased.Fields.Employee")]
        public string EmployeeId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.EmployeeName")]
        public string EmployeeName { get; set; }

        [XBaseResourceDisplayName("Asset.AssetDecreased.Fields.ToWareHouse")]
        public string WareHouseCode { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WareHouse")]
        public string WareHouseName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Note")]
        public string Notes { get; set; }

        [XBaseResourceDisplayName("Common.Fields.CreatedBy")]
        public string CreatedBy { get; set; }

        [XBaseResourceDisplayName("Common.Fields.CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ModifiedBy")]
        public string ModifiedBy { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ModifiedDate")]
        public DateTime ModifiedDate { get; set; }

        public IList<SelectListItem> AvailableReasons { get; set; }

        public IList<SelectListItem> AvailableAssets { get; set; }

        public IList<SelectListItem> AvailableUsers { get; set; }

        public IList<SelectListItem> AvailableWarehouses { get; set; }

        public AssetDecreasedModel()
        {
            Quantity = 1;
            AvailableReasons = new List<SelectListItem>();
            AvailableAssets = new List<SelectListItem>();
            AvailableUsers = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
        }
    }

    public partial class AssetDecreasedValidation : AbstractValidator<AssetDecreasedModel>
    {
        public AssetDecreasedValidation(ILocalizationService localizationService)
        {
            RuleFor(x => x.AssetId).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Common.Fields.AssetId")));

            RuleFor(x => x.DecreaseDate).Must(d => d != default)
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Asset.AssetDecreased.Fields.DecreaseDate")));                   
        }
    }
}

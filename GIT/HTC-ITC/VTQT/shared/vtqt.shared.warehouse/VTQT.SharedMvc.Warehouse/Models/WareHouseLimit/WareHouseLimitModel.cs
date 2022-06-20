using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public partial class WareHouseLimitModel : BaseEntityModel
    {
        [XBaseResourceDisplayName("Common.Warehouse")]
        public string WareHouseId { get; set; }

        [XBaseResourceDisplayName("Common.WarehouseItem")]
        public string ItemId { get; set; }

        [XBaseResourceDisplayName("Common.Unit")]
        public string UnitId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.UnitName")]
        public string UnitName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.MinQuantity")]
        public decimal MinQuantity { get; set; }

        [XBaseResourceDisplayName("Common.Fields.MaxQuantity")]
        public decimal MaxQuantity { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        [XBaseResourceDisplayName("WareHouse.WarehouseBalance.Fields.Quantity")]


        public dynamic Quantity { get; set; }


        [XBaseResourceDisplayName("Common.Unit")]
        public UnitModel UnitModel { get; set; }

        [XBaseResourceDisplayName("Common.WareHouse")]
        public WareHouseModel WareHouseModel { get; set; }

        [XBaseResourceDisplayName("Common.WareHouseItem")]
        public WareHouseItemModel WareHouseItemModel { get; set; }

        public string WareHouseItemName { get; set; }

    }

    public partial class WareHouseLimitValidator : AbstractValidator<WareHouseLimitModel>
    {
        public WareHouseLimitValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.WareHouseId).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.SelectFields.Required"),
                                           localizationService.GetResource("Common.WareHouse")));

            RuleFor(x => x.ItemId).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.SelectFields.Required"),
                                           localizationService.GetResource("Common.WarehouseItem")));

            RuleFor(x => x.UnitId).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.SelectFields.Required"),
                                           localizationService.GetResource("Common.Unit")));

            RuleFor(x => x.MinQuantity).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                           localizationService.GetResource("Common.Fields.MinQuantity")));

            RuleFor(x => x.MaxQuantity).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                           localizationService.GetResource("Common.Fields.MaxQuantity")));

            RuleFor(x => x.MaxQuantity).GreaterThanOrEqualTo(x => x.MinQuantity)
                .WithMessage(string.Format(
                    localizationService.GetResource("Common.Validators.Objects.GreaterThanOrEqualTo"),
                    localizationService.GetResource("Common.Fields.MaxQuantity"),
                    localizationService.GetResource("Common.Fields.MinQuantity")));

        }
    }
}

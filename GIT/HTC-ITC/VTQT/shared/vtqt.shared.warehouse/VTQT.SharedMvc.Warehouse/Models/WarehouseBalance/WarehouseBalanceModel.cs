using FluentValidation;
using FluentValidation.Validators;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public partial class WarehouseBalanceModel : BaseEntityModel
    {
        [XBaseResourceDisplayName("WareHouse.BeginningWareHouses.Fields.ItemId")]

        public string ItemId { get; set; }
        [XBaseResourceDisplayName("WareHouse.BeginningWareHouses.Fields.WareHouseId")]


        public string WarehouseId { get; set; }
        [XBaseResourceDisplayName("WareHouse.WarehouseBalance.Fields.Quantity")]


        public int Quantity { get; set; }

        [XBaseResourceDisplayName("WareHouse.WarehouseBalance.Fields.UIQuantity")]


        public dynamic UIQuantity { get; set; }

        [XBaseResourceDisplayName("WareHouse.WarehouseBalance.Fields.Amount")]


        public decimal Amount { get; set; }

        [XBaseResourceDisplayName("Common.WareHouse")]
        public WareHouseModel WareHouseModel { get; set; }
    }


    public partial class WarehouseBalanceValidator : AbstractValidator<WarehouseBalanceModel>
    {
        public WarehouseBalanceValidator(ILocalizationService localizationService)
        {
         
            RuleFor(x => x.ItemId).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.SelectFields.Required"),
                                           localizationService.GetResource("WareHouse.BeginningWareHouses.Fields.ItemId")));
            RuleFor(x => x.ItemId).SetValidator(new MaximumLengthValidator<WarehouseBalanceModel>(36))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("WareHouse.BeginningWareHouses.Fields.ItemId"), 36));

            RuleFor(x => x.WarehouseId).NotEmpty()
                 .WithMessage(string.Format(localizationService.GetResource("Common.Validators.SelectFields.Required"),
                                            localizationService.GetResource("WareHouse.BeginningWareHouses.Fields.WareHouseId")));
            RuleFor(x => x.WarehouseId).SetValidator(new MaximumLengthValidator<WarehouseBalanceModel>(36))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("WareHouse.BeginningWareHouses.Fields.WareHouseId"), 36));

            RuleFor(x => x.Quantity).NotEmpty()
                 .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                            localizationService.GetResource("WareHouse.WarehouseBalance.Fields.Quantity")));
            RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0)
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Number.MinLenght"),
                                           localizationService.GetResource("WareHouse.WarehouseBalance.Fields.Quantity"), 0));


            RuleFor(x => x.Amount).NotEmpty()
                  .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                             localizationService.GetResource("WareHouse.WarehouseBalance.Fields.Amount")));
            RuleFor(x => x.Amount).GreaterThanOrEqualTo(0)
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Number.MinLenght"),
                                           localizationService.GetResource("WareHouse.WarehouseBalance.Fields.Amount"), 0));

        }
    }

}

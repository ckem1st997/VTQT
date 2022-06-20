using FluentValidation;
using FluentValidation.Validators;
using System;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public partial class BeginningWareHouseModel : BaseEntityModel
    {
        [XBaseResourceDisplayName("WareHouse.BeginningWareHouses.Fields.WareHouseId")]
        public string WareHouseId { get; set; }

        [XBaseResourceDisplayName("WareHouse.BeginningWareHouses.Fields.ItemId")]
        public string ItemId { get; set; }

        [XBaseResourceDisplayName("WareHouse.BeginningWareHouses.Fields.UnitId")]
        public string UnitId { get; set; }         
        
        [XBaseResourceDisplayName("WareHouse.BeginningWareHouses.Fields.UnitName")]
        public string UnitName { get; set; }

        [XBaseResourceDisplayName("WareHouse.BeginningWareHouses.Fields.Quantity")]
        public decimal Quantity { get; set; }

        [XBaseResourceDisplayName("WareHouse.BeginningWareHouses.Fields.CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [XBaseResourceDisplayName("WareHouse.BeginningWareHouses.Fields.CreatedBy")]
        public string CreatedBy { get; set; }

        [XBaseResourceDisplayName("WareHouse.BeginningWareHouses.Fields.ModifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [XBaseResourceDisplayName("WareHouse.BeginningWareHouses.Fields.ModifiedBy")]
        public string ModifiedBy { get; set; }

        [XBaseResourceDisplayName("Common.Unit")]
        public UnitModel UnitModel { get; set; }

        [XBaseResourceDisplayName("Common.WareHouse")]
        public WareHouseModel WareHouseModel { get; set; }

        [XBaseResourceDisplayName("Common.WareHouseItem")]
        public WareHouseItemModel WareHouseItemModel { get; set; }

        public string WareHouseItemName { get; set; }
    }

    public partial class BeginningWareHouseValidator : AbstractValidator<BeginningWareHouseModel>
    {
        public BeginningWareHouseValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.WareHouseId).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.SelectFields.Required", "1"),
                                           localizationService.GetResource("WareHouse.BeginningWareHouses.Fields.WareHouseId")));
            RuleFor(x => x.WareHouseId).SetValidator(new MaximumLengthValidator<BeginningWareHouseModel>(36))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("WareHouse.BeginningWareHouses.Fields.WareHouseId"), 36));

            RuleFor(x => x.ItemId).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.SelectFields.Required"),
                                           localizationService.GetResource("WareHouse.BeginningWareHouses.Fields.ItemId")));
            RuleFor(x => x.ItemId).SetValidator(new MaximumLengthValidator<BeginningWareHouseModel>(36))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("WareHouse.BeginningWareHouses.Fields.ItemId"), 36));

            RuleFor(x => x.UnitId).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.SelectFields.Required"),
                                           localizationService.GetResource("WareHouse.BeginningWareHouses.Fields.UnitId")));
            RuleFor(x => x.UnitId).SetValidator(new MaximumLengthValidator<BeginningWareHouseModel>(36))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("WareHouse.BeginningWareHouses.Fields.UnitId"), 36));

            RuleFor(x => x.Quantity).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                           localizationService.GetResource("WareHouse.BeginningWareHouses.Fields.Quantity")));
       
        }
    }
}

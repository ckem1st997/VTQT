using FluentValidation;
using System;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public sealed partial class WareHouseUserModel : BaseEntityModel
    {
        [XBaseResourceDisplayName("Common.Fields.WareHouseUser.WarehouseId")]
        public string WarehouseId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WareHouseUser.UserId")]
        public string UserId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WareHouseUser.CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WareHouseUser.CreatedBy")]
        public string CreatedBy { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WareHouseUser.UserName")]
        public  string UserName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WareHouseUser.AccountName")]
        public  string AccountName { get; set; }

        [XBaseResourceDisplayName("Warehouse.WareHouses.Fields.Name")]
        public  string WarehouseName { get; set; }

        public WareHouseUserModel()
        {
        }
    }

    public partial class WareHouseUserValidator : AbstractValidator<WareHouseUserModel>
    {
        public WareHouseUserValidator(ILocalizationService localization)
        {
            RuleFor(x => x.WarehouseId).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.SelectFields.Required"),
                                           localization.GetResource("WareHouse.WareHouseUsers.Fields.WarehouseId")));

            RuleFor(x => x.UserId).NotEmpty()
                .WithMessage(string.Format(localization.GetResource("Common.Validators.SelectFields.Required"),
                                           localization.GetResource("WareHouse.WareHouseUsers.Fields.UserId")));
        }
    }
}
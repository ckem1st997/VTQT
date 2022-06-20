using FluentValidation;
using FluentValidation.Validators;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public sealed partial class SerialWareHouseModel : BaseEntityModel
    {
        [XBaseResourceDisplayName("WareHouse.BeginningWareHouses.Fields.ItemId")]

        public string ItemId { get; set; }
        [XBaseResourceDisplayName("WareHouse.SerialWareHous.Fields.Serial")]


        public string Serial { get; set; }
        [XBaseResourceDisplayName("WareHouse.SerialWareHous.Fields.InwardDetailId")]


        public string InwardDetailId { get; set; }
        [XBaseResourceDisplayName("WareHouse.SerialWareHous.Fields.OutwardDetailId")]

        public string OutwardDetailId { get; set; }

        /// <summary>
        /// Đánh dấu Serial mới nhập mà chưa xuất ra (Giá trị mặc định = 0)
        /// </summary>
        [XBaseResourceDisplayName("WareHouse.SerialWareHous.Fields.IsOver")]
        public bool IsOver { get; set; }
    }
    public partial class SerialWareHouseValidator : AbstractValidator<SerialWareHouseModel>
    {
        public SerialWareHouseValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.ItemId).NotEmpty()
              .WithMessage(string.Format(localizationService.GetResource("Common.Validators.SelectFields.Required"),
                                         localizationService.GetResource("WareHouse.BeginningWareHouses.Fields.ItemId")))
              .SetValidator(new MaximumLengthValidator<SerialWareHouseModel>(36))
              .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                         localizationService.GetResource("WareHouse.BeginningWareHouses.Fields.ItemId"), 36));

            RuleFor(x => x.Serial).NotEmpty()
            .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                       localizationService.GetResource("WareHouse.SerialWareHous.Fields.Serial")))
            .SetValidator(new MaximumLengthValidator<SerialWareHouseModel>(50))
            .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                       localizationService.GetResource("WareHouse.SerialWareHous.Fields.Serial"), 50));

            RuleFor(x => x.InwardDetailId)
           .SetValidator(new MaximumLengthValidator<SerialWareHouseModel>(36))
           .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                      localizationService.GetResource("WareHouse.SerialWareHous.Fields.InwardDetailId"), 36));

            RuleFor(x => x.OutwardDetailId)
        .SetValidator(new MaximumLengthValidator<SerialWareHouseModel>(36))
        .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                   localizationService.GetResource("WareHouse.SerialWareHous.Fields.OutwardDetailId"), 36));
        }
    }

}

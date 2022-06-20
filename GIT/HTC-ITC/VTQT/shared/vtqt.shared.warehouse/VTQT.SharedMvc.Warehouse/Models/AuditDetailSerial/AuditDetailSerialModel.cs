using FluentValidation;
using FluentValidation.Validators;
using VTQT.Services.Localization;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public partial class AuditDetailSerialModel : BaseEntityModel
    {
        public string ItemId { get; set; }

        public string Serial { get; set; }

        public string AuditDetailId { get; set; }

        public WareHouseItemModel WareHouseItemModel { get; set; }

        public AuditDetailModel AuditDetailModel { get; set; }

        public AuditDetailSerialModel()
        {
            
        }
    }

    public partial class AuditDetailSerialValidator : AbstractValidator<AuditDetailSerialModel>
    {
        public AuditDetailSerialValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.ItemId).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.SelectFields.Required"),
                                           localizationService.GetResource("WareHouse.AuditDetailSerials.Fields.ItemId")));            

            RuleFor(x => x.Serial).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"),
                                           localizationService.GetResource("WareHouse.AuditDetailSerials.Fields.Serial")));
            RuleFor(x => x.Serial).SetValidator(new MaximumLengthValidator<AuditDetailSerialModel>(50))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"),
                                           localizationService.GetResource("WareHouse.AuditDetailSerials.Fields.Serial"), 50));          
        }
    }
}

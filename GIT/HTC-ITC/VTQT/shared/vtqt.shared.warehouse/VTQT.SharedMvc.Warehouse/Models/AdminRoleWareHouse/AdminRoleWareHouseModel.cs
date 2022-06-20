using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public class AdminRoleWareHouseModel : BaseEntityModel
    {
        [XBaseResourceDisplayName("Warehouse.WareHouses.Fields.UserId")]
        public string UserId { get; set; }

        [XBaseResourceDisplayName("Warehouse.WareHouses.Fields.Inactive")]
        public bool Inactive { get; set; }

        [XBaseResourceDisplayName("Warehouse.WareHouses.Fields.CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [XBaseResourceDisplayName("Warehouse.WareHouses.Fields.CreatedBy")]
        [UIHint("DropDownList")]
        public string CreatedBy { get; set; }

        public IList<SelectListItem> AvailableCreatedBy { get; set; }

        public AdminRoleWareHouseModel()
        {
            Inactive = false;
            AvailableCreatedBy = new List<SelectListItem>();
        }
    }

    public partial class AdminRoleWareHouseValidator : AbstractValidator<AdminRoleWareHouseModel>
    {
        public AdminRoleWareHouseValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.UserId).NotEmpty()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Warehouse.WareHouses.Fields.UserId")));
            RuleFor(x => x.UserId).SetValidator(new MaximumLengthValidator<AdminRoleWareHouseModel>(255))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Warehouse.WareHouses.Fields.UserId"), 255));

           
        }
    }
}
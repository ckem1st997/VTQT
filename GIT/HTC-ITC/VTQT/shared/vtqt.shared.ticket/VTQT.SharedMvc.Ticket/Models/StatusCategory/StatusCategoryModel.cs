using System.Collections.Generic;
using VTQT.Core.Domain;
using VTQT.Web.Framework.Modelling;
using FluentValidation;
using FluentValidation.Validators;
using VTQT.Services.Localization;


namespace VTQT.SharedMvc.Ticket.Models
{
    public partial class StatusCategoryModel: BaseEntityModel, ILocalizedMvcModel<StatusCategoryLocalizedModel>
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public bool Inactive { get; set; }

        public IList<StatusCategoryLocalizedModel> Locales { get; set; }

        public StatusCategoryModel()
        {
            Inactive = false;
            Locales = new List<StatusCategoryLocalizedModel>();
        }
    }

    public partial class StatusCategoryLocalizedModel : ILocalizedMvcLocaleModel
    {
        public string LanguageId { get; set; }

        public string Name { get; set; }
    }

    public partial class StatusCategoryValidator : AbstractValidator<StatusCategoryModel>
    {
        public StatusCategoryValidator(ILocalizationService ls)
        {
            RuleFor(x => x.Name).NotEmpty()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.InputFields.Required"),
                    ls.GetResource("Ticket.TicketCategories.Fields.Name")));
            RuleFor(x => x.Name).SetValidator(new MaximumLengthValidator<StatusCategoryModel>(50))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"),
                    ls.GetResource("Ticket.TicketCategories.Fields.Name"), 50));
        }
    }
}

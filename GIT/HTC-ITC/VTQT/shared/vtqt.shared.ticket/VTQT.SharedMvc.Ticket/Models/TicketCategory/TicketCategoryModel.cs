using FluentValidation;
using FluentValidation.Validators;
using System.Collections.Generic;
using VTQT.Services.Localization;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Ticket.Models
{
    public partial class TicketCategoryModel : BaseEntityModel, ILocalizedMvcModel<TicketCategoryLocalizedModel>
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string ProjectId { get; set; }

        public bool Inactive { get; set; }

        public IList<TicketCategoryLocalizedModel> Locales { get; set; }

        public TicketCategoryModel()
        {
            Inactive = false;
            Locales = new List<TicketCategoryLocalizedModel>();
        }
    }

    public partial class TicketCategoryLocalizedModel : ILocalizedMvcLocaleModel
    {
        public string LanguageId { get; set; }

        public string Name { get; set; }
    }

    public partial class TicketCategoryValidator : AbstractValidator<TicketCategoryModel>
    {
        public TicketCategoryValidator(ILocalizationService ls)
        {
            RuleFor(x => x.Name).NotEmpty()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.InputFields.Required"),
                                           ls.GetResource("Ticket.TicketCategories.Fields.Name")));
            RuleFor(x => x.Name).SetValidator(new MaximumLengthValidator<TicketCategoryModel>(50))
                .WithMessage(string.Format(ls.GetResource("Common.Validators.Characters.MaxLength"),
                                           ls.GetResource("Ticket.TicketCategories.Fields.Name"), 50));

            RuleFor(x => x.ProjectId).NotEmpty()
                .WithMessage(string.Format(ls.GetResource("Common.Validators.InputFields.Required"),
                                           ls.GetResource("Ticket.TicketCategories.Fields.ProjectId")));
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Services.Localization;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Master.Models
{
    public class AppActionModel : BaseEntityModel, ILocalizedMvcModel<AppActionLocalizedModel>
    {
        [XBaseResourceDisplayName("Master.AppActions.Fields.App")]
        public string AppId { get; set; } // varchar(36)

        [XBaseResourceDisplayName("Master.AppActions.Fields.Parent")]
        [UIHint("DropDownList")]
        public string ParentId { get; set; } // varchar(36)

        [XBaseResourceDisplayName("Master.AppActions.Fields.Name")]
        public string Name { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Common.Fields.Description")]
        public string Description { get; set; } // longtext

        [XBaseResourceDisplayName("Master.AppActions.Fields.Controller")]
        [UIHint("DropDownList")]
        public string Controller { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Master.AppActions.Fields.Action")]
        [UIHint("DropDownList")]
        public string Action { get; set; } // varchar(255)

        [XBaseResourceDisplayName("Common.Fields.Icon")]
        public string Icon { get; set; } // varchar(1000)

        [XBaseResourceDisplayName("Common.Fields.ShowOnMenu")]
        public bool ShowOnMenu { get; set; } // tinyint(1)

        [XBaseResourceDisplayName("Common.Fields.Active")]
        public bool Active { get; set; } // tinyint(1)

        [XBaseResourceDisplayName("Common.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; } // int

        public IList<AppActionLocalizedModel> Locales { get; set; }

        #region Associations

        /// <summary>
        /// FK_AppActions_PK_App
        /// </summary>
        [XBaseResourceDisplayName("Master.AppActions.Fields.App")]
        public AppModel App { get; set; }

        /// <summary>
        /// FK_AppActionChildren_PK_Parent_BackReference
        /// </summary>
        public IEnumerable<AppActionModel> AppActionChildren { get; set; }

        public IList<SelectListItem> AvailableApps { get; set; }
        public IList<SelectListItem> AvailableParents { get; set; }
        public IList<SelectListItem> AvailableControllers { get; set; }
        public IList<SelectListItem> AvailableActions { get; set; }

        /// <summary>
        /// FK_AppActionChildren_PK_Parent
        /// </summary>
        public AppActionModel Parent { get; set; }

        #endregion

        public AppActionModel()
        {
            ShowOnMenu = false;
            Active = true;
            DisplayOrder = 1;
            AppActionChildren = new List<AppActionModel>();
            AvailableApps = new List<SelectListItem>();
            AvailableParents = new List<SelectListItem>();
            AvailableControllers = new List<SelectListItem>();
            AvailableActions = new List<SelectListItem>();
            Locales = new List<AppActionLocalizedModel>();
        }
    }

    public partial class AppActionLocalizedModel : ILocalizedMvcLocaleModel
    {
        public string LanguageId { get; set; }

        [XBaseResourceDisplayName("Master.AppActions.Fields.Name")]
        public string Name { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Description")]
        public string Description { get; set; }
    }

    public partial class AppActionValidator : AbstractValidator<AppActionModel>
    {
        public AppActionValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.AppId).NotNull()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.SelectFields.Required"), localizationService.GetResource("Master.AppActions.Fields.App")));

            RuleFor(x => x.Name).NotNull()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Master.AppActions.Fields.Name")));
            RuleFor(x => x.Name).SetValidator(new MaximumLengthValidator<AppActionModel>(255))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Master.AppActions.Fields.Name"), 255));

            RuleFor(x => x.Controller).SetValidator(new MaximumLengthValidator<AppActionModel>(255))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Master.AppActions.Fields.Controller"), 255));

            RuleFor(x => x.Action).SetValidator(new MaximumLengthValidator<AppActionModel>(255))
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.Characters.MaxLength"), localizationService.GetResource("Master.AppActions.Fields.Action"), 255));

            RuleFor(x => x.DisplayOrder).NotNull()
                .WithMessage(string.Format(localizationService.GetResource("Common.Validators.InputFields.Required"), localizationService.GetResource("Common.Fields.DisplayOrder")));
        }
    }

    public class AppActionTreeModel : FancytreeItem
    {
        public AppActionTreeModel()
        {
            ShowOnMenu = false;
            Active = true;
            DisplayOrder = 1;
            children = new List<AppActionTreeModel>();
        }

        public string AppId { get; set; }

        public string? ParentId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string AssemblyArea { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string Icon { get; set; }

        public bool ShowOnMenu { get; set; }

        public bool Active { get; set; }

        public int DisplayOrder { get; set; }

        public new IEnumerable<AppActionTreeModel> children { get; set; }
    }

    public sealed class AppActionModelIdEqualityComparer : IEqualityComparer<AppActionModel>
    {
        public bool Equals(AppActionModel x, AppActionModel y)
        {
            if (Object.ReferenceEquals(x, y))
                return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(AppActionModel obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.Id.GetHashCode();
        }
    }
}

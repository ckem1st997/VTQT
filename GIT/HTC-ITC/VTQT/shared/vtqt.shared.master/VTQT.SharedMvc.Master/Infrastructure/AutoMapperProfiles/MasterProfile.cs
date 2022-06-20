using AutoMapper;
using VTQT.Core.Domain.Localization;
using VTQT.Core.Domain.Master;
using VTQT.Core.Domain.Security;
using VTQT.Core.Infrastructure.Mapper;
using VTQT.SharedMvc.Master.Models;
using VTQT.Web.Framework.Infrastructure.AutoMapperProfiles;

namespace VTQT.SharedMvc.Master.Infrastructure.AutoMapperProfiles
{
    public class MasterProfile : Profile, IOrderedMapperProfile
    {
        public MasterProfile()
        {
            //add some generic mapping rules
            ForAllMaps(CommonProfile.AllMapsAction);

            #region App
            CreateMap<App, AppModel>()
                .ForMember(x => x.Language, opt => opt.Ignore())
                .ForMember(x => x.Locales, opt => opt.Ignore())
                .ForMember(x => x.AvailableAppTypes, opt => opt.Ignore())
                .ForMember(x => x.AvailableLanguages, opt => opt.Ignore());
            CreateMap<AppModel, App>()
                .ForMember(x => x.AppMappings, opt => opt.Ignore())
                .ForMember(x => x.AppActions, opt => opt.Ignore())
                .ForMember(x => x.FK_Report_AppId_BackReferences, opt => opt.Ignore());
            #endregion

            #region AppAction
            CreateMap<AppAction, AppActionModel>()
                .ForMember(x => x.Locales, opt => opt.Ignore())
                .ForMember(x => x.App, opt => opt.Ignore())
                .ForMember(x => x.AppActionChildren, opt => opt.Ignore())
                .ForMember(x => x.AvailableApps, opt => opt.Ignore())
                .ForMember(x => x.AvailableParents, opt => opt.Ignore())
                .ForMember(x => x.AvailableControllers, opt => opt.Ignore())
                .ForMember(x => x.AvailableActions, opt => opt.Ignore())
                .ForMember(x => x.Parent, opt => opt.Ignore());
            CreateMap<AppActionModel, AppAction>()
                .ForMember(x => x.App, opt => opt.Ignore())
                .ForMember(x => x.AppActionChildren, opt => opt.Ignore())
                .ForMember(x => x.AppActionRoles, opt => opt.Ignore())
                .ForMember(x => x.AppActionUserExclusions, opt => opt.Ignore())
                .ForMember(x => x.AppActionUserInclusions, opt => opt.Ignore())
                .ForMember(x => x.Parent, opt => opt.Ignore());

            CreateMap<AppActionSvcEntity, AppActionModel>()
                .ForMember(x => x.App, opt => opt.Ignore())
                .ForMember(x => x.AppActionChildren, opt => opt.Ignore())
                .ForMember(x => x.AvailableApps, opt => opt.Ignore())
                .ForMember(x => x.AvailableParents, opt => opt.Ignore())
                .ForMember(x => x.AvailableControllers, opt => opt.Ignore())
                .ForMember(x => x.AvailableActions, opt => opt.Ignore())
                .ForMember(x => x.Parent, opt => opt.Ignore())
                .ForMember(x => x.Locales, opt => opt.Ignore());
            CreateMap<AppActionModel, AppActionSvcEntity>();
            #endregion

            #region Currency
            CreateMap<Currency, CurrencyModel>()
                .ForMember(x => x.Locales, opt => opt.Ignore());
            CreateMap<CurrencyModel, Currency>()
                .ForMember(x => x.CurrencyCode, opt => opt.Ignore())
                .ForMember(x => x.CreatedOnUtc, opt => opt.Ignore())
                .ForMember(x => x.UpdatedOnUtc, opt => opt.Ignore());
            #endregion

            #region Language
            CreateMap<Language, LanguageModel>()
                .ForMember(x => x.Currency, opt => opt.Ignore())
                .ForMember(x => x.AvailableCultures, opt => opt.Ignore())
                .ForMember(x => x.AvailableTwoLetterLanguageCodes, opt => opt.Ignore())
                .ForMember(x => x.AvailableFlags, opt => opt.Ignore())
                .ForMember(x => x.AvailableCurrencies, opt => opt.Ignore())
                .ForMember(x => x.Locales, opt => opt.Ignore());
            CreateMap<LanguageModel, Language>()
                .ForMember(x => x.LocaleStringResources, opt => opt.Ignore())
                .ForMember(x => x.LocalizedProperties, opt => opt.Ignore());

            CreateMap<LanguageSvcEntity, LanguageModel>()
                .ForMember(x => x.Currency, opt => opt.Ignore())
                .ForMember(x => x.AvailableCultures, opt => opt.Ignore())
                .ForMember(x => x.AvailableTwoLetterLanguageCodes, opt => opt.Ignore())
                .ForMember(x => x.AvailableFlags, opt => opt.Ignore())
                .ForMember(x => x.AvailableCurrencies, opt => opt.Ignore())
                .ForMember(x => x.Locales, opt => opt.Ignore());
            CreateMap<LanguageModel, LanguageSvcEntity>();
            #endregion

            #region LocaleStringResource
            CreateMap<LocaleStringResource, LocaleStringResourceModel>();
            CreateMap<LocaleStringResourceModel, LocaleStringResource>()
                .ForMember(x => x.Language, opt => opt.Ignore());
            #endregion

            #region Role
            CreateMap<Role, RoleModel>()
                .ForMember(x => x.AvailableApps, opt => opt.Ignore())
                .ForMember(x => x.Locales, opt => opt.Ignore());
            CreateMap<RoleModel, Role>()
                .ForMember(x => x.AppActionRoles, opt => opt.Ignore())
                .ForMember(x => x.UserRoles, opt => opt.Ignore());
            #endregion

            #region User
            CreateMap<User, UserModel>()
                .ForMember(x => x.AvailableApps, opt => opt.Ignore());
            CreateMap<UserModel, User>();
            #endregion

            #region Report
            CreateMap<Report, ReportModel>();
            CreateMap<ReportModel, Report>()
                .ForMember(x => x.Report_AppId, opt => opt.Ignore());
            #endregion
        }

        public int Order => 1;
    }
}

using VTQT.Core.Domain.Localization;
using VTQT.Core.Domain.Master;
using VTQT.Core.Domain.Security;
using VTQT.Core.Infrastructure.Mapper;
using VTQT.SharedMvc.Master.Models;

namespace VTQT.SharedMvc.Master
{
    public static class MappingExtensions
    {
        #region App
        public static AppModel ToModel(this App entity)
        {
            return AutoMapperConfiguration.Mapper.Map<App, AppModel>(entity);
        }

        public static App ToEntity(this AppModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<AppModel, App>(model);
        }

        public static App ToEntity(this AppModel model, App destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }
        #endregion

        #region AppAction
        public static AppActionModel ToModel(this AppAction entity)
        {
            return AutoMapperConfiguration.Mapper.Map<AppAction, AppActionModel>(entity);
        }

        public static AppAction ToEntity(this AppActionModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<AppActionModel, AppAction>(model);
        }

        public static AppAction ToEntity(this AppActionModel model, AppAction destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static AppActionModel ToModel(this AppActionSvcEntity entity)
        {
            return AutoMapperConfiguration.Mapper.Map<AppActionSvcEntity, AppActionModel>(entity);
        }

        public static AppActionSvcEntity ToSvcEntity(this AppActionModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<AppActionModel, AppActionSvcEntity>(model);
        }

        public static AppActionSvcEntity ToSvcEntity(this AppActionModel model, AppActionSvcEntity destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }
        #endregion

        #region Currency
        public static CurrencyModel ToModel(this Currency entity)
        {
            return AutoMapperConfiguration.Mapper.Map<Currency, CurrencyModel>(entity);
        }

        public static Currency ToEntity(this CurrencyModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<CurrencyModel, Currency>(model);
        }

        public static Currency ToEntity(this CurrencyModel model, Currency destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }
        #endregion

        #region Language
        public static LanguageModel ToModel(this Language entity)
        {
            return AutoMapperConfiguration.Mapper.Map<Language, LanguageModel>(entity);
        }

        public static Language ToEntity(this LanguageModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<LanguageModel, Language>(model);
        }

        public static Language ToEntity(this LanguageModel model, Language destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        public static LanguageModel ToModel(this LanguageSvcEntity entity)
        {
            return AutoMapperConfiguration.Mapper.Map<LanguageSvcEntity, LanguageModel>(entity);
        }

        public static LanguageSvcEntity ToSvcEntity(this LanguageModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<LanguageModel, LanguageSvcEntity>(model);
        }

        public static LanguageSvcEntity ToSvcEntity(this LanguageModel model, LanguageSvcEntity destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }
        #endregion

        #region LocaleStringResource
        public static LocaleStringResourceModel ToModel(this LocaleStringResource entity)
        {
            return AutoMapperConfiguration.Mapper.Map<LocaleStringResource, LocaleStringResourceModel>(entity);
        }

        public static LocaleStringResource ToEntity(this LocaleStringResourceModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<LocaleStringResourceModel, LocaleStringResource>(model);
        }

        public static LocaleStringResource ToEntity(this LocaleStringResourceModel model, LocaleStringResource destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }
        #endregion

        #region Role
        public static RoleModel ToModel(this Role entity)
        {
            return AutoMapperConfiguration.Mapper.Map<Role, RoleModel>(entity);
        }

        public static Role ToEntity(this RoleModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<RoleModel, Role>(model);
        }

        public static Role ToEntity(this RoleModel model, Role destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }
        #endregion

        #region User
        public static UserModel ToModel(this User entity)
        {
            return AutoMapperConfiguration.Mapper.Map<User, UserModel>(entity);
        }

        public static User ToEntity(this UserModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<UserModel, User>(model);
        }

        public static User ToEntity(this UserModel model, User destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }
        #endregion

        #region Report
        public static ReportModel ToModel(this Report entity)
        {
            return AutoMapperConfiguration.Mapper.Map<Report, ReportModel>(entity);
        }

        public static Report ToEntity(this ReportModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<ReportModel, Report>(model);
        }

        public static Report ToEntity(this ReportModel model, Report destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }
        #endregion
    }
}

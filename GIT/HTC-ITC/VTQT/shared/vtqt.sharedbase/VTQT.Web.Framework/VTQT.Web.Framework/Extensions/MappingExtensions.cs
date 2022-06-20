using VTQT.Core.Domain.Apps;
using VTQT.Core.Domain.Localization;
using VTQT.Core.Domain.Master;
using VTQT.Core.Infrastructure.Mapper;

namespace VTQT.Web.Framework
{
    public static class MappingExtensions
    {
        #region AppSvcEntity
        public static AppSvcEntity ToSvcEntity(this App entity)
        {
            return AutoMapperConfiguration.Mapper.Map<App, AppSvcEntity>(entity);
        }
        public static App ToEntity(this AppSvcEntity svcEntity)
        {
            return AutoMapperConfiguration.Mapper.Map<AppSvcEntity, App>(svcEntity);
        }
        public static App ToEntity(this AppSvcEntity svcEntity, App destination)
        {
            return AutoMapperConfiguration.Mapper.Map(svcEntity, destination);
        }
        #endregion

        #region LanguageSvcEntity
        public static LanguageSvcEntity ToSvcEntity(this Language entity)
        {
            return AutoMapperConfiguration.Mapper.Map<Language, LanguageSvcEntity>(entity);
        }
        public static Language ToEntity(this LanguageSvcEntity svcEntity)
        {
            return AutoMapperConfiguration.Mapper.Map<LanguageSvcEntity, Language>(svcEntity);
        }
        public static Language ToEntity(this LanguageSvcEntity svcEntity, Language destination)
        {
            return AutoMapperConfiguration.Mapper.Map(svcEntity, destination);
        }
        #endregion
    }
}

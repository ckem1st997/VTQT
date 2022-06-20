using System;
using AutoMapper;
using VTQT.Core.Domain.Apps;
using VTQT.Core.Domain.Localization;
using VTQT.Core.Domain.Master;
using VTQT.Core.Infrastructure.Mapper;
using VTQT.Web.Framework.Modelling;

namespace VTQT.Web.Framework.Infrastructure.AutoMapperProfiles
{
    public class CommonProfile : Profile, IOrderedMapperProfile
    {
        public static Action<TypeMap, IMappingExpression> AllMapsAction = (mapConfiguration, map) =>
        {
            //exclude Form and CustomProperties from mapping BaseNopModel
            if (typeof(BaseModel).IsAssignableFrom(mapConfiguration.DestinationType))
            {
                //map.ForMember(nameof(BaseNopModel.Form), options => options.Ignore());
                map.ForMember(nameof(BaseModel.CustomProperties), options => options.Ignore());
            }
        };

        public CommonProfile()
        {
            // Custom-MongoDB
            // special mapper, that avoids DbUpdate exceptions in cases where
            // optional (nullable) int FK properties are 0 instead of null 
            // after mapping model > entity.
            // if type is nullable source value shouldn't be touched
            var fkConverter = new OptionalFkConverter();
            CreateMap<int?, int?>().ConvertUsing(fkConverter);
            CreateMap<int, int?>().ConvertUsing(fkConverter);

            //add some generic mapping rules
            ForAllMaps(AllMapsAction);

            #region AppSvcEntity
            CreateMap<App, AppSvcEntity>();
            CreateMap<AppSvcEntity, App>()
                .ForMember(x => x.AppMappings, x => x.Ignore())
                .ForMember(x => x.AppActions, x => x.Ignore())
                .ForMember(x => x.FK_Report_AppId_BackReferences, x => x.Ignore());
            #endregion

            #region LanguageSvcEntity
            CreateMap<Language, LanguageSvcEntity>();
            CreateMap<LanguageSvcEntity, Language>()
                .ForMember(x => x.LocaleStringResources, x => x.Ignore())
                .ForMember(x => x.LocalizedProperties, x => x.Ignore());
            #endregion
        }

        public int Order => 0;
    }

    public class OptionalFkConverter : ITypeConverter<int?, int?>, ITypeConverter<int, int?>
    {
        public int? Convert(int? source, int? destination, ResolutionContext context)
        {
            return source;
        }

        public int? Convert(int source, int? destination, ResolutionContext context)
        {
            return source == 0 ? (int?)null : source;
        }
    }
}

using System;
using Autofac;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Core.Events;
using VTQT.Core.Logging;
using VTQT.Services.Apps;
using VTQT.Services.Configuration;
using VTQT.Services.Helpers;
using VTQT.Services.Localization;

namespace VTQT.Services
{
    public interface ICommonServices
    {
        IComponentContext Container { get; }
        IXBaseCacheManager CacheManager { get; }
        IAppContext AppContext { get; }
        IWebHelper WebHelper { get; }
        IWorkContext WorkContext { get; }
        ILocalizationService Localization { get; }
        IEventPublisher EventPublisher { get; }
        IMvcNotifier MvcNotifier { get; }
        ISettingService Settings { get; }
        IAppService AppService { get; }
        IDateTimeHelper DateTimeHelper { get; }
    }

    public static class ICommonServicesExtensions
    {
        public static TService Resolve<TService>(this ICommonServices services)
        {
            return services.Container.Resolve<TService>();
        }

        public static TService Resolve<TService>(this ICommonServices services, object serviceKey)
        {
            return services.Container.ResolveKeyed<TService>(serviceKey);
        }

        public static TService ResolveNamed<TService>(this ICommonServices services, string serviceName)
        {
            return services.Container.ResolveNamed<TService>(serviceName);
        }

        public static object Resolve(this ICommonServices services, Type serviceType)
        {
            return services.Resolve(null, serviceType);
        }

        public static object Resolve(this ICommonServices services, object serviceKey, Type serviceType)
        {
            return services.Container.ResolveKeyed(serviceKey, serviceType);
        }

        public static object ResolveNamed(this ICommonServices services, string serviceName, Type serviceType)
        {
            return services.Container.ResolveNamed(serviceName, serviceType);
        }
    }
}

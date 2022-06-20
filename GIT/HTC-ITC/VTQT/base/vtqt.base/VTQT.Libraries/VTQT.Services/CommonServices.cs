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
    public class CommonServices : ICommonServices
    {
        private readonly IComponentContext _container;
        private readonly Lazy<IXBaseCacheManager> _cacheManager;
        private readonly Lazy<IAppContext> _appContext;
        private readonly Lazy<IWebHelper> _webHelper;
        private readonly Lazy<IWorkContext> _workContext;
        private readonly Lazy<IEventPublisher> _eventPublisher;
        private readonly Lazy<ILocalizationService> _localization;
        private readonly Lazy<IMvcNotifier> _mvcNotifier;
        private readonly Lazy<ISettingService> _settings;
        private readonly Lazy<IAppService> _appService;
        private readonly Lazy<IDateTimeHelper> _dateTimeHelper;

        public CommonServices(
            IComponentContext container,
            Lazy<IXBaseCacheManager> cacheManager,
            Lazy<IAppContext> appContext,
            Lazy<IWebHelper> webHelper,
            Lazy<IWorkContext> workContext,
            Lazy<IEventPublisher> eventPublisher,
            Lazy<ILocalizationService> localization,
            Lazy<IMvcNotifier> notifier,
            Lazy<ISettingService> settings,
            Lazy<IAppService> appService,
            Lazy<IDateTimeHelper> dateTimeHelper)
        {
            _container = container;
            _cacheManager = cacheManager;
            _appContext = appContext;
            _webHelper = webHelper;
            _workContext = workContext;
            _eventPublisher = eventPublisher;
            _localization = localization;
            _mvcNotifier = notifier;
            _settings = settings;
            _appService = appService;
            _dateTimeHelper = dateTimeHelper;
        }

        public IComponentContext Container => _container;
        public IXBaseCacheManager CacheManager => _cacheManager.Value;
        public IAppContext AppContext => _appContext.Value;
        public IWebHelper WebHelper => _webHelper.Value;
        public IWorkContext WorkContext => _workContext.Value;
        public IEventPublisher EventPublisher => _eventPublisher.Value;
        public ILocalizationService Localization => _localization.Value;
        public IMvcNotifier MvcNotifier => _mvcNotifier.Value;
        public ISettingService Settings => _settings.Value;
        public IAppService AppService => _appService.Value;
        public IDateTimeHelper DateTimeHelper => _dateTimeHelper.Value;
    }
}

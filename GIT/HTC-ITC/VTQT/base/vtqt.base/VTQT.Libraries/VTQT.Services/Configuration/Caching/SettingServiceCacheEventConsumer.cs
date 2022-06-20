using VTQT.Caching;
using VTQT.Core.Domain.Master;
using VTQT.Core.Infrastructure;
using VTQT.Services.Caching;

namespace VTQT.Services.Configuration.Caching
{
    /// <summary>
    /// Represents a setting cache event consumer
    /// </summary>
    public partial class SettingServiceCacheEventConsumer : ServiceCacheEventConsumer<Setting>
    {
        public SettingServiceCacheEventConsumer()
            : base(EngineContext.Current.Resolve<IXBaseCacheManager>())
        {

        }
    }
}

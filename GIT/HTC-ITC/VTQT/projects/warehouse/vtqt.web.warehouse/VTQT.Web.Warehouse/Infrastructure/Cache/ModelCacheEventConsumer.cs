using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Core.Domain.Master;
using VTQT.Core.Events;
using VTQT.Services.Events;

namespace VTQT.Web.Warehouse.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer :
        //languages
        IConsumer<EntityInsertedEvent<Language>>,
        IConsumer<EntityUpdatedEvent<Language>>,
        IConsumer<EntityDeletedEvent<Language>>,
        //settings
        IConsumer<EntityUpdatedEvent<Setting>>
    {
        #region Fields

        private readonly IXBaseCacheManager _cacheManager;

        #endregion

        #region Ctor

        public ModelCacheEventConsumer(
            IXBaseCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        #region Languages

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Language> eventMessage)
        {
            //clear all localizable models

        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Language> eventMessage)
        {
            //clear all localizable models

        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Language> eventMessage)
        {
            //clear all localizable models

        }

        #endregion

        #region Setting

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Setting> eventMessage)
        {
            //clear models which depend on settings

        }

        #endregion

        #endregion
    }
}

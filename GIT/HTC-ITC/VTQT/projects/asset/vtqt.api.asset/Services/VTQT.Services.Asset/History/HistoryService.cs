using System;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;

namespace VTQT.Services.Asset
{
    public partial class HistoryService : IHistoryService
    {
        #region Fields
        private readonly IRepository<History> _historyRepository;
        #endregion

        #region Ctor
        public HistoryService()
        {
            _historyRepository = EngineContext.Current.Resolve<IRepository<History>>(DataConnectionHelper.ConnectionStringNames.Asset);
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(History entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _historyRepository.InsertAsync(entity);

            return result;
        }
        #endregion

        #region List
        public IPagedList<History> Get(HistorySearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _historyRepository.Table
                        where p.AssetId == ctx.AssetId
                        select p;

            if (ctx.Keywords.HasValue())
            {
                query = from p in query
                        where p.Content != null &&
                              p.Content.Contains(ctx.Keywords)
                        select p;
            }

            query = from p in query orderby p.TimeStamp select p;

            return new PagedList<History>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IPagedList<History> GetHistoryAttachment(HistorySearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _historyRepository.Table
                        where p.AssetAttachmentId == ctx.AssetAttachmentId
                        select p;

            if (ctx.Keywords.HasValue())
            {
                query = from p in query
                        where p.Content != null &&
                              p.Content.Contains(ctx.Keywords)
                        select p;
            }

            query = from p in query orderby p.TimeStamp select p;

            return new PagedList<History>(query, ctx.PageIndex, ctx.PageSize);
        }
        #endregion

        #region Utilities

        #endregion
    }
}

using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Asset;

namespace VTQT.Services.Asset
{
    public partial interface IHistoryService
    {
        Task<int> InsertAsync(History entity);

        IPagedList<History> Get(HistorySearchContext ctx);

        IPagedList<History> GetHistoryAttachment(HistorySearchContext ctx);
    }
}

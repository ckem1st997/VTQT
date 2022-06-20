using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface IChannelAreaService
    {
        Task<int> InsertAsync(ChannelArea entity);

        Task<int> UpdateAsync(ChannelArea entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<ChannelArea> GetByIdAsync(string id);

        Task<ChannelArea> GetByCodeAsync(string code);

        Task<bool> ExistedAsync(string code);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<ChannelArea> Get(ChannelAreaSearchContext ctx);

        IList<ChannelArea> GetAll(bool showHidden = false);
    }
}
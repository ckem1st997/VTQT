using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface IChannelStatusService
    {
        Task<int> InsertAsync(ChannelStatus entity);

        Task<int> UpdateAsync(ChannelStatus entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<ChannelStatus> GetByIdAsync(string id);

        Task<ChannelStatus> GetByCodeAsync(string code);

        Task<bool> ExistedAsync(string code);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<ChannelStatus> Get(ChannelStatusSearchContext ctx);

        IList<ChannelStatus> GetAll(bool showHidden = false);
    }
}
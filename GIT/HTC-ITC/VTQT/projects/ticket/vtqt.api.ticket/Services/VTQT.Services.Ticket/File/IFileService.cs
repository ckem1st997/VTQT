using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public partial interface IFileService
    {
        Task<int> InsertAsync(File entity);

        Task<long> InsertsAsync(IEnumerable<File> entities);

        Task<int> UpdateAsync(File entity);

        Task<int> UpdatesAsync(IEnumerable<File> entities);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<File> GetByIdAsync(string id);

        IList<File> GetByTicketIdAsync(string ticketId);

        IList<File> GetByCRIdAsync(string crId);

        IList<File> GetByFTTHIdAsync(string ftthId);
    }
}
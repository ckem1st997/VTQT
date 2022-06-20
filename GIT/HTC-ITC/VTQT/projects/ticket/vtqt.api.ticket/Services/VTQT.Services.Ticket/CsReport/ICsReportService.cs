using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface ICsReportService
    {
        Task<Core.Domain.Ticket.CsReport> GetByIdAsync(string id);

        Task<int> UpdateAsync(Core.Domain.Ticket.CsReport entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<int> InsertAsync(CsReport entity);

    }
}
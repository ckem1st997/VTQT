using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public partial interface IInfrastructorFeeService
    {
        Task<int> InsertAsync(InfrastructorFee entity);

        Task<long> InsertsAsync(IEnumerable<InfrastructorFee> entities);

        Task<int> UpdateAsync(InfrastructorFee entity);

        Task<int> UpdatesAsync(IEnumerable<InfrastructorFee> entities);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<InfrastructorFee> GetByIdAsync(string id);

        IList<InfrastructorFee> GetByTicketIdAsync(string ticketId);

        IPagedList<InfrastructorFee> Get(InfrastructorFeeSearchContext ctx);

        IList<Core.Domain.Ticket.InfrastructorFee> GetByInfrastructorFeeId(InfrastructorFeeSearchContext ctx);
    }
}

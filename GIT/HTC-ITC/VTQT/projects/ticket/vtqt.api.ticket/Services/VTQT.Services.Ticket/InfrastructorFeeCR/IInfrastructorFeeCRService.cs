using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface IInfrastructorFeeCRService
    {
        Task<int> InsertAsync(InfrastructorFeeCR entity);

        Task<long> InsertsAsync(IEnumerable<InfrastructorFeeCR> entities);

        Task<int> UpdateAsync(InfrastructorFeeCR entity);

        Task<int> UpdatesAsync(IEnumerable<InfrastructorFeeCR> entities);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<InfrastructorFeeCR> GetByIdAsync(string id);

        IList<InfrastructorFeeCR> GetByCRIdAsync(string crId);

        IPagedList<InfrastructorFeeCR> Get(InfrastructorFeeCRSearchContext ctx);

        IList<Core.Domain.Ticket.InfrastructorFeeCR> GetByInfrastructorFeeCRId(InfrastructorFeeCRSearchContext ctx);
    }
}
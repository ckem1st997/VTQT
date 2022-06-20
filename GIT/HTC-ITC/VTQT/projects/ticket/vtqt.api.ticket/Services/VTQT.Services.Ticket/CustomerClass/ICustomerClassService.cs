using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface ICustomerClassService
    {
        Task<int> InsertAsync(CustomerClass entity);

        Task<int> UpdateAsync(CustomerClass entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<CustomerClass> GetByIdAsync(string id);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<CustomerClass> Get(CustomerClassSearchContext ctx);

        IList<CustomerClass> GetAll(bool showHidden = false);

        Task<bool> ExistsAsync(string code);

        Task<bool> ExistsAsync(string oldCode, string newCode);
    }
}
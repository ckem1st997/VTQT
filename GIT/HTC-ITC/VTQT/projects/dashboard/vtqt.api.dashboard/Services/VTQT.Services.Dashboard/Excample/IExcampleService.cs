using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Dashboard;

namespace VTQT.Services.Dashboard
{
    public interface IExcampleService
    {
        Task<long> InsertAsync(IEnumerable<Example> entity);

        Task<long> UpdateAsync(IEnumerable<Example> entity);

        Task<long> DeleteAsync(IEnumerable<string> ids);
        
        Task<Example> GetByIdAsync(string id);

        IList<Example> GetAll();
    }
}
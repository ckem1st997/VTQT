using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Domain.Dashboard;

namespace VTQT.Services.Dashboard
{
    public interface IMasterVTCNTTService
    {
        Task<long> InsertAsync(IEnumerable<MasterVTCNTT> entity);

        Task<long> UpdateAsync(IEnumerable<MasterVTCNTT> entity);

        Task<long> DeleteAsync(IEnumerable<string> ids);
        
        Task<MasterVTCNTT> GetByIdAsync(string id);

        IList<MasterVTCNTT> GetAll();
        Task<IList<MasterVTCNTT>> GetAllQuery();
        
    }
}
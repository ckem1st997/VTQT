using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Asset;

namespace VTQT.Services.Asset
{
    public partial interface IStationService
    {
        Task<Station> GetByIdAsync(string id);

        IList<Station> GetAll();

        void UpdateAll();

        Task<int> UpdateAsync(Station entity);

        Task<int> InsertAsync(Station entity);

        Task<bool> ExistsAsync(string code);

        Task<Station> GetByCodeAsync(string code);
    }
}

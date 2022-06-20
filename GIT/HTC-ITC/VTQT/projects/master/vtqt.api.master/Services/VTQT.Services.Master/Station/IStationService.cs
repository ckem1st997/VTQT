using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Qlsc;

namespace VTQT.Services.Master
{
    public partial interface IStationService
    {
        IList<danh_sach_tram> GetAll();

        IPagedList<danh_sach_tram> Get(StationSearchContext ctx);

        Task<danh_sach_tram> GetByIdAsync(int id);

        IList<danh_sach_tram> GetAvailable();

        Task<danh_sach_tram> GetByCodeAsync(string code);
    }
}

using System.Threading.Tasks;

namespace VTQT.Services.Asset
{
    public partial interface IMaintenanceService
    {
        Task<int> InsertAsync(Core.Domain.Asset.Maintenance entity);
    }
}

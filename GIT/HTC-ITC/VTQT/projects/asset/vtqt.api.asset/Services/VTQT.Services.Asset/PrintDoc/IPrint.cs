using System.Threading.Tasks;
using VTQT.Core.Domain.Asset;

namespace VTQT.Services.Asset
{
    public partial interface IPrint
    {
        Task<Audit> GetByIdToWordAuditAsync(string id);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.File;

namespace VTQT.Services.File
{
    public interface IFilesService
    {
        Task<long> InsertRangeAsync(IEnumerable<Files> entities);

        Task<Files> GetByIdAsync(string id);
    }
}
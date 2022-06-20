using System.Collections.Generic;
using System.Threading.Tasks;

namespace VTQT.Services.Dashboard
{
    public interface IDynamicService
    {
        Task<List<object>> GetListAll(string table);
    }
}
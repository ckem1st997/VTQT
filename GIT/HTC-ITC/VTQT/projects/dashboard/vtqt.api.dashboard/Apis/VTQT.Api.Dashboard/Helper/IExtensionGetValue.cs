using System.Collections.Generic;
using System.Threading.Tasks;

namespace VTQT.Api.Dashboard.Helper
{
    public interface IExtensionGetValue
    {
        Task<IList<GetColumnNameModel>> GetColumnName(string name);
    }
}

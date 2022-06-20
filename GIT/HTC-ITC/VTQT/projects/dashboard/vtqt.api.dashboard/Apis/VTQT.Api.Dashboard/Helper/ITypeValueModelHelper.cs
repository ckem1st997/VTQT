using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.SharedMvc.Dashboard.Models;

namespace VTQT.Api.Dashboard.Helper
{
    public interface ITypeValueModelHelper
    {
        public Task<IList<TypeValuesTreeModel>> GetTypeValueTree(int? expandLevel, bool showHidden = false);


        public Task<IList<TypeValueModel>> GetTypeValueDropdownTreeAsync(bool showHidden = false, bool showList = false);

    }
}
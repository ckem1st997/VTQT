using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Asset;

namespace VTQT.Services.Asset
{
    public partial interface IAssetCategoryService
    {
        Task<int> InsertAsync(AssetCategory entity);

        Task<int> UpdateAsync(AssetCategory entity);

        Task<int> DeleteAsync(IEnumerable<string> ids);

        Task<bool> ExistAsync(string code);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool status);

        Task<AssetCategory> GetByIdAsync(string id);

        Task UpdatePath(AssetCategory item);

        void UpdateAllPath();

        IList<AssetCategory> GetAll(bool showHidden = false, int assetType = 0);

        IPagedList<AssetCategory> Get(AssetCategorySearchContext ctx);

        IList<SelectListItem> GetMvcListItems(bool showHidden = false);
    }
}

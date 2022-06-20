using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Asset;

namespace VTQT.Services.Asset
{
    public partial interface IAssetService
    {
        Task<int> InsertAsync(Core.Domain.Asset.Asset entity);

        Task<int> UpdateAsync(Core.Domain.Asset.Asset entity);

        Task<int> DeleteAsync(IEnumerable<string> ids);

        Task<bool> ExistAsync(string code);

        Task<Core.Domain.Asset.Asset> GetByIdAsync(string id);

        IList<Core.Domain.Asset.Asset> GetAll(int assetType);

        IList<Core.Domain.Asset.Asset> GetListDropDownd();

        IPagedList<AssetSvcEntity> Get(AssetSearchContext ctx);

        IPagedList<AssetSvcEntity> GetExcelData(AssetSearchContext ctx);

        IList<SelectListItem> GetMvcListItems(int assetType);

        IList<Core.Domain.Asset.Asset> GetByIdsAsync(IEnumerable<string> ids);
        
        IPagedList<Core.Domain.Asset.Asset> GetMvcDropdownList(AssetSearchContext ctx);
    }
}

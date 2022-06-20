using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Domain.Asset;

namespace VTQT.Services.Asset
{
    public partial interface IAssetAttachmentService
    {
        Task<int> InsertAsync(AssetAttachment entity);

        Task<int> UpdateAsync(AssetAttachment entity);

        Task<int> DeleteAsync(IEnumerable<string> ids);

        Task<bool> ExistAsync(string code);

        Task<AssetAttachment> GetByIdAsync(string id);

        IPagedList<AssetAttachment> GetMvcDropdownList(AssetAttachmentSearchContext ctx);

        IPagedList<AssetAttachment> Get(AssetAttachmentSearchContext ctx);

        IPagedList<AssetAttachment> GetExcelData(AssetAttachmentSearchContext ctx);

        IList<SelectListItem> GetMvcListItems(int assetType);

        IList<AssetAttachment> GetByIdsAsync(IEnumerable<string> ids);
    }
}
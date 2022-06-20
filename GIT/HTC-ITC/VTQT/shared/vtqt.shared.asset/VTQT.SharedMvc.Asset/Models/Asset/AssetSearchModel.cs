using VTQT.Core.Domain.Asset.Enum;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Asset.Models
{
    public  class AssetSearchModel : BaseSearchModel
    {
        public AssetType AssetType { get; set; }

        public string OrganizationId { get; set; }
    }
}

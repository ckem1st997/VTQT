using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VTQT.Core;

namespace VTQT.Services.Asset
{
    public partial interface IChartService
    {
        IEnumerable<Core.Domain.Asset.Asset> GetChartPie(int AssetType, string OrganizationId);
        IEnumerable<Core.Domain.Asset.Asset> GetChartCoulunm(int AssetType, string OrganizationId);
        Task<IPagedList<Core.Domain.Asset.Asset>> GetWarrantyDuration(ChartSearchContext model);
        Task<IPagedList<Core.Domain.Asset.Asset>> GetProjectBase(ChartSearchContext model);


    }
}

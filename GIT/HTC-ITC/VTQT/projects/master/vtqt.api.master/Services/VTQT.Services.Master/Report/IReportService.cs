using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Asset;
using VTQT.SharedMvc.Asset.Models;
using VTQT.SharedMvc.Master.Models;

namespace VTQT.Services.Master
{
    public partial interface IReportService
    {
        IList<ReportTreeModel> GetReportListByAppId(string appid, bool inactive = false);

        Task<IList<ReportValueModel>> GetReportExcel(ReportValueSearchContext ctx, string route);

        Task<IList<ReportInwardMisaModel>> GetReportInwardMisaExcel(ReportInwardMisaSearchContext ctx, string route);

        Task<IList<ReportOutwardMisaModel>> GetReportOutwardMisaExcel(ReportOutwardMisaSearchContext ctx, string route);

        Task<IList<ReportAssetInfrastructorModel>> GetExportAssetInfrastructorTreeAsync(ReportAssetInfrastructorSearchContext ctx, string route);
        Task<IList<Asset>> GetExportAssetOfficeTreeAsync(ReportAssetInfrastructorSearchContext ctx, string route);
        Task<IList<Asset>> GetExportAssetProjectTreeAsync(ReportAssetInfrastructorSearchContext ctx, string route);

        Task<IPagedList<ReportValueModel>> GetReport(ReportValueSearchContext ctx, string route);

        Task<IPagedList<ReportInwardMisaModel>> GetReportInwardMisa(ReportInwardMisaSearchContext ctx, string route);

        Task<IPagedList<ReportOutwardMisaModel>> GetReportOutwardMisa(ReportOutwardMisaSearchContext ctx, string route);

        Task<IPagedList<ReportAssetInfrastructorModel>> GetReportAssetInfrastructorTreeAsync(ReportAssetInfrastructorSearchContext ctx, string route);
        
        Task<IPagedList<Asset>> GetReportAssetProjectTreeAsync(ReportAssetInfrastructorSearchContext ctx, string route);
        
        Task<IPagedList<Asset>> GetReportAssetOfficeTreeAsync(ReportAssetInfrastructorSearchContext ctx, string route);

        IPagedList<AssetMaintenanceReportModel> GetReportAssetMaintenance(AssetMaintenanceReportSearchContext ctx);

        IList<AssetMaintenanceReportModel> GetReportExcelAssetMaintenance(AssetMaintenanceReportSearchContext ctx);
    }
}

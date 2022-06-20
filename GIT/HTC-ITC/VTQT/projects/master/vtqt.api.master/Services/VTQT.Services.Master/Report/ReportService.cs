using System.Collections.Generic;
using System.Linq;
using VTQT.Core;
using VTQT.Core.Domain.Master;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using VTQT.SharedMvc.Master.Models;
using System;
using VTQT.Core.Domain.Warehouse;
using VTQT.Core.Domain.Warehouse.Enum;
using VTQT.SharedMvc.Master.Extensions;
using System.Text;
using LinqToDB.Data;
using System.Threading.Tasks;
using VTQT.SharedMvc.Asset.Models;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Domain.FbmOrganization;

namespace VTQT.Services.Master
{
    public partial class ReportService : IReportService
    {
        #region Fields

        private readonly IRepository<Report> _reportRepository;
        private readonly IRepository<InwardDetail> _inwardDetailRepository;
        private readonly IRepository<OutwardDetail> _outwardDetailRepository;
        private readonly IRepository<BeginningWareHouse> _beginningRepository;
        private readonly IRepository<Inward> _inwardRepository;
        private readonly IRepository<Outward> _outwardRepository;
        private readonly IRepository<WareHouseItem> _itemRepository;
        private readonly IRepository<Unit> _unitRepository;
        private readonly IRepository<WareHouseItemCategory> _categoryRepository;
        private readonly IRepository<Asset> _assetRepository;
        private readonly IRepository<Maintenance> _assetMaintenanceRepository;
        private readonly IRepository<MaintenanceDetail> _assetMaintenanceDetailRepository;
        private readonly IRepository<Station> _stationRepository;
        private readonly IRepository<UsageStatus> _useStatusRepository;
        private readonly IRepository<WareHouse> _wareHouseRepository;
        private readonly IIntRepository<OrganizationUnit> _organizationRepository;
        private readonly IRepository<WareHouseItem> _wareHouseItemRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<AccObject> _accObjectRepository;
        private readonly IWorkContext _workContext;

        private readonly IDictionary<string, string> reportRoute;

        #endregion

        #region Ctor

        public ReportService(IWorkContext workContext)
        {
            reportRoute = new Dictionary<string, string>
            {
                { "Total", "/report/summary-warehouse" },
                { "Detail", "/report/ledger-warehouse" },
                { "InwardMisa", "/report/inward-misa" },
                { "OutwardMisa", "/report/outward-misa" },
                //{ "FTTH", "/report/ftth" },
                //{ "ChannelLink", "/report/channel" },
                //{ "TicketLink", "/report/ticket" },
                //{ "CRLink", "/report/cr" },
                //{ "ChannelDatetimeLink", "/report/channel-datetime" },
                { AssetStatic.AssetInfrastructor, AssetStatic.AssetInfrastructorLink },
                { AssetStatic.AssetProject, AssetStatic.AssetProjectLink },
                { AssetStatic.AssetOffice, AssetStatic.AssetOfficeLink },
                { AssetStatic.WareHouseSerial, AssetStatic.WareHouseSerialLink },
                //{ AssetStatic.FTTH, AssetStatic.FTTHLink },
                //{ AssetStatic.Channel, AssetStatic.ChannelLink },
                //{ AssetStatic.Ticket, AssetStatic.TicketLink },
                //{ AssetStatic.CR, AssetStatic.CRLink },
                //{ AssetStatic.ChannelDatetime, AssetStatic.ChannelDatetimeLink },
            };
            _reportRepository =
                EngineContext.Current.Resolve<IRepository<Report>>(DataConnectionHelper.ConnectionStringNames.Master);
            _inwardDetailRepository =
                EngineContext.Current.Resolve<IRepository<InwardDetail>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _outwardDetailRepository =
                EngineContext.Current.Resolve<IRepository<OutwardDetail>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _beginningRepository =
                EngineContext.Current.Resolve<IRepository<BeginningWareHouse>>(DataConnectionHelper
                    .ConnectionStringNames.Warehouse);
            _inwardRepository =
                EngineContext.Current.Resolve<IRepository<Inward>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _outwardRepository =
                EngineContext.Current.Resolve<IRepository<Outward>>(
                    DataConnectionHelper.ConnectionStringNames.Warehouse);
            _itemRepository =
                EngineContext.Current.Resolve<IRepository<WareHouseItem>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _unitRepository =
                EngineContext.Current.Resolve<IRepository<Unit>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _categoryRepository =
                EngineContext.Current.Resolve<IRepository<WareHouseItemCategory>>(DataConnectionHelper
                    .ConnectionStringNames.Warehouse);
            _assetRepository =
                EngineContext.Current.Resolve<IRepository<Asset>>(DataConnectionHelper.ConnectionStringNames.Asset);
            _stationRepository =
                EngineContext.Current.Resolve<IRepository<Station>>(DataConnectionHelper.ConnectionStringNames.Asset);
            _useStatusRepository =
                EngineContext.Current.Resolve<IRepository<UsageStatus>>(
                    DataConnectionHelper.ConnectionStringNames.Asset);
            _organizationRepository =
                EngineContext.Current.Resolve<IIntRepository<OrganizationUnit>>(DataConnectionHelper
                    .ConnectionStringNames.FbmOrganization);
            _assetMaintenanceRepository =
                EngineContext.Current.Resolve<IRepository<Maintenance>>(
                    DataConnectionHelper.ConnectionStringNames.Asset);
            _assetMaintenanceDetailRepository =
                EngineContext.Current.Resolve<IRepository<MaintenanceDetail>>(DataConnectionHelper.ConnectionStringNames
                    .Asset);
            _wareHouseRepository =
                EngineContext.Current.Resolve<IRepository<WareHouse>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _wareHouseItemRepository= EngineContext.Current.Resolve<IRepository<WareHouseItem>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _vendorRepository = EngineContext.Current.Resolve<IRepository<Vendor>>(DataConnectionHelper.ConnectionStringNames
        .Warehouse);
            _accObjectRepository = EngineContext.Current.Resolve<IRepository<AccObject>>(DataConnectionHelper.ConnectionStringNames
        .Warehouse);
            _workContext = workContext;
        }

        #endregion

        #region Methods

        #endregion

        #region List

        public async Task<IList<Asset>> GetExportAssetProjectTreeAsync(ReportAssetInfrastructorSearchContext ctx,
            string route)
        {
            if (string.IsNullOrEmpty(route))
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (string.IsNullOrEmpty(ctx.OrganizationUnitId))
            {
                throw new ArgumentNullException(nameof(ctx.OrganizationUnitId));
            }

            // BuildMyString.com generated code. Please enjoy your string responsibly.
            if (route.Equals(reportRoute[AssetStatic.AssetProject]))
            {
                var list = new StringBuilder();
                list.Append(" ( ");
                var department =
                    _organizationRepository.Table.FirstOrDefault(x => x.Id.ToString() == ctx.OrganizationUnitId);

                if (department != null && !string.IsNullOrEmpty(department.TreePath))
                {
                    var departmentIds = _organizationRepository.Table.Where(x => !string.IsNullOrEmpty(x.TreePath) &&
                            x.TreePath.Contains(department.TreePath))
                        .Select(x => x.Id.ToString());
                    if (departmentIds?.ToList().Count > 0)
                    {
                        var listDepartmentIds = departmentIds.ToList();
                        for (int i = 0; i < listDepartmentIds.Count; i++)
                        {
                            if (i == listDepartmentIds.Count - 1)
                                list.Append(listDepartmentIds[i]);
                            else
                                list.Append(listDepartmentIds[i] + ", ");
                        }
                    }
                }

                list.Append(" ) ");
                StringBuilder sb = new StringBuilder();

                sb.Append("SELECT ");
                sb.Append("  a.AllocationDate, ");
                sb.Append("  a.Code, ");
                sb.Append("  a.Name, ");
                sb.Append("  a.ProjectName, ");
                sb.Append("  a.ProjectCode, ");
                sb.Append("  a.CustomerName, ");
                sb.Append("  a.CustomerCode, ");
                sb.Append("  us.Description AS CurrentUsageStatus, ");
                sb.Append("  ac.Name AS CategoryId, ");
                sb.Append("  a.OriginQuantity, ");
                sb.Append("  a.RecallQuantity, ");
                sb.Append("  a.SoldQuantity, ");
                sb.Append("  a.BrokenQuantity, ");
                sb.Append("  a.UnitName, ");
                sb.Append("  a.MaintenancedDate ");
                sb.Append("FROM Asset a ");
                sb.Append("  INNER JOIN UsageStatus us ");
                sb.Append("    ON a.CurrentUsageStatus = us.Id ");
                sb.Append("  inner JOIN AssetCategory ac ");
                sb.Append("    ON a.CategoryId = ac.Id ");
                sb.Append("WHERE a.AssetType = 30 ");
                if (department != null && !string.IsNullOrEmpty(department.TreePath))
                {
                    sb.Append(" AND a.OrganizationUnitId in ");
                    sb.Append(list);
                }

                if (ctx.ItemCode.HasValue())
                    sb.Append(" AND a.Code=@pCode ");
                if (ctx.KeyWords.HasValue())
                    sb.Append(" AND a.ProjectCode = @pProjectCode ");
                //  sb.Append("AND a.CreatedDate >= @fromDate ");
                sb.Append(" AND a.AllocationDate <= @toDate ");
                sb.Append(" ORDER BY a.AllocationDate desc ");


                DataParameter OranId = new DataParameter("@OranId", ctx.OrganizationUnitId);
                DataParameter toDate = new DataParameter("@toDate", ConvertDateTimeToDateTimeSql(ctx.ToDate));
                DataParameter pCode = new DataParameter("@pCode", ctx.ItemCode);
                DataParameter pStationCode = new DataParameter("@pProjectCode", ctx.KeyWords);
                return await _assetRepository.DataConnection.QueryToListAsync<Asset>(sb.ToString(), OranId,
                    toDate, pCode, pStationCode);
            }

            return new List<Asset>();
        }

        /// <summary>
        /// Sinh báo cáo
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        public async Task<IPagedList<ReportValueModel>> GetReport(ReportValueSearchContext ctx, string route)
        {
            if (string.IsNullOrEmpty(route))
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (route.Equals(reportRoute["Total"]))
            {
                return await GetReportTotalAsync(ctx);
            }
            else if (route.Equals(reportRoute["Detail"]))
            {
                return GetReportDetail(ctx);
            }
            else if (route.Equals(reportRoute["FTTH"]))
            {
                return GetReportDetail(ctx);
            }
            else if (route.Equals(reportRoute["Channel"]))
            {
                return GetReportDetail(ctx);
            }
            else if (route.Equals(reportRoute["Ticket"]))
            {
                return GetReportDetail(ctx);
            }
            else if (route.Equals(reportRoute["CR"]))
            {
                return GetReportDetail(ctx);
            }
            else if (route.Equals(reportRoute["ChannelDatetime"]))
            {
                return GetReportDetail(ctx);
            }
            else if (route.Equals(reportRoute[AssetStatic.WareHouseSerial]))
            {
                return await GetReportSerial(ctx);
            }
            else
            {
                return null;
            }
        }

        public async Task<IPagedList<ReportInwardMisaModel>> GetReportInwardMisa(ReportInwardMisaSearchContext ctx, string route)
        {
            if (string.IsNullOrEmpty(route))
            {
                throw new ArgumentNullException(nameof(route));
            }
            else if (route.Equals(reportRoute["InwardMisa"]))
            {
                return await GetReportInwardMisa(ctx);
            }
            else
            {
                return null;
            }
        }

        private async Task<IPagedList<ReportValueModel>> GetReportSerial(ReportValueSearchContext ctx)
        {
            if (ctx is null || ctx.WareHouseId is null)
            {
                throw new NotImplementedException("Value search null or WareHouseId is null !");
            }


            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("  whi.Code as WareHouseItemCode, ");
            sb.Append("  whi.Name as WareHouseItemName, ");
            sb.Append("  (SELECT ");
            sb.Append("      CASE WHEN SUM(whl.Quantity) IS NULL THEN 0 ELSE SUM(whl.Quantity) END ");
            sb.Append("    FROM vWareHouseLedger whl ");
            sb.Append("    WHERE whl.WareHouseId = @pWareHouseId ");
            sb.Append("    AND whl.ItemId = whi.Id ");
            sb.Append("    AND whl.VoucherDate < @pFrom) AS Beginning, ");
            sb.Append("  (SELECT ");
            sb.Append("      CASE WHEN SUM(Id.Quantity) IS NULL THEN 0 ELSE SUM(Id.Quantity) END ");
            sb.Append("    FROM Inward i ");
            sb.Append("      INNER JOIN InwardDetail Id ");
            sb.Append("        ON i.Id = Id.InwardId ");
            sb.Append("    WHERE i.VoucherDate BETWEEN @pFrom AND @pTo ");
            sb.Append("    AND i.WareHouseId = @pWareHouseId ");
            if (!string.IsNullOrEmpty(ctx.WareHouseItemId))
                sb.Append("    AND Id.ItemId = @pWareHouseItemId ");
            sb.Append("    AND Id.ItemId = whi.Id) AS Import, ");
            sb.Append("  (SELECT ");
            sb.Append("      CASE WHEN SUM(od.Quantity) IS NULL THEN 0 ELSE SUM(od.Quantity) END ");
            sb.Append("    FROM Outward o ");
            sb.Append("      INNER JOIN OutwardDetail od ");
            sb.Append("        ON o.Id = od.OutwardId ");
            sb.Append("    WHERE o.VoucherDate BETWEEN @pFrom AND @pTo ");
            sb.Append("    AND o.WareHouseId = @pWareHouseId ");
            if (!string.IsNullOrEmpty(ctx.WareHouseItemId))
                sb.Append("    AND od.ItemId = @pWareHouseItemId ");
            sb.Append("    AND od.ItemId = whi.Id) AS Export, ");
            sb.Append("  (SELECT ");
            sb.Append("      CASE WHEN SUM(whl.Quantity) IS NULL THEN 0 ELSE SUM(whl.Quantity) END ");
            sb.Append("    FROM vWareHouseLedger whl ");
            sb.Append("    WHERE whl.WareHouseId = @pWareHouseId ");
            sb.Append("    AND whl.ItemId = whi.Id ");
            sb.Append("    AND whl.VoucherDate < @pFrom) + (SELECT ");
            sb.Append("      CASE WHEN SUM(Id.Quantity) IS NULL THEN 0 ELSE SUM(Id.Quantity) END ");
            sb.Append("    FROM Inward i ");
            sb.Append("      INNER JOIN InwardDetail Id ");
            sb.Append("        ON i.Id = Id.InwardId ");
            sb.Append("    WHERE i.VoucherDate BETWEEN @pFrom AND @pTo ");
            sb.Append("    AND i.WareHouseId = @pWareHouseId ");
            if (!string.IsNullOrEmpty(ctx.WareHouseItemId))
                sb.Append("    AND Id.ItemId = @pWareHouseItemId ");
            sb.Append("    AND Id.ItemId = whi.Id) - (SELECT ");
            sb.Append("      CASE WHEN SUM(od.Quantity) IS NULL THEN 0 ELSE SUM(od.Quantity) END ");
            sb.Append("    FROM Outward o ");
            sb.Append("      INNER JOIN OutwardDetail od ");
            sb.Append("        ON o.Id = od.OutwardId ");
            sb.Append("    WHERE o.VoucherDate BETWEEN @pFrom  AND @pTo ");
            sb.Append("    AND o.WareHouseId = @pWareHouseId ");
            if (!string.IsNullOrEmpty(ctx.WareHouseItemId))
                sb.Append("    AND od.ItemId = @pWareHouseItemId ");
            sb.Append("    AND od.ItemId = whi.Id) AS Balance, ");
            sb.Append("  u.UnitName ");
            sb.Append("FROM WareHouseItem whi ");
            sb.Append("  INNER JOIN Unit u ");
            sb.Append("    ON whi.UnitId = u.Id INNER JOIN vWareHouseLedger whl ON whi.Id = whl.ItemId  ");
            sb.Append("  WHERE whl.WareHouseId= @pWareHouseId ");
            if (!string.IsNullOrEmpty(ctx.WareHouseItemId))
                sb.Append(" and whi.Id = @pWareHouseItemId ");
            sb.Append("GROUP BY whi.Id, ");
            sb.Append("         whi.Name, ");
            sb.Append("         u.UnitName ");
            sb.Append("ORDER BY whl.VoucherDate ");
            sb.Append(" LIMIT @p2 OFFSET @p3 ");

            //count


            StringBuilder sbCount = new StringBuilder();

            sbCount.Append("SELECT COUNT(*) FROM ( ");
            sbCount.Append("SELECT ");
            sbCount.Append("  whi.Code as WareHouseItemCode ");
            sbCount.Append("FROM WareHouseItem whi ");
            sbCount.Append("  INNER JOIN Unit u ");
            sbCount.Append("    ON whi.UnitId = u.Id INNER JOIN vWareHouseLedger whl ON whi.Id = whl.ItemId  ");
            sbCount.Append("  WHERE whl.WareHouseId= @pWareHouseId ");
            if (!string.IsNullOrEmpty(ctx.WareHouseItemId))
                sbCount.Append("and whi.Id = @pWareHouseItemId ");
            sbCount.Append("GROUP BY whi.Id, ");
            sbCount.Append("         whi.Name ");
            sbCount.Append("              ) t   ");
            sbCount.Append("  ");


            var from = ctx.FromDate.ToUniversalTime();
            var to = ctx.ToDate.ToUniversalTime();
            DataParameter p2 = new DataParameter("p2", ctx.PageSize);
            DataParameter p3 = new DataParameter("p3", ctx.PageIndex * ctx.PageSize);

            DataParameter pWareHouseId = new DataParameter("pWareHouseId", ctx.WareHouseId);
            DataParameter pWareHouseItemId = new DataParameter("pWareHouseItemId", ctx.WareHouseItemId);
            DataParameter pFrom = new DataParameter("pFrom",
                "" + from.Value.Year + "-" + from.Value.Month + "-" + from.Value.Day + "  12:0:0 AM  ");
            DataParameter pTo = new DataParameter("pTo",
                "" + to.Value.Year + "-" + to.Value.Month + "-" + to.Value.Day + "  12:0:0 AM  ");
            var result = await _beginningRepository.DataConnection.QueryToListAsync<ReportValueModel>(sb.ToString(),
                pWareHouseId, pWareHouseItemId, pFrom, pTo, p2, p3);
            var resCount = await _beginningRepository.DataConnection.QueryToListAsync<int>(sbCount.ToString(),
                pWareHouseId, pWareHouseItemId, pFrom, pTo);

            return new PagedList<ReportValueModel>(result, ctx.PageIndex, ctx.PageSize, resCount.FirstOrDefault());
        }

        /// <summary>
        /// Sinh báo cáo excel
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        public async Task<IList<ReportValueModel>> GetReportExcel(ReportValueSearchContext ctx, string route)
        {
            if (string.IsNullOrEmpty(route))
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (route.Equals(reportRoute["Total"]))
            {
                return await GetReportExcelTotal(ctx);
            }
            else if (route.Equals(reportRoute["Detail"]))
            {
                return GetReportExcelDetail(ctx);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sinh báo cáo excel nhập kho misa
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        public async Task<IList<ReportInwardMisaModel>> GetReportInwardMisaExcel(ReportInwardMisaSearchContext ctx, string route)
        {
            if (string.IsNullOrEmpty(route))
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (route.Equals(reportRoute["InwardMisa"]))
            {
                return await GetReportInwardMisaExcelDetail(ctx);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sinh báo cáo excel xuất kho misa
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        public async Task<IList<ReportOutwardMisaModel>> GetReportOutwardMisaExcel(ReportOutwardMisaSearchContext ctx, string route)
        {
            if (string.IsNullOrEmpty(route))
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (route.Equals(reportRoute["OutwardMisa"]))
            {
                return await GetReportOutwardMisaExcelDetail(ctx);
            }
            else
            {
                return null;
            }
        }

        public IPagedList<AssetMaintenanceReportModel> GetReportAssetMaintenance(
            AssetMaintenanceReportSearchContext ctx)
        {
            if (ctx.ToDate == null ||
                string.IsNullOrEmpty(ctx.OrganizationUnitId) ||
                string.IsNullOrEmpty(ctx.StationCode))
            {
                return null;
            }

            var listDepartmentIds = new List<string>();

            var department =
                _organizationRepository.Table.FirstOrDefault(x => x.Id.ToString() == ctx.OrganizationUnitId);

            if (!string.IsNullOrEmpty(department.TreePath))
            {
                var departmentIds = _organizationRepository.Table.Where(x => !string.IsNullOrEmpty(x.TreePath) &&
                                                                             x.TreePath.Contains(department.TreePath))
                    .Select(x => x.Id.ToString());
                if (departmentIds?.ToList().Count > 0)
                {
                    listDepartmentIds = departmentIds.ToList();
                }
            }
            else
            {
                var departmentIds = from p in _organizationRepository.Table select p.Id.ToString();

                if (departmentIds?.ToList().Count > 0)
                {
                    listDepartmentIds = departmentIds.ToList();
                }
            }

            if (!string.IsNullOrEmpty(ctx.AssetId))
            {
                var reports = from d in _assetMaintenanceDetailRepository.Table
                              join am in _assetMaintenanceRepository.Table on d.MaintenanceId equals am.Id into amd
                              join a in _assetRepository.Table on d.AssetId equals a.Id into ad
                              from da in ad.DefaultIfEmpty()
                              from dma in amd.DefaultIfEmpty()
                              where dma.MaintenancedDate <= ctx.ToDate &&
                                    da.Id == ctx.AssetId &&
                                    da.AssetType == ctx.AssetType &&
                                    da.StationCode == ctx.StationCode &&
                                    listDepartmentIds.Contains(da.OrganizationUnitId)
                              select new AssetMaintenanceReportModel
                              {
                                  AssetId = da.Id,
                                  OrganizationUnitId = da.OrganizationUnitId,
                                  OrganizationUnitName = da.OrganizationUnitName,
                                  AssetName = $"[{da.Code}] {da.Name}",
                                  CurrentUsageStatus = da.CurrentUsageStatus,
                                  MaintenanceDate = dma.MaintenancedDate.ToLocalTime(),
                                  Content = dma.Content,
                                  Serial = da.Serial,
                                  MaintenanceLocation = d.MaintenanceLocation,
                                  ReasonDescription = d.ReasonDescription
                              };

                var results = reports.ToList();
                results.Sort();

                return new PagedList<AssetMaintenanceReportModel>(results, ctx.PageIndex, ctx.PageSize);
            }
            else
            {
                var reports = from d in _assetMaintenanceDetailRepository.Table
                              join am in _assetMaintenanceRepository.Table on d.MaintenanceId equals am.Id into amd
                              join a in _assetRepository.Table on d.AssetId equals a.Id into ad
                              from da in ad.DefaultIfEmpty()
                              from dma in amd.DefaultIfEmpty()
                              where dma.MaintenancedDate <= ctx.ToDate &&
                                    da.AssetType == ctx.AssetType &&
                                    da.StationCode == ctx.StationCode &&
                                    listDepartmentIds.Contains(da.OrganizationUnitId)
                              select new AssetMaintenanceReportModel
                              {
                                  AssetId = da.Id,
                                  OrganizationUnitId = da.OrganizationUnitId,
                                  OrganizationUnitName = da.OrganizationUnitName,
                                  AssetName = $"[{da.Code}] {da.Name}",
                                  CurrentUsageStatus = da.CurrentUsageStatus,
                                  MaintenanceDate = dma.MaintenancedDate.ToLocalTime(),
                                  Content = dma.Content,
                                  Serial = da.Serial,
                                  MaintenanceLocation = d.MaintenanceLocation,
                                  ReasonDescription = d.ReasonDescription
                              };

                var results = reports.ToList();
                results.Sort();

                return new PagedList<AssetMaintenanceReportModel>(results, ctx.PageIndex, ctx.PageSize);
            }
        }

        public IList<AssetMaintenanceReportModel> GetReportExcelAssetMaintenance(
            AssetMaintenanceReportSearchContext ctx)
        {
            if (ctx.ToDate == null ||
                string.IsNullOrEmpty(ctx.OrganizationUnitId) ||
                string.IsNullOrEmpty(ctx.StationCode))
            {
                return null;
            }

            var listDepartmentIds = new List<string>();

            var department =
                _organizationRepository.Table.FirstOrDefault(x => x.Id.ToString() == ctx.OrganizationUnitId);

            if (!string.IsNullOrEmpty(department.TreePath))
            {
                var departmentIds = _organizationRepository.Table.Where(x => !string.IsNullOrEmpty(x.TreePath) &&
                                                                             x.TreePath.Contains(department.TreePath))
                    .Select(x => x.Id.ToString());
                if (departmentIds?.ToList().Count > 0)
                {
                    listDepartmentIds = departmentIds.ToList();
                }
            }
            else
            {
                var departmentIds = from p in _organizationRepository.Table select p.Id.ToString();

                if (departmentIds?.ToList().Count > 0)
                {
                    listDepartmentIds = departmentIds.ToList();
                }
            }

            if (!string.IsNullOrEmpty(ctx.AssetId))
            {
                var reports = from d in _assetMaintenanceDetailRepository.Table
                              join am in _assetMaintenanceRepository.Table on d.MaintenanceId equals am.Id into amd
                              join a in _assetRepository.Table on d.AssetId equals a.Id into ad
                              from da in ad.DefaultIfEmpty()
                              from dma in amd.DefaultIfEmpty()
                              where dma.MaintenancedDate <= ctx.ToDate &&
                                    da.Id == ctx.AssetId &&
                                    da.AssetType == ctx.AssetType &&
                                    da.StationCode == ctx.StationCode &&
                                    listDepartmentIds.Contains(da.OrganizationUnitId)
                              select new AssetMaintenanceReportModel
                              {
                                  AssetId = da.Id,
                                  OrganizationUnitId = da.OrganizationUnitId,
                                  OrganizationUnitName = da.OrganizationUnitName,
                                  AssetName = $"[{da.Code}] {da.Name}",
                                  CurrentUsageStatus = da.CurrentUsageStatus,
                                  MaintenanceDate = dma.MaintenancedDate.ToLocalTime(),
                                  Content = dma.Content,
                                  Serial = da.Serial,
                                  MaintenanceLocation = d.MaintenanceLocation,
                                  ReasonDescription = d.ReasonDescription
                              };

                var results = reports.ToList();
                results.Sort();

                return results;
            }
            else
            {
                var reports = from d in _assetMaintenanceDetailRepository.Table
                              join am in _assetMaintenanceRepository.Table on d.MaintenanceId equals am.Id into amd
                              join a in _assetRepository.Table on d.AssetId equals a.Id into ad
                              from da in ad.DefaultIfEmpty()
                              from dma in amd.DefaultIfEmpty()
                              where dma.MaintenancedDate <= ctx.ToDate &&
                                    da.AssetType == ctx.AssetType &&
                                    da.StationCode == ctx.StationCode &&
                                    listDepartmentIds.Contains(da.OrganizationUnitId)
                              select new AssetMaintenanceReportModel
                              {
                                  AssetId = da.Id,
                                  OrganizationUnitId = da.OrganizationUnitId,
                                  OrganizationUnitName = da.OrganizationUnitName,
                                  AssetName = $"[{da.Code}] {da.Name}",
                                  CurrentUsageStatus = da.CurrentUsageStatus,
                                  MaintenanceDate = dma.MaintenancedDate.ToLocalTime(),
                                  Content = dma.Content,
                                  Serial = da.Serial,
                                  MaintenanceLocation = d.MaintenanceLocation,
                                  ReasonDescription = d.ReasonDescription
                              };

                var results = reports.ToList();
                results.Sort();

                return results;
            }
        }

        /// <summary>
        /// Sinh dữ liệu cây báo cáo
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="inactive"></param>
        /// <returns></returns>
        public IList<ReportTreeModel> GetReportListByAppId(string appid, bool inactive = false)
        {
            var qq = new Queue<ReportTreeModel>();
            var lstCheck = new List<ReportTreeModel>();
            var result = new List<ReportTreeModel>();

            var reportTreeModels = GetReportUnits(appid, inactive)
                .Select(s => new ReportTreeModel
                {
                    children = new List<ReportTreeModel>(),
                    folder = false,
                    key = s.Route,
                    title = s.Name,
                    tooltip = s.Name,
                    Name = s.Name,
                    Route = s.Route,
                    Inactive = s.Inactive,
                    AppId = s.AppId,
                    Type = s.Type,
                    Order = s.Order
                });
            foreach (var item in reportTreeModels)
            {
                result.Add(item);
            }

            return result.OrderBy(x => x.Order).ToList();
        }

        #endregion

        #region Utilities
        private IList<ReportModel> GetReportUnits(string appid, bool inactive = false)
        {
            var query = from p in _reportRepository.Table select p;
            if (string.IsNullOrEmpty(appid))
                query = from p in query
                        where p.Inactive.Equals(inactive)
                        orderby p.Name
                        select p;
            else
                query = from p in query
                        where p.AppId.Equals(appid) && p.Inactive.Equals(inactive)
                        orderby p.Name
                        select p;

            var models = new List<ReportModel>();

            if (query?.ToList()?.Count > 0)
            {
                query.ToList().ForEach(x =>
                {
                    var m = new ReportModel
                    {
                        Id = x.Id,
                        AppId = x.AppId,
                        Name = x.Name,
                        Inactive = x.Inactive,
                        Route = x.Route,
                        Type = x.Type,
                        Order = x.Order,
                        ReportTemplate = x.ReportTemplate,
                        ExcelTemplate = x.ExcelTemplate,
                        Description = x.Description
                    };
                    models.Add(m);
                });
            }

            return models;
        }

        private async Task<IPagedList<ReportInwardMisaModel>> GetReportInwardMisa(ReportInwardMisaSearchContext ctx)
        {
            var query = from a in _inwardRepository.Table
                        join b in _inwardDetailRepository.Table on a.Id equals b.InwardId
                        join acc in _accObjectRepository.Table  on a.AccObjectId equals acc.Id into acco
                        from acc in acco.DefaultIfEmpty()
                        join d in _wareHouseItemRepository.Table on b.ItemId equals d.Id
                        join c in _wareHouseRepository.Table on a.WareHouseID equals c.Id
                        join e in _vendorRepository.Table on a.VendorId equals e.Id into ps
                        from e in ps.DefaultIfEmpty()
                        join u in _unitRepository.Table on b.UnitId equals u.Id
                        select new ReportInwardMisaModel
                        {
                            Moment=a.VoucherDate.ToString("dd/MM/yyyy"),
                            WareHouseItemId =c.Name,
                            Voucher=a.Voucher,
                            VoucherDate=a.VoucherDate,
                            VoucherDateTime= a.VoucherDate.ToString("dd/MM/yyyy"),
                            VoucherCode=a.VoucherCode,
                            VendorCode=acc.Code,
                            ProjectId=acc.Name,
                            NoteRender=a.Description,
                            WareHouseItemCode=d.Code,
                            WareHouseItemName=d.Name,
                            AccountMore="1561",
                            AccountYes = "331",
                            UnitName=u.UnitName,
                            Amount=b.Amount,
                            Price=b.UIPrice,
                            Quantity=b.Quantity,
                            Id = b.Id,
                            DepartmentName=a.Deliver,
                            WareHouseId = c.Id,
                        };
            if (ctx.FromDate.HasValue)
            {
                query = query?
                    .Where(x => x.VoucherDate >= ctx.FromDate.Value);
            }

            if (ctx.ToDate.HasValue)
            {
                query = query?
                    .Where(x => x.VoucherDate <= ctx.ToDate.Value);
            }

            if (ctx.WareHouseId.HasValue())
            {
                var department = _wareHouseRepository.Table.FirstOrDefault(x => x.Id == ctx.WareHouseId);

                if (department != null)
                {
                    StringBuilder GetListChidren = new StringBuilder();

                    GetListChidren.Append("with recursive cte (Id, Name, ParentId) as ( ");
                    GetListChidren.Append("  select     wh.Id, ");
                    GetListChidren.Append("             wh.Name, ");
                    GetListChidren.Append("             wh.ParentId ");
                    GetListChidren.Append("  from       WareHouse wh ");
                    GetListChidren.Append("  where      wh.ParentId='" + ctx.WareHouseId + "' ");
                    GetListChidren.Append("  union all ");
                    GetListChidren.Append("  SELECT     p.Id, ");
                    GetListChidren.Append("             p.Name, ");
                    GetListChidren.Append("             p.ParentId ");
                    GetListChidren.Append("  from       WareHouse  p ");
                    GetListChidren.Append("  inner join cte ");
                    GetListChidren.Append("          on p.ParentId = cte.id ");
                    GetListChidren.Append(") ");
                    GetListChidren.Append(" select cte.Id FROM cte GROUP BY cte.Id,cte.Name,cte.ParentId; ");
                    var departmentIds =
                        await _unitRepository.DataConnection.QueryToListAsync<string>(GetListChidren.ToString());
                    departmentIds.Add(ctx.WareHouseId);
                    if (departmentIds?.ToList().Count > 0)
                    {
                        var listDepartmentIds = departmentIds.ToList();
                        query = from p in query
                                where listDepartmentIds.Contains(p.WareHouseId)
                                select p;
                    }
                }
                else
                {
                    query = from p in query
                            where p.WareHouseId.Contains(ctx.WareHouseId.Trim())
                            select p;
                }
            }

          
            return new PagedList<ReportInwardMisaModel>(query, ctx.PageIndex, ctx.PageSize);
        }
        private IPagedList<ReportValueModel> GetReportDetail(ReportValueSearchContext ctx)
        {
            if (string.IsNullOrEmpty(ctx.WareHouseId) ||
                string.IsNullOrEmpty(ctx.WareHouseItemId) ||
                ctx.FromDate == null ||
                ctx.ToDate == null)
            {
                return null;
            }

            var results = new List<ReportValueModel>();
            var listWarehouseIds = new List<string>();

            var warehouse = _wareHouseRepository.Table.FirstOrDefault(x => x.Id == ctx.WareHouseId);

            if (!string.IsNullOrEmpty(warehouse.Path))
            {
                var warehouseIds = _wareHouseRepository.Table.Where(x => !string.IsNullOrEmpty(x.Path) &&
                                                                        x.Path.Contains(warehouse.Path))
                                                             .Select(x => x.Id);
                if (warehouseIds?.ToList().Count > 0)
                {
                    listWarehouseIds = warehouseIds.ToList();
                }
            }

            var inwardsOnTime = (from i in _inwardRepository.Table
                                 join iwd in _inwardDetailRepository.Table on i.Id equals iwd.InwardId into iwdi
                                 from idwi in iwdi.DefaultIfEmpty()
                                 where idwi != null &&
                                       idwi.ItemId == ctx.WareHouseItemId &&
                                       i.VoucherDate >= ctx.FromDate &&
                                       i.VoucherDate <= ctx.ToDate &&
                                       listWarehouseIds.Contains(i.WareHouseID)
                                 orderby i.VoucherDate
                                 select i).Distinct();

            var outwardsOnTime = (from o in _outwardRepository.Table
                                  join owd in _outwardDetailRepository.Table on o.Id equals owd.OutwardId into owdo
                                  from odwo in owdo.DefaultIfEmpty()
                                  where odwo != null &&
                                        odwo.ItemId == ctx.WareHouseItemId &&
                                        o.VoucherDate >= ctx.FromDate &&
                                        o.VoucherDate <= ctx.ToDate &&
                                        listWarehouseIds.Contains(o.WareHouseID)
                                  orderby o.VoucherDate
                                  select o).Distinct();

            var firstInwardVoucher = inwardsOnTime?.FirstOrDefault();
            var firstOutwardVoucher = outwardsOnTime?.FirstOrDefault();

            if (firstInwardVoucher != null && firstOutwardVoucher != null)
            {
                if (firstInwardVoucher.VoucherDate <= firstOutwardVoucher.VoucherDate)
                {
                    var beginItem = _beginningRepository.Table.FirstOrDefault(x => x.WareHouseId == firstInwardVoucher.WareHouseID &&
                                                                                   x.ItemId == ctx.WareHouseItemId);
                    CalculationFirstVoucher(results, ctx, firstInwardVoucher, null, beginItem);
                    if (results.Count == 0)
                    {
                        results.Add(
                            new ReportValueModel
                            {
                                Moment = firstInwardVoucher.VoucherDate,
                                WareHouseId = firstInwardVoucher.WareHouseID,
                                WareHouseItemId = ctx.WareHouseItemId,
                                Beginning = beginItem == null ? 0 : beginItem.Quantity,
                                VoucherCodeImport = firstInwardVoucher.VoucherCode,
                                UserId = firstInwardVoucher.CreatedBy
                            }
                        );
                    }
                }
                else
                {
                    var beginItem = _beginningRepository.Table.FirstOrDefault(x => x.WareHouseId == firstOutwardVoucher.WareHouseID &&
                                                                                   x.ItemId == ctx.WareHouseItemId);
                    CalculationFirstVoucher(results, ctx, null, firstOutwardVoucher, beginItem);
                    if (results.Count == 0)
                    {
                        results.Add(
                            new ReportValueModel
                            {
                                Moment = firstOutwardVoucher.VoucherDate,
                                WareHouseId = firstOutwardVoucher.WareHouseID,
                                WareHouseItemId = ctx.WareHouseItemId,
                                Beginning = beginItem == null ? 0 : beginItem.Quantity,
                                VoucherCodeExport = firstOutwardVoucher.VoucherCode,
                                UserId = firstOutwardVoucher.CreatedBy
                            }
                        );
                    }
                }
            }
            else if (firstInwardVoucher != null && firstOutwardVoucher == null)
            {
                var beginItem = _beginningRepository.Table.FirstOrDefault(x => x.WareHouseId == firstInwardVoucher.WareHouseID &&
                                                                               x.ItemId == ctx.WareHouseItemId);
                CalculationFirstVoucher(results, ctx, firstInwardVoucher, null, beginItem);
                if (results.Count == 0)
                {
                    results.Add(
                        new ReportValueModel
                        {
                            Moment = firstInwardVoucher.VoucherDate,
                            WareHouseId = firstInwardVoucher.WareHouseID,
                            WareHouseItemId = ctx.WareHouseItemId,
                            Beginning = beginItem == null ? 0 : beginItem.Quantity,
                            VoucherCodeImport = firstInwardVoucher.VoucherCode,
                            UserId = firstInwardVoucher.CreatedBy
                        }
                    );
                }
            }
            else if (firstOutwardVoucher != null && firstInwardVoucher == null)
            {
                var beginItem = _beginningRepository.Table.FirstOrDefault(x => x.WareHouseId == firstOutwardVoucher.WareHouseID &&
                                                                               x.ItemId == ctx.WareHouseItemId);
                CalculationFirstVoucher(results, ctx, null, firstOutwardVoucher, beginItem);
                if (results.Count == 0)
                {
                    results.Add(
                        new ReportValueModel
                        {
                            Moment = firstOutwardVoucher.VoucherDate,
                            WareHouseId = firstOutwardVoucher.WareHouseID,
                            WareHouseItemId = ctx.WareHouseItemId,
                            Beginning = beginItem == null ? 0 : beginItem.Quantity,
                            VoucherCodeExport = firstOutwardVoucher.VoucherCode,
                            UserId = firstOutwardVoucher.CreatedBy
                        }
                    );
                }
            }
            else
            {
                if (listWarehouseIds?.Count > 0)
                {
                    listWarehouseIds.ForEach(warehouseId =>
                    {
                        var beginItem = _beginningRepository.Table.FirstOrDefault(x => x.WareHouseId == warehouseId &&
                                                                                       x.ItemId == ctx.WareHouseItemId);
                        var beginTime = new DateTime(1990, 01, 01);
                        var totalImportPrimitive = (from i in _inwardRepository.Table
                                                    join iwd in _inwardDetailRepository.Table on i.Id equals iwd.InwardId into iwdi
                                                    from idwi in iwdi.DefaultIfEmpty()
                                                    where idwi != null &&
                                                          idwi.ItemId == ctx.WareHouseItemId &&
                                                          i.WareHouseID == warehouseId &&
                                                          i.VoucherDate >= beginTime &&
                                                          i.VoucherDate < ctx.FromDate
                                                    select idwi.Quantity).Sum();
                        var totalExportPrimitive = (from o in _outwardRepository.Table
                                                    join owd in _outwardDetailRepository.Table on o.Id equals owd.OutwardId into owdo
                                                    from odwo in owdo.DefaultIfEmpty()
                                                    where odwo != null &&
                                                          odwo.ItemId == ctx.WareHouseItemId &&
                                                          o.WareHouseID == warehouseId &&
                                                          o.VoucherDate >= beginTime &&
                                                          o.VoucherDate < ctx.FromDate
                                                    select odwo.Quantity).Sum();
                        results.Add(
                                new ReportValueModel
                                {
                                    Moment = (DateTime)ctx.FromDate,
                                    WareHouseId = warehouseId,
                                    WareHouseItemId = ctx.WareHouseItemId,
                                    Beginning = beginItem == null ? 0 : beginItem.Quantity + totalImportPrimitive - totalExportPrimitive
                                }
                            );
                    });
                }
            }

            if (results.Count > 0)
            {
                var inwardsOnTimeList = inwardsOnTime?.ToList();
                inwardsOnTimeList?.ForEach(x =>
                {
                    var inwardReasons = Enum.GetValues(typeof(InwardReason));
                    var strPurpose = "";
                    foreach (InwardReason rea in inwardReasons)
                    {
                        if (x.Reason == (int)rea)
                        {
                            strPurpose = rea.GetEnumDescription();
                        }
                    }

                    if (string.IsNullOrEmpty(ctx.DepartmentId) && string.IsNullOrEmpty(ctx.ProjectId))
                    {
                        var inwardDetails = _inwardDetailRepository.Table.Where(iwd => iwd.InwardId == x.Id &&
                            iwd.ItemId == ctx.WareHouseItemId);
                        UpdateInwardToResults(inwardDetails, results, ctx, x, strPurpose);
                    }
                    else if (!string.IsNullOrEmpty(ctx.DepartmentId) && string.IsNullOrEmpty(ctx.ProjectId))
                    {
                        var inwardDetails = _inwardDetailRepository.Table.Where(iwd => iwd.InwardId == x.Id &&
                            iwd.DepartmentId == ctx.DepartmentId &&
                            iwd.ItemId == ctx.WareHouseItemId);
                        UpdateInwardToResults(inwardDetails, results, ctx, x, strPurpose);
                    }
                    else if (!string.IsNullOrEmpty(ctx.ProjectId) && string.IsNullOrEmpty(ctx.DepartmentId))
                    {
                        var inwardDetails = _inwardDetailRepository.Table.Where(iwd => iwd.InwardId == x.Id &&
                            iwd.ProjectId == ctx.ProjectId &&
                            iwd.ItemId == ctx.WareHouseItemId);
                        UpdateInwardToResults(inwardDetails, results, ctx, x, strPurpose);
                    }
                    else
                    {
                        var inwardDetails = _inwardDetailRepository.Table.Where(iwd => iwd.InwardId == x.Id &&
                            iwd.DepartmentId == ctx.DepartmentId &&
                            iwd.ProjectId == ctx.ProjectId &&
                            iwd.ItemId == ctx.WareHouseItemId);
                        UpdateInwardToResults(inwardDetails, results, ctx, x, strPurpose);
                    }
                });

                var outwardsOnTimeList = outwardsOnTime?.ToList();
                outwardsOnTimeList?.ForEach(x =>
                {
                    var outwardReasons = Enum.GetValues(typeof(OutwardReason));
                    var strPurpose = "";
                    foreach (OutwardReason rea in outwardReasons)
                    {
                        if (x.Reason == (int)rea)
                        {
                            strPurpose = rea.GetEnumDescription();
                        }
                    }

                    if (string.IsNullOrEmpty(ctx.DepartmentId) && string.IsNullOrEmpty(ctx.ProjectId))
                    {
                        var outwardDetails = _outwardDetailRepository.Table.Where(owd => owd.OutwardId == x.Id &&
                            owd.ItemId == ctx.WareHouseItemId);
                        UpdateOutwardToResults(outwardDetails, results, ctx, x, strPurpose);
                    }
                    else if (!string.IsNullOrEmpty(ctx.DepartmentId) && string.IsNullOrEmpty(ctx.ProjectId))
                    {
                        var outwardDetails = _outwardDetailRepository.Table.Where(owd => owd.OutwardId == x.Id &&
                            owd.DepartmentId == ctx.DepartmentId &&
                            owd.ItemId == ctx.WareHouseItemId);
                        UpdateOutwardToResults(outwardDetails, results, ctx, x, strPurpose);
                    }
                    else if (!string.IsNullOrEmpty(ctx.ProjectId) && string.IsNullOrEmpty(ctx.DepartmentId))
                    {
                        var outwardDetails = _outwardDetailRepository.Table.Where(owd => owd.OutwardId == x.Id &&
                            owd.ProjectId == ctx.ProjectId &&
                            owd.ItemId == ctx.WareHouseItemId);
                        UpdateOutwardToResults(outwardDetails, results, ctx, x, strPurpose);
                    }
                    else
                    {
                        var outwardDetails = _outwardDetailRepository.Table.Where(owd => owd.OutwardId == x.Id &&
                            owd.ProjectId == ctx.ProjectId &&
                            owd.DepartmentId == ctx.DepartmentId &&
                            owd.ItemId == ctx.WareHouseItemId);
                        UpdateOutwardToResults(outwardDetails, results, ctx, x, strPurpose);
                    }
                });

                results.Sort();
                ReportValueModel lastItem = null;
                var itemWareHouse = _itemRepository.Table.FirstOrDefault(x => x.Id == ctx.WareHouseItemId);
                Unit itemUnit = null;
                WareHouseItemCategory itemCategory = null;

                if (itemWareHouse != null)
                {
                    itemUnit = _unitRepository.Table.FirstOrDefault(x => x.Id == itemWareHouse.UnitId);
                    itemCategory = _categoryRepository.Table.FirstOrDefault(x => x.Id == itemWareHouse.CategoryID);
                }

                results.ForEach(x =>
                {
                    x.WareHouseItemCode = itemWareHouse.Code;
                    x.WareHouseItemName = itemWareHouse.Name;
                    x.UnitName = itemUnit?.UnitName;
                    x.Category = itemCategory?.Name;
                    if (lastItem == null)
                    {
                        x.Balance = x.Beginning + x.Import - x.Export;
                        lastItem = x;
                    }
                    else
                    {
                        x.Beginning = lastItem.Balance;
                        x.Balance = x.Beginning + x.Import - x.Export;
                        lastItem = x;
                    }
                });
            }

            return new PagedList<ReportValueModel>(results, ctx.PageIndex, ctx.PageSize);
        }

        private void UpdateOutwardToResults(
            IQueryable<OutwardDetail> outwardDetails,
            List<ReportValueModel> results,
            ReportValueSearchContext ctx,
            Outward x,
            string strPurpose)
        {
            var firstAssign = false;
            if (outwardDetails?.ToList()?.Count > 0)
            {
                outwardDetails.ToList().ForEach(item =>
                {
                    if (!firstAssign)
                    {
                        var row = results.FirstOrDefault();
                        if (row?.Moment == x.VoucherDate && row?.VoucherCodeExport == x.VoucherCode)
                        {
                            row.Export = item.Quantity;
                            row.DepartmentName = item.DepartmentName;
                            row.Purpose = strPurpose;
                            row.ProjectName = item.ProjectName;
                            row.Description = x.Description;
                        }
                        else
                        {
                            results.Add(new ReportValueModel
                            {
                                Moment = x.VoucherDate,
                                WareHouseId = x.WareHouseID,
                                WareHouseItemId = ctx.WareHouseItemId,
                                Export = item.Quantity,
                                VoucherCodeExport = x.VoucherCode,
                                UserId = x.CreatedBy,
                                ProjectName = item.ProjectName,
                                DepartmentName = item.DepartmentName,
                                Description = x.Description,
                                Purpose = strPurpose
                            });
                        }

                        firstAssign = true;
                    }
                    else
                    {
                        results.Add(new ReportValueModel
                        {
                            Moment = x.VoucherDate,
                            WareHouseId = x.WareHouseID,
                            WareHouseItemId = ctx.WareHouseItemId,
                            Export = item.Quantity,
                            VoucherCodeExport = x.VoucherCode,
                            UserId = x.CreatedBy,
                            ProjectName = item.ProjectName,
                            DepartmentName = item.DepartmentName,
                            Description = x.Description,
                            Purpose = strPurpose
                        });
                    }
                });
            }
        }

        private void UpdateInwardToResults(
            IQueryable<InwardDetail> inwardDetails,
            List<ReportValueModel> results,
            ReportValueSearchContext ctx,
            Inward x,
            string strPurpose)
        {
            var firstAssign = false;
            if (inwardDetails?.ToList()?.Count > 0)
            {
                inwardDetails.ToList().ForEach(item =>
                {
                    if (!firstAssign)
                    {
                        var row = results.FirstOrDefault();
                        if (row?.Moment == x.VoucherDate && row?.VoucherCodeImport == x.VoucherCode)
                        {
                            row.Import = item.Quantity;
                            row.DepartmentName = item.DepartmentName;
                            row.Purpose = strPurpose;
                            row.ProjectName = item.ProjectName;
                            row.Description = x.Description;
                        }
                        else
                        {
                            results.Add(new ReportValueModel
                            {
                                Moment = x.VoucherDate,
                                WareHouseId = x.WareHouseID,
                                WareHouseItemId = ctx.WareHouseItemId,
                                Import = item.Quantity,
                                VoucherCodeImport = x.VoucherCode,
                                UserId = x.CreatedBy,
                                ProjectName = item.ProjectName,
                                DepartmentName = item.DepartmentName,
                                Description = x.Description,
                                Purpose = strPurpose
                            });
                        }

                        firstAssign = true;
                    }
                    else
                    {
                        results.Add(new ReportValueModel
                        {
                            Moment = x.VoucherDate,
                            WareHouseId = x.WareHouseID,
                            WareHouseItemId = ctx.WareHouseItemId,
                            Import = item.Quantity,
                            VoucherCodeImport = x.VoucherCode,
                            UserId = x.CreatedBy,
                            ProjectName = item.ProjectName,
                            DepartmentName = item.DepartmentName,
                            Description = x.Description,
                            Purpose = strPurpose
                        });
                    }
                });
            }
        }

        private void CalculationFirstVoucher(
            List<ReportValueModel> results,
            ReportValueSearchContext ctx,
            Inward firstInwardVoucher,
            Outward firstOutwardVoucher,
            BeginningWareHouse beginItem)
        {
            decimal totalImport = 0;
            decimal totalExport = 0;

            if (firstInwardVoucher != null)
            {
                var inwardsFirstTime = (from i in _inwardRepository.Table
                                        join iwd in _inwardDetailRepository.Table on i.Id equals iwd.InwardId into iwdi
                                        from idwi in iwdi.DefaultIfEmpty()
                                        where idwi != null &&
                                              idwi.ItemId == ctx.WareHouseItemId &&
                                              i.VoucherDate >= new DateTime(1990, 01, 01) &&
                                              i.VoucherDate < firstInwardVoucher.VoucherDate &&
                                              i.WareHouseID == firstInwardVoucher.WareHouseID
                                        orderby i.VoucherDate
                                        select i).Distinct();

                var outwardsFirstTime = (from o in _outwardRepository.Table
                                         join owd in _outwardDetailRepository.Table on o.Id equals owd.OutwardId into owdo
                                         from odwo in owdo.DefaultIfEmpty()
                                         where odwo != null &&
                                               odwo.ItemId == ctx.WareHouseItemId &&
                                               o.VoucherDate >= new DateTime(1990, 01, 01) &&
                                               o.VoucherDate < firstInwardVoucher.VoucherDate &&
                                               o.WareHouseID == firstInwardVoucher.WareHouseID
                                         orderby o.VoucherDate
                                         select o).Distinct();

                CalculationTotalImportExportFirstVoucher(out totalImport, out totalExport, ctx, inwardsFirstTime,
                    outwardsFirstTime);

                results.Add(
                    new ReportValueModel
                    {
                        Moment = firstInwardVoucher.VoucherDate,
                        WareHouseId = firstInwardVoucher.WareHouseID,
                        WareHouseItemId = ctx.WareHouseItemId,
                        Beginning = beginItem == null ? 0 : beginItem.Quantity + totalImport - totalExport,
                        VoucherCodeImport = firstInwardVoucher.VoucherCode,
                        UserId = firstInwardVoucher.CreatedBy
                    }
                );
            }
            else if (firstOutwardVoucher != null)
            {
                var inwardsFirstTime = (from i in _inwardRepository.Table
                                        join iwd in _inwardDetailRepository.Table on i.Id equals iwd.InwardId into iwdi
                                        from idwi in iwdi.DefaultIfEmpty()
                                        where idwi != null &&
                                              idwi.ItemId == ctx.WareHouseItemId &&
                                              i.VoucherDate >= new DateTime(1990, 01, 01) &&
                                              i.VoucherDate < firstOutwardVoucher.VoucherDate &&
                                              i.WareHouseID == firstOutwardVoucher.WareHouseID
                                        orderby i.VoucherDate
                                        select i).Distinct();

                var outwardsFirstTime = (from o in _outwardRepository.Table
                                         join owd in _outwardDetailRepository.Table on o.Id equals owd.OutwardId into owdo
                                         from odwo in owdo.DefaultIfEmpty()
                                         where odwo != null &&
                                               odwo.ItemId == ctx.WareHouseItemId &&
                                               o.VoucherDate >= new DateTime(1990, 01, 01) &&
                                               o.VoucherDate < firstOutwardVoucher.VoucherDate &&
                                               o.WareHouseID == firstOutwardVoucher.WareHouseID
                                         orderby o.VoucherDate
                                         select o).Distinct();

                CalculationTotalImportExportFirstVoucher(out totalImport, out totalExport, ctx, inwardsFirstTime,
                    outwardsFirstTime);

                results.Add(
                    new ReportValueModel
                    {
                        Moment = firstOutwardVoucher.VoucherDate,
                        WareHouseId = firstOutwardVoucher.WareHouseID,
                        WareHouseItemId = ctx.WareHouseItemId,
                        Beginning = beginItem == null ? 0 : beginItem.Quantity + totalImport - totalExport,
                        VoucherCodeExport = firstOutwardVoucher.VoucherCode,
                        UserId = firstOutwardVoucher.CreatedBy
                    }
                );
            }
        }

        private void CalculationTotalImportExportFirstVoucher(
            out decimal totalImport, out decimal totalExport,
            ReportValueSearchContext ctx,
            IQueryable<Inward> inwardsFirstTime,
            IQueryable<Outward> outwardsFirstTime)
        {
            if (string.IsNullOrEmpty(ctx.DepartmentId) && string.IsNullOrEmpty(ctx.ProjectId))
            {
                totalImport = (from i in inwardsFirstTime
                               join iwd in _inwardDetailRepository.Table on i.Id equals iwd.InwardId into iwdi
                               from idwi in iwdi.DefaultIfEmpty()
                               where idwi.ItemId == ctx.WareHouseItemId
                               select idwi.Quantity).Sum();

                totalExport = (from o in outwardsFirstTime
                               join owd in _outwardDetailRepository.Table on o.Id equals owd.OutwardId into owdo
                               from odwo in owdo.DefaultIfEmpty()
                               where odwo.ItemId == ctx.WareHouseItemId
                               select odwo.Quantity).Sum();
            }
            else if (!string.IsNullOrEmpty(ctx.ProjectId) && string.IsNullOrEmpty(ctx.DepartmentId))
            {
                totalImport = (from i in inwardsFirstTime
                               join iwd in _inwardDetailRepository.Table on i.Id equals iwd.InwardId into iwdi
                               from idwi in iwdi.DefaultIfEmpty()
                               where idwi.ItemId == ctx.WareHouseItemId &&
                                     idwi.ProjectId == ctx.ProjectId
                               select idwi.Quantity).Sum();

                totalExport = (from o in outwardsFirstTime
                               join owd in _outwardDetailRepository.Table on o.Id equals owd.OutwardId into owdo
                               from odwo in owdo.DefaultIfEmpty()
                               where odwo.ItemId == ctx.WareHouseItemId &&
                                     odwo.ProjectId == ctx.ProjectId
                               select odwo.Quantity).Sum();
            }
            else if (!string.IsNullOrEmpty(ctx.DepartmentId) && string.IsNullOrEmpty(ctx.ProjectId))
            {
                totalImport = (from i in inwardsFirstTime
                               join iwd in _inwardDetailRepository.Table on i.Id equals iwd.InwardId into iwdi
                               from idwi in iwdi.DefaultIfEmpty()
                               where idwi.ItemId == ctx.WareHouseItemId &&
                                     idwi.DepartmentId == ctx.DepartmentId
                               select idwi.Quantity).Sum();

                totalExport = (from o in outwardsFirstTime
                               join owd in _outwardDetailRepository.Table on o.Id equals owd.OutwardId into owdo
                               from odwo in owdo.DefaultIfEmpty()
                               where odwo.ItemId == ctx.WareHouseItemId &&
                                     odwo.DepartmentId == ctx.DepartmentId
                               select odwo.Quantity).Sum();
            }
            else
            {
                totalImport = (from i in inwardsFirstTime
                               join iwd in _inwardDetailRepository.Table on i.Id equals iwd.InwardId into iwdi
                               from idwi in iwdi.DefaultIfEmpty()
                               where idwi.ItemId == ctx.WareHouseItemId &&
                                     idwi.DepartmentId == ctx.DepartmentId &&
                                     idwi.ProjectId == ctx.ProjectId
                               select idwi.Quantity).Sum();

                totalExport = (from o in outwardsFirstTime
                               join owd in _outwardDetailRepository.Table on o.Id equals owd.OutwardId into owdo
                               from odwo in owdo.DefaultIfEmpty()
                               where odwo.ItemId == ctx.WareHouseItemId &&
                                     odwo.DepartmentId == ctx.DepartmentId &&
                                     odwo.ProjectId == ctx.ProjectId
                               select odwo.Quantity).Sum();
            }
        }

        private async Task<IPagedList<ReportValueModel>> GetReportTotalAsync(ReportValueSearchContext ctx)
        {
            if (string.IsNullOrEmpty(ctx.WareHouseId))
            {
                return null;
            }

            var result = await SearchReportTotal(ctx, true);
            return result;
        }

        private async Task<IPagedList<ReportValueModel>> SearchReportTotal(ReportValueSearchContext ctx, bool limit)
        {
            var check = false;
            var list = new StringBuilder();
            list.Append(" ( '" + ctx.WareHouseId + "'");
            var department = _wareHouseRepository.Table.FirstOrDefault(x => x.Id == ctx.WareHouseId);

            if (department != null)
            {
                StringBuilder GetListChidren = new StringBuilder();

                GetListChidren.Append("with recursive cte (Id, Name, ParentId) as ( ");
                GetListChidren.Append("  select     wh.Id, ");
                GetListChidren.Append("             wh.Name, ");
                GetListChidren.Append("             wh.ParentId ");
                GetListChidren.Append("  from       WareHouse wh ");
                GetListChidren.Append("  where      wh.ParentId='" + ctx.WareHouseId + "' ");
                GetListChidren.Append("  union all ");
                GetListChidren.Append("  SELECT     p.Id, ");
                GetListChidren.Append("             p.Name, ");
                GetListChidren.Append("             p.ParentId ");
                GetListChidren.Append("  from       WareHouse  p ");
                GetListChidren.Append("  inner join cte ");
                GetListChidren.Append("          on p.ParentId = cte.id ");
                GetListChidren.Append(") ");
                GetListChidren.Append(" select * FROM cte GROUP BY cte.Id,cte.Name,cte.ParentId; ");
                var departmentIds =
                    await _inwardRepository.DataConnection.QueryToListAsync<WareHouse>(GetListChidren.ToString());
                if (departmentIds?.ToList().Count > 0)
                {
                    list.Append(",");
                    check = true;
                    var listDepartmentIds = departmentIds.ToList();
                    for (int i = 0; i < listDepartmentIds.Count; i++)
                    {
                        if (i == listDepartmentIds.Count - 1)
                            list.Append("'" + listDepartmentIds[i].Id + "'");
                        else
                            list.Append("'" + listDepartmentIds[i].Id + "'" + ", ");
                    }
                }
            }

            list.Append(" ) ");

            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("  whi.Code as WareHouseItemCode, ");
            sb.Append("  whi.Name as WareHouseItemName, ");
            sb.Append("  (SELECT ");
            sb.Append("      CASE WHEN SUM(whl.Quantity) IS NULL THEN 0 ELSE SUM(whl.Quantity) END ");
            sb.Append("    FROM vWareHouseLedger whl ");
            sb.Append("    WHERE whl.WareHouseId in ");
            sb.Append(list);
            sb.Append("    AND whl.ItemId = whi.Id ");
            sb.Append("    AND whl.VoucherDate < @pFrom) AS Beginning, ");
            sb.Append("  (SELECT ");
            sb.Append("      CASE WHEN SUM(Id.Quantity) IS NULL THEN 0 ELSE SUM(Id.Quantity) END ");
            sb.Append("    FROM Inward i ");
            sb.Append("      INNER JOIN InwardDetail Id ");
            sb.Append("        ON i.Id = Id.InwardId ");
            sb.Append("    WHERE i.VoucherDate BETWEEN @pFrom AND @pTo ");
            sb.Append("    AND i.WareHouseId in  ");
            sb.Append(list);
            if (!string.IsNullOrEmpty(ctx.WareHouseItemId))
                sb.Append("    AND Id.ItemId = @pWareHouseItemId ");
            sb.Append("    AND Id.ItemId = whi.Id) AS Import, ");
            sb.Append("  (SELECT ");
            sb.Append("      CASE WHEN SUM(od.Quantity) IS NULL THEN 0 ELSE SUM(od.Quantity) END ");
            sb.Append("    FROM Outward o ");
            sb.Append("      INNER JOIN OutwardDetail od ");
            sb.Append("        ON o.Id = od.OutwardId ");
            sb.Append("    WHERE o.VoucherDate BETWEEN @pFrom AND @pTo ");
            sb.Append("    AND o.WareHouseId in  ");
            sb.Append(list);
            if (!string.IsNullOrEmpty(ctx.WareHouseItemId))
                sb.Append("    AND od.ItemId = @pWareHouseItemId ");
            sb.Append("    AND od.ItemId = whi.Id) AS Export, ");
            sb.Append("  (SELECT ");
            sb.Append("      CASE WHEN SUM(whl.Quantity) IS NULL THEN 0 ELSE SUM(whl.Quantity) END ");
            sb.Append("    FROM vWareHouseLedger whl ");
            sb.Append("    WHERE whl.WareHouseId in ");
            sb.Append(list);
            sb.Append("    AND whl.ItemId = whi.Id ");
            sb.Append("    AND whl.VoucherDate < @pFrom) + (SELECT ");
            sb.Append("      CASE WHEN SUM(Id.Quantity) IS NULL THEN 0 ELSE SUM(Id.Quantity) END ");
            sb.Append("    FROM Inward i ");
            sb.Append("      INNER JOIN InwardDetail Id ");
            sb.Append("        ON i.Id = Id.InwardId ");
            sb.Append("    WHERE i.VoucherDate BETWEEN @pFrom AND @pTo ");
            sb.Append("    AND i.WareHouseId in  ");
            sb.Append(list);
            if (!string.IsNullOrEmpty(ctx.WareHouseItemId))
                sb.Append("    AND Id.ItemId = @pWareHouseItemId ");
            sb.Append("    AND Id.ItemId = whi.Id) - (SELECT ");
            sb.Append("      CASE WHEN SUM(od.Quantity) IS NULL THEN 0 ELSE SUM(od.Quantity) END ");
            sb.Append("    FROM Outward o ");
            sb.Append("      INNER JOIN OutwardDetail od ");
            sb.Append("        ON o.Id = od.OutwardId ");
            sb.Append("    WHERE o.VoucherDate BETWEEN @pFrom  AND @pTo ");
            sb.Append("    AND o.WareHouseId  in  ");
            sb.Append(list);
            if (!string.IsNullOrEmpty(ctx.WareHouseItemId))
                sb.Append("    AND od.ItemId = @pWareHouseItemId ");
            sb.Append("    AND od.ItemId = whi.Id) AS Balance, ");
            sb.Append("  u.UnitName ");
            sb.Append("FROM WareHouseItem whi ");
            sb.Append("  INNER JOIN Unit u ");
            sb.Append("    ON whi.UnitId = u.Id INNER JOIN vWareHouseLedger whl ON whi.Id = whl.ItemId  ");
            sb.Append("  WHERE whl.WareHouseId in  ");
            sb.Append(list);
            if (!string.IsNullOrEmpty(ctx.WareHouseItemId))
                sb.Append(" and whi.Id = @pWareHouseItemId ");
            sb.Append("GROUP BY whi.Id, ");
            sb.Append("         whi.Name, ");
            sb.Append("         u.UnitName ");
            sb.Append("ORDER BY whi.Name ");
            if (limit)
                sb.Append(" LIMIT @p2 OFFSET @p3 ");
            //count


            StringBuilder sbCount = new StringBuilder();

            sbCount.Append("SELECT COUNT(*) FROM ( ");
            sbCount.Append("SELECT ");
            sbCount.Append("  whi.Code as WareHouseItemCode ");
            sbCount.Append("FROM WareHouseItem whi ");
            sbCount.Append("  INNER JOIN Unit u ");
            sbCount.Append("    ON whi.UnitId = u.Id INNER JOIN vWareHouseLedger whl ON whi.Id = whl.ItemId  ");
            sbCount.Append("  WHERE whl.WareHouseId= @pWareHouseId ");
            if (!string.IsNullOrEmpty(ctx.WareHouseItemId))
                sbCount.Append("and whi.Id = @pWareHouseItemId ");
            sbCount.Append("GROUP BY whi.Id, ");
            sbCount.Append("         whi.Name ");
            sbCount.Append("              ) t   ");
            sbCount.Append("  ");


            var from = ctx.FromDate.ToUniversalTime();
            var to = ctx.ToDate.ToUniversalTime();
            DataParameter p2 = new DataParameter("p2", ctx.PageSize);
            DataParameter p3 = new DataParameter("p3", ctx.PageIndex * ctx.PageSize);

            DataParameter pWareHouseId = new DataParameter("pWareHouseId", ctx.WareHouseId);
            DataParameter pWareHouseItemId = new DataParameter("pWareHouseItemId", ctx.WareHouseItemId);
            DataParameter pFrom = new DataParameter("pFrom",
                "" + from.Value.Year + "-" + from.Value.Month + "-" + from.Value.Day + "  12:0:0 AM  ");
            DataParameter pTo = new DataParameter("pTo",
                "" + to.Value.Year + "-" + to.Value.Month + "-" + to.Value.Day + "  12:0:0 AM  ");
            var result = await _beginningRepository.DataConnection.QueryToListAsync<ReportValueModel>(sb.ToString(),
                pWareHouseId, pWareHouseItemId, pFrom, pTo, p2, p3);
            var resCount = await _beginningRepository.DataConnection.QueryToListAsync<int>(sbCount.ToString(),
                pWareHouseId, pWareHouseItemId, pFrom, pTo);

            return new PagedList<ReportValueModel>(result, ctx.PageIndex, ctx.PageSize, resCount.FirstOrDefault());
        }

        private async Task<IList<ReportValueModel>> GetReportExcelTotal(ReportValueSearchContext ctx)
        {
            if (string.IsNullOrEmpty(ctx.WareHouseId))
            {
                return null;
            }

            var results = new List<ReportValueModel>();
            var list = new StringBuilder();
            list.Append(" ( '" + ctx.WareHouseId + "'");
            var department = _wareHouseRepository.Table.FirstOrDefault(x => x.Id == ctx.WareHouseId);

            if (department != null)
            {
                StringBuilder GetListChidren = new StringBuilder();

                GetListChidren.Append("with recursive cte (Id, Name, ParentId) as ( ");
                GetListChidren.Append("  select     wh.Id, ");
                GetListChidren.Append("             wh.Name, ");
                GetListChidren.Append("             wh.ParentId ");
                GetListChidren.Append("  from       WareHouse wh ");
                GetListChidren.Append("  where      wh.ParentId='" + ctx.WareHouseId + "' ");
                GetListChidren.Append("  union all ");
                GetListChidren.Append("  SELECT     p.Id, ");
                GetListChidren.Append("             p.Name, ");
                GetListChidren.Append("             p.ParentId ");
                GetListChidren.Append("  from       WareHouse  p ");
                GetListChidren.Append("  inner join cte ");
                GetListChidren.Append("          on p.ParentId = cte.id ");
                GetListChidren.Append(") ");
                GetListChidren.Append(" select * FROM cte GROUP BY cte.Id,cte.Name,cte.ParentId; ");
                var departmentIds =
                    await _inwardRepository.DataConnection.QueryToListAsync<WareHouse>(GetListChidren.ToString());
                if (departmentIds?.ToList().Count > 0)
                {
                    list.Append(",");
                    var listDepartmentIds = departmentIds.ToList();
                    for (int i = 0; i < listDepartmentIds.Count; i++)
                    {
                        if (i == listDepartmentIds.Count - 1)
                            list.Append("'" + listDepartmentIds[i].Id + "'");
                        else
                            list.Append("'" + listDepartmentIds[i].Id + "'" + ", ");
                    }
                }
            }

            list.Append(" ) ");

            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("  whi.Code as WareHouseItemCode, ");
            sb.Append("  whi.Name as WareHouseItemName, ");
            sb.Append("  (SELECT ");
            sb.Append("      CASE WHEN SUM(whl.Quantity) IS NULL THEN 0 ELSE SUM(whl.Quantity) END ");
            sb.Append("    FROM vWareHouseLedger whl ");
            sb.Append("    WHERE whl.WareHouseId = @pWareHouseId ");
            sb.Append("    AND whl.ItemId = whi.Id ");
            sb.Append("    AND whl.VoucherDate < @pFrom) AS Beginning, ");
            sb.Append("  (SELECT ");
            sb.Append("      CASE WHEN SUM(Id.Quantity) IS NULL THEN 0 ELSE SUM(Id.Quantity) END ");
            sb.Append("    FROM Inward i ");
            sb.Append("      INNER JOIN InwardDetail Id ");
            sb.Append("        ON i.Id = Id.InwardId ");
            sb.Append("    WHERE i.VoucherDate BETWEEN @pFrom AND @pTo ");
            sb.Append("    AND i.WareHouseId in  ");
            sb.Append(list);
            if (!string.IsNullOrEmpty(ctx.WareHouseItemId))
                sb.Append("    AND Id.ItemId = @pWareHouseItemId ");
            sb.Append("    AND Id.ItemId = whi.Id) AS Import, ");
            sb.Append("  (SELECT ");
            sb.Append("      CASE WHEN SUM(od.Quantity) IS NULL THEN 0 ELSE SUM(od.Quantity) END ");
            sb.Append("    FROM Outward o ");
            sb.Append("      INNER JOIN OutwardDetail od ");
            sb.Append("        ON o.Id = od.OutwardId ");
            sb.Append("    WHERE o.VoucherDate BETWEEN @pFrom AND @pTo ");
            sb.Append("    AND o.WareHouseId in  ");
            sb.Append(list);
            if (!string.IsNullOrEmpty(ctx.WareHouseItemId))
                sb.Append("    AND od.ItemId = @pWareHouseItemId ");
            sb.Append("    AND od.ItemId = whi.Id) AS Export, ");
            sb.Append("  (SELECT ");
            sb.Append("      CASE WHEN SUM(whl.Quantity) IS NULL THEN 0 ELSE SUM(whl.Quantity) END ");
            sb.Append("    FROM vWareHouseLedger whl ");
            sb.Append("    WHERE whl.WareHouseId = @pWareHouseId ");
            sb.Append("    AND whl.ItemId = whi.Id ");
            sb.Append("    AND whl.VoucherDate < @pFrom) + (SELECT ");
            sb.Append("      CASE WHEN SUM(Id.Quantity) IS NULL THEN 0 ELSE SUM(Id.Quantity) END ");
            sb.Append("    FROM Inward i ");
            sb.Append("      INNER JOIN InwardDetail Id ");
            sb.Append("        ON i.Id = Id.InwardId ");
            sb.Append("    WHERE i.VoucherDate BETWEEN @pFrom AND @pTo ");
            sb.Append("    AND i.WareHouseId in  ");
            sb.Append(list);
            if (!string.IsNullOrEmpty(ctx.WareHouseItemId))
                sb.Append("    AND Id.ItemId = @pWareHouseItemId ");
            sb.Append("    AND Id.ItemId = whi.Id) - (SELECT ");
            sb.Append("      CASE WHEN SUM(od.Quantity) IS NULL THEN 0 ELSE SUM(od.Quantity) END ");
            sb.Append("    FROM Outward o ");
            sb.Append("      INNER JOIN OutwardDetail od ");
            sb.Append("        ON o.Id = od.OutwardId ");
            sb.Append("    WHERE o.VoucherDate BETWEEN @pFrom  AND @pTo ");
            sb.Append("    AND o.WareHouseId  in  ");
            sb.Append(list);
            if (!string.IsNullOrEmpty(ctx.WareHouseItemId))
                sb.Append("    AND od.ItemId = @pWareHouseItemId ");
            sb.Append("    AND od.ItemId = whi.Id) AS Balance, ");
            sb.Append("  u.UnitName ");
            sb.Append("FROM WareHouseItem whi ");
            sb.Append("  INNER JOIN Unit u ");
            sb.Append("    ON whi.UnitId = u.Id INNER JOIN vWareHouseLedger whl ON whi.Id = whl.ItemId  ");
            sb.Append("  WHERE whl.WareHouseId in  ");
            sb.Append(list);
            if (!string.IsNullOrEmpty(ctx.WareHouseItemId))
                sb.Append(" and whi.Id = @pWareHouseItemId ");
            sb.Append("GROUP BY whi.Id, ");
            sb.Append("         whi.Name, ");
            sb.Append("         u.UnitName ");
            sb.Append("ORDER BY whi.Name ");


            var from = ctx.FromDate.ToUniversalTime();
            var to = ctx.ToDate.ToUniversalTime();

            DataParameter pWareHouseId = new DataParameter("pWareHouseId", ctx.WareHouseId);
            DataParameter pWareHouseItemId = new DataParameter("pWareHouseItemId", ctx.WareHouseItemId);
            DataParameter pFrom = new DataParameter("pFrom",
                "" + from.Value.Year + "-" + from.Value.Month + "-" + from.Value.Day + " 12:0:0 AM   ");
            DataParameter pTo = new DataParameter("pTo",
                "" + to.Value.Year + "-" + to.Value.Month + "-" + to.Value.Day + "  12:0:0 AM   ");
            var result =
                await _beginningRepository.DataConnection.QueryToListAsync<ReportValueModel>(sb.ToString(),
                    pWareHouseId, pWareHouseItemId, pFrom, pTo);

            foreach (var item in result)
            {
                results.Add(item);
            }

            return results;
        }

        private IList<ReportValueModel> GetReportExcelDetail(ReportValueSearchContext ctx)
        {
            if (string.IsNullOrEmpty(ctx.WareHouseId) ||
                string.IsNullOrEmpty(ctx.WareHouseItemId) ||
                ctx.FromDate == null ||
                ctx.ToDate == null)
            {
                return null;
            }

            var results = new List<ReportValueModel>();
            var listWarehouseIds = new List<string>();

            var warehouse = _wareHouseRepository.Table.FirstOrDefault(x => x.Id == ctx.WareHouseId);

            if (!string.IsNullOrEmpty(warehouse.Path))
            {
                var warehouseIds = _wareHouseRepository.Table.Where(x => !string.IsNullOrEmpty(x.Path) &&
                                                                        x.Path.Contains(warehouse.Path))
                                                             .Select(x => x.Id);
                if (warehouseIds?.ToList().Count > 0)
                {
                    listWarehouseIds = warehouseIds.ToList();
                }
            }

            var inwardsOnTime = (from i in _inwardRepository.Table
                                 join iwd in _inwardDetailRepository.Table on i.Id equals iwd.InwardId into iwdi
                                 from idwi in iwdi.DefaultIfEmpty()
                                 where idwi != null &&
                                       idwi.ItemId == ctx.WareHouseItemId &&
                                       i.VoucherDate >= ctx.FromDate &&
                                       i.VoucherDate <= ctx.ToDate &&
                                       listWarehouseIds.Contains(i.WareHouseID)
                                 orderby i.VoucherDate
                                 select i).Distinct();

            var outwardsOnTime = (from o in _outwardRepository.Table
                                  join owd in _outwardDetailRepository.Table on o.Id equals owd.OutwardId into owdo
                                  from odwo in owdo.DefaultIfEmpty()
                                  where odwo != null &&
                                        odwo.ItemId == ctx.WareHouseItemId &&
                                        o.VoucherDate >= ctx.FromDate &&
                                        o.VoucherDate <= ctx.ToDate &&
                                        listWarehouseIds.Contains(o.WareHouseID)
                                  orderby o.VoucherDate
                                  select o).Distinct();

            var firstInwardVoucher = inwardsOnTime?.FirstOrDefault();
            var firstOutwardVoucher = outwardsOnTime?.FirstOrDefault();

            if (firstInwardVoucher != null && firstOutwardVoucher != null)
            {
                if (firstInwardVoucher.VoucherDate <= firstOutwardVoucher.VoucherDate)
                {
                    var beginItem = _beginningRepository.Table.FirstOrDefault(x => x.WareHouseId == firstInwardVoucher.WareHouseID &&
                                                                                   x.ItemId == ctx.WareHouseItemId);
                    CalculationFirstVoucher(results, ctx, firstInwardVoucher, null, beginItem);
                    if (results.Count == 0)
                    {
                        results.Add(
                            new ReportValueModel
                            {
                                Moment = firstInwardVoucher.VoucherDate,
                                WareHouseId = firstInwardVoucher.WareHouseID,
                                WareHouseItemId = ctx.WareHouseItemId,
                                Beginning = beginItem == null ? 0 : beginItem.Quantity,
                                VoucherCodeImport = firstInwardVoucher.VoucherCode,
                                UserId = firstInwardVoucher.CreatedBy
                            }
                        );
                    }
                }
                else
                {
                    var beginItem = _beginningRepository.Table.FirstOrDefault(x => x.WareHouseId == firstOutwardVoucher.WareHouseID &&
                                                                                   x.ItemId == ctx.WareHouseItemId);
                    CalculationFirstVoucher(results, ctx, null, firstOutwardVoucher, beginItem);
                    if (results.Count == 0)
                    {
                        results.Add(
                            new ReportValueModel
                            {
                                Moment = firstOutwardVoucher.VoucherDate,
                                WareHouseId = firstOutwardVoucher.WareHouseID,
                                WareHouseItemId = ctx.WareHouseItemId,
                                Beginning = beginItem == null ? 0 : beginItem.Quantity,
                                VoucherCodeExport = firstOutwardVoucher.VoucherCode,
                                UserId = firstOutwardVoucher.CreatedBy
                            }
                        );
                    }
                }
            }
            else if (firstInwardVoucher != null && firstOutwardVoucher == null)
            {
                var beginItem = _beginningRepository.Table.FirstOrDefault(x => x.WareHouseId == firstInwardVoucher.WareHouseID &&
                                                                               x.ItemId == ctx.WareHouseItemId);
                CalculationFirstVoucher(results, ctx, firstInwardVoucher, null, beginItem);
                if (results.Count == 0)
                {
                    results.Add(
                        new ReportValueModel
                        {
                            Moment = firstInwardVoucher.VoucherDate,
                            WareHouseId = firstInwardVoucher.WareHouseID,
                            WareHouseItemId = ctx.WareHouseItemId,
                            Beginning = beginItem == null ? 0 : beginItem.Quantity,
                            VoucherCodeImport = firstInwardVoucher.VoucherCode,
                            UserId = firstInwardVoucher.CreatedBy
                        }
                    );
                }
            }
            else if (firstOutwardVoucher != null && firstInwardVoucher == null)
            {
                var beginItem = _beginningRepository.Table.FirstOrDefault(x => x.WareHouseId == firstOutwardVoucher.WareHouseID &&
                                                                               x.ItemId == ctx.WareHouseItemId);
                CalculationFirstVoucher(results, ctx, null, firstOutwardVoucher, beginItem);
                if (results.Count == 0)
                {
                    results.Add(
                        new ReportValueModel
                        {
                            Moment = firstOutwardVoucher.VoucherDate,
                            WareHouseId = firstOutwardVoucher.WareHouseID,
                            WareHouseItemId = ctx.WareHouseItemId,
                            Beginning = beginItem == null ? 0 : beginItem.Quantity,
                            VoucherCodeExport = firstOutwardVoucher.VoucherCode,
                            UserId = firstOutwardVoucher.CreatedBy
                        }
                    );
                }
            }
            else
            {
                if (listWarehouseIds?.Count > 0)
                {
                    listWarehouseIds.ForEach(warehouseId =>
                    {
                        var beginItem = _beginningRepository.Table.FirstOrDefault(x => x.WareHouseId == warehouseId &&
                                                                                       x.ItemId == ctx.WareHouseItemId);
                        var beginTime = new DateTime(1990, 01, 01);
                        var totalImportPrimitive = (from i in _inwardRepository.Table
                                                    join iwd in _inwardDetailRepository.Table on i.Id equals iwd.InwardId into iwdi
                                                    from idwi in iwdi.DefaultIfEmpty()
                                                    where idwi != null &&
                                                          idwi.ItemId == ctx.WareHouseItemId &&
                                                          i.WareHouseID == warehouseId &&
                                                          i.VoucherDate >= beginTime &&
                                                          i.VoucherDate < ctx.FromDate
                                                    select idwi.Quantity).Sum();
                        var totalExportPrimitive = (from o in _outwardRepository.Table
                                                    join owd in _outwardDetailRepository.Table on o.Id equals owd.OutwardId into owdo
                                                    from odwo in owdo.DefaultIfEmpty()
                                                    where odwo != null &&
                                                          odwo.ItemId == ctx.WareHouseItemId &&
                                                          o.WareHouseID == warehouseId &&
                                                          o.VoucherDate >= beginTime &&
                                                          o.VoucherDate < ctx.FromDate
                                                    select odwo.Quantity).Sum();
                        results.Add(
                                new ReportValueModel
                                {
                                    Moment = (DateTime)ctx.FromDate,
                                    WareHouseId = warehouseId,
                                    WareHouseItemId = ctx.WareHouseItemId,
                                    Beginning = beginItem == null ? 0 : beginItem.Quantity + totalImportPrimitive - totalExportPrimitive
                                }
                            );
                    });
                }
            }

            if (results.Count > 0)
            {
                var inwardsOnTimeList = inwardsOnTime?.ToList();
                inwardsOnTimeList?.ForEach(x =>
                {
                    var inwardReasons = Enum.GetValues(typeof(InwardReason));
                    var strPurpose = "";
                    foreach (InwardReason rea in inwardReasons)
                    {
                        if (x.Reason == (int)rea)
                        {
                            strPurpose = rea.GetEnumDescription();
                        }
                    }

                    if (string.IsNullOrEmpty(ctx.DepartmentId) && string.IsNullOrEmpty(ctx.ProjectId))
                    {
                        var inwardDetails = _inwardDetailRepository.Table.Where(iwd => iwd.InwardId == x.Id &&
                            iwd.ItemId == ctx.WareHouseItemId);
                        UpdateInwardToResults(inwardDetails, results, ctx, x, strPurpose);
                    }
                    else if (!string.IsNullOrEmpty(ctx.DepartmentId) && string.IsNullOrEmpty(ctx.ProjectId))
                    {
                        var inwardDetails = _inwardDetailRepository.Table.Where(iwd => iwd.InwardId == x.Id &&
                            iwd.DepartmentId == ctx.DepartmentId &&
                            iwd.ItemId == ctx.WareHouseItemId);
                        UpdateInwardToResults(inwardDetails, results, ctx, x, strPurpose);
                    }
                    else if (!string.IsNullOrEmpty(ctx.ProjectId) && string.IsNullOrEmpty(ctx.DepartmentId))
                    {
                        var inwardDetails = _inwardDetailRepository.Table.Where(iwd => iwd.InwardId == x.Id &&
                            iwd.ProjectId == ctx.ProjectId &&
                            iwd.ItemId == ctx.WareHouseItemId);
                        UpdateInwardToResults(inwardDetails, results, ctx, x, strPurpose);
                    }
                    else
                    {
                        var inwardDetails = _inwardDetailRepository.Table.Where(iwd => iwd.InwardId == x.Id &&
                            iwd.DepartmentId == ctx.DepartmentId &&
                            iwd.ProjectId == ctx.ProjectId &&
                            iwd.ItemId == ctx.WareHouseItemId);
                        UpdateInwardToResults(inwardDetails, results, ctx, x, strPurpose);
                    }
                });

                var outwardsOnTimeList = outwardsOnTime?.ToList();
                outwardsOnTimeList?.ForEach(x =>
                {
                    var outwardReasons = Enum.GetValues(typeof(OutwardReason));
                    var strPurpose = "";
                    foreach (OutwardReason rea in outwardReasons)
                    {
                        if (x.Reason == (int)rea)
                        {
                            strPurpose = rea.GetEnumDescription();
                        }
                    }

                    if (string.IsNullOrEmpty(ctx.DepartmentId) && string.IsNullOrEmpty(ctx.ProjectId))
                    {
                        var outwardDetails = _outwardDetailRepository.Table.Where(owd => owd.OutwardId == x.Id &&
                            owd.ItemId == ctx.WareHouseItemId);
                        UpdateOutwardToResults(outwardDetails, results, ctx, x, strPurpose);
                    }
                    else if (!string.IsNullOrEmpty(ctx.DepartmentId) && string.IsNullOrEmpty(ctx.ProjectId))
                    {
                        var outwardDetails = _outwardDetailRepository.Table.Where(owd => owd.OutwardId == x.Id &&
                            owd.DepartmentId == ctx.DepartmentId &&
                            owd.ItemId == ctx.WareHouseItemId);
                        UpdateOutwardToResults(outwardDetails, results, ctx, x, strPurpose);
                    }
                    else if (!string.IsNullOrEmpty(ctx.ProjectId) && string.IsNullOrEmpty(ctx.DepartmentId))
                    {
                        var outwardDetails = _outwardDetailRepository.Table.Where(owd => owd.OutwardId == x.Id &&
                            owd.ProjectId == ctx.ProjectId &&
                            owd.ItemId == ctx.WareHouseItemId);
                        UpdateOutwardToResults(outwardDetails, results, ctx, x, strPurpose);
                    }
                    else
                    {
                        var outwardDetails = _outwardDetailRepository.Table.Where(owd => owd.OutwardId == x.Id &&
                            owd.ProjectId == ctx.ProjectId &&
                            owd.DepartmentId == ctx.DepartmentId &&
                            owd.ItemId == ctx.WareHouseItemId);
                        UpdateOutwardToResults(outwardDetails, results, ctx, x, strPurpose);
                    }
                });

                results.Sort();
                ReportValueModel lastItem = null;
                var itemWareHouse = _itemRepository.Table.FirstOrDefault(x => x.Id == ctx.WareHouseItemId);
                Unit itemUnit = null;
                WareHouseItemCategory itemCategory = null;

                if (itemWareHouse != null)
                {
                    itemUnit = _unitRepository.Table.FirstOrDefault(x => x.Id == itemWareHouse.UnitId);
                    itemCategory = _categoryRepository.Table.FirstOrDefault(x => x.Id == itemWareHouse.CategoryID);
                }

                results.ForEach(x =>
                {
                    x.WareHouseItemCode = itemWareHouse.Code;
                    x.WareHouseItemName = itemWareHouse.Name;
                    x.UnitName = itemUnit?.UnitName;
                    x.Category = itemCategory?.Name;
                    if (lastItem == null)
                    {
                        x.Balance = x.Beginning + x.Import - x.Export;
                        lastItem = x;
                    }
                    else
                    {
                        x.Beginning = lastItem.Balance;
                        x.Balance = x.Beginning + x.Import - x.Export;
                        lastItem = x;
                    }
                });
            }

            return results;
        }

        private async Task<IList<ReportInwardMisaModel>> GetReportInwardMisaExcelDetail(ReportInwardMisaSearchContext ctx)
        {
            var inwward = from a in _inwardRepository.Table select a;
            if (ctx.FromDate.HasValue)
            {
                inwward = from p in inwward
                          where p.VoucherDate >= ctx.FromDate
                        select p;
            }

            if (ctx.ToDate.HasValue)
            {
                inwward = from p in inwward
                          where p.VoucherDate <= ctx.ToDate
                        select p;
            }

            var query = from a in inwward
                        join b in _inwardDetailRepository.Table on a.Id equals b.InwardId
                        join d in _wareHouseItemRepository.Table on b.ItemId equals d.Id
                        join acc in _accObjectRepository.Table  on a.AccObjectId equals acc.Id into acco
                        from acc in acco.DefaultIfEmpty()
                        join c in _wareHouseRepository.Table on a.WareHouseID equals c.Id
                        join e in _vendorRepository.Table on a.VendorId equals e.Id into ps
                        from e in ps.DefaultIfEmpty()
                        join u in _unitRepository.Table on b.UnitId equals u.Id
                        select new ReportInwardMisaModel
                        {

                            Moment= a.VoucherDate.ToString("dd/MM/yyyy"),
                            WareHouseItemId = c.Code,
                            Voucher=a.Voucher,
                            VoucherDateTime = a.VoucherDate.ToString("dd/MM/yyyy"),
                            VoucherCode = a.VoucherCode,
                            VendorCode = acc.Code,
                            ProjectId = acc.Name,
                            NoteRender = a.Description,
                            WareHouseItemCode = d.Code,
                            WareHouseItemName = d.Name,
                            AccountMore = "1561",
                            AccountYes = "331",
                            UnitName = u.UnitName,
                            Amount = b.Amount,
                            Price = b.UIPrice,
                            Quantity = b.Quantity,
                            Id = b.Id,
                            DepartmentName = a.Deliver,
                            WareHouseId = c.Id,
                        };

          
            if (ctx.WareHouseId.HasValue())
            {
                var department = _wareHouseRepository.Table.FirstOrDefault(x => x.Id == ctx.WareHouseId);

                if (department != null)
                {
                    StringBuilder GetListChidren = new StringBuilder();

                    GetListChidren.Append("with recursive cte (Id, Name, ParentId) as ( ");
                    GetListChidren.Append("  select     wh.Id, ");
                    GetListChidren.Append("             wh.Name, ");
                    GetListChidren.Append("             wh.ParentId ");
                    GetListChidren.Append("  from       WareHouse wh ");
                    GetListChidren.Append("  where      wh.ParentId='" + ctx.WareHouseId + "' ");
                    GetListChidren.Append("  union all ");
                    GetListChidren.Append("  SELECT     p.Id, ");
                    GetListChidren.Append("             p.Name, ");
                    GetListChidren.Append("             p.ParentId ");
                    GetListChidren.Append("  from       WareHouse  p ");
                    GetListChidren.Append("  inner join cte ");
                    GetListChidren.Append("          on p.ParentId = cte.id ");
                    GetListChidren.Append(") ");
                    GetListChidren.Append(" select cte.Id FROM cte GROUP BY cte.Id,cte.Name,cte.ParentId; ");
                    var departmentIds =
                        await _unitRepository.DataConnection.QueryToListAsync<string>(GetListChidren.ToString());
                    departmentIds.Add(ctx.WareHouseId);
                    if (departmentIds?.ToList().Count > 0)
                    {
                        var listDepartmentIds = departmentIds.ToList();
                        query = from p in query
                                where listDepartmentIds.Contains(p.WareHouseId)
                                select p;
                    }
                }
                else
                {
                    query = from p in query
                            where p.WareHouseId.Contains(ctx.WareHouseId.Trim())
                            select p;
                }
            }


            return query.ToList();

        }

        private async Task<IList<ReportOutwardMisaModel>> GetReportOutwardMisaExcelDetail(ReportOutwardMisaSearchContext ctx)
        {
            var outwards = from a in _outwardRepository.Table select a;
            if (ctx.FromDate.HasValue)
            {
                outwards = from p in outwards
                           where p.VoucherDate >= ctx.FromDate
                          select p;
            }

            if (ctx.ToDate.HasValue)
            {
                outwards = from p in outwards
                           where p.VoucherDate <= ctx.ToDate
                          select p;
            }

            var query = from a in outwards
                        join b in _outwardDetailRepository.Table on a.Id equals b.OutwardId into ps
                        from b in ps.DefaultIfEmpty()
                        join d in _wareHouseItemRepository.Table on b.ItemId equals d.Id into pi
                        from d in pi.DefaultIfEmpty()
                        join c in _wareHouseRepository.Table on a.WareHouseID equals c.Id into pa
                        from c in pa.DefaultIfEmpty()
                        join u in _unitRepository.Table on b.UnitId equals u.Id into pb
                        from u in pb.DefaultIfEmpty()
                        join acc in _accObjectRepository.Table  on a.AccObjectId equals acc.Id into acco
                        from acc in acco.DefaultIfEmpty()

                        select new ReportOutwardMisaModel
                        {
                            ReceiverCode = acc.Code,
                            Receiver = acc.Name,
                            Voucher=a.VoucherCodeReality,
                            Moment =a.VoucherDate.ToString("dd/MM/yyyy"),
                            User =a.CreatedBy,
                            WareHouseItemId = c.Code,
                            VoucherDateTime = a.VoucherDate.ToString("dd/MM/yyyy"),
                            VoucherCode = a.VoucherCode,
                            Reason = a.Reason.ToString(),
                            NoteRender = a.Description,
                            WareHouseItemCode = d.Code,
                            WareHouseItemName = d.Name,
                            AccountMore = "1541.66",
                            AccountYes = "1561",
                            UnitName = u.UnitName,
                            Amount = b.Amount,
                            Price = b.UIPrice,
                            Quantity = b.Quantity,
                            Id = b.Id,
                            DepartmentName = a.Deliver,
                            WareHouseId = c.Id,
                        };


            if (ctx.WareHouseId.HasValue())
            {
                var department = _wareHouseRepository.Table.FirstOrDefault(x => x.Id == ctx.WareHouseId);

                if (department != null)
                {
                    StringBuilder GetListChidren = new StringBuilder();

                    GetListChidren.Append("with recursive cte (Id, Name, ParentId) as ( ");
                    GetListChidren.Append("  select     wh.Id, ");
                    GetListChidren.Append("             wh.Name, ");
                    GetListChidren.Append("             wh.ParentId ");
                    GetListChidren.Append("  from       WareHouse wh ");
                    GetListChidren.Append("  where      wh.ParentId='" + ctx.WareHouseId + "' ");
                    GetListChidren.Append("  union all ");
                    GetListChidren.Append("  SELECT     p.Id, ");
                    GetListChidren.Append("             p.Name, ");
                    GetListChidren.Append("             p.ParentId ");
                    GetListChidren.Append("  from       WareHouse  p ");
                    GetListChidren.Append("  inner join cte ");
                    GetListChidren.Append("          on p.ParentId = cte.id ");
                    GetListChidren.Append(") ");
                    GetListChidren.Append(" select cte.Id FROM cte GROUP BY cte.Id,cte.Name,cte.ParentId; ");
                    var departmentIds =
                        await _unitRepository.DataConnection.QueryToListAsync<string>(GetListChidren.ToString());
                    departmentIds.Add(ctx.WareHouseId);
                    if (departmentIds?.ToList().Count > 0)
                    {
                        var listDepartmentIds = departmentIds.ToList();
                        query = from p in query
                                where listDepartmentIds.Contains(p.WareHouseId)
                                select p;
                    }
                }
                else
                {
                    query = from p in query
                            where p.WareHouseId.Contains(ctx.WareHouseId.Trim())
                            select p;
                }
            }


            return query.ToList();

        }

        public async Task<IPagedList<ReportAssetInfrastructorModel>> GetReportAssetInfrastructorTreeAsync(
            ReportAssetInfrastructorSearchContext ctx, string route)
        {
            if (string.IsNullOrEmpty(route))
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (string.IsNullOrEmpty(ctx.OrganizationUnitId))
            {
                throw new ArgumentNullException(nameof(ctx.OrganizationUnitId));
            }

            if (route.Equals(reportRoute[AssetStatic.AssetInfrastructor]))
            {
                var list = new StringBuilder();
                list.Append(" ( ");
                var department =
                    _organizationRepository.Table.FirstOrDefault(x => x.Id.ToString() == ctx.OrganizationUnitId);

                if (department != null && !string.IsNullOrEmpty(department.TreePath))
                {
                    var departmentIds = _organizationRepository.Table.Where(x => !string.IsNullOrEmpty(x.TreePath) &&
                            x.TreePath.Contains(department.TreePath))
                        .Select(x => x.Id.ToString());
                    if (departmentIds?.ToList().Count > 0)
                    {
                        var listDepartmentIds = departmentIds.ToList();
                        for (int i = 0; i < listDepartmentIds.Count; i++)
                        {
                            if (i == listDepartmentIds.Count - 1)
                                list.Append(listDepartmentIds[i]);
                            else
                                list.Append(listDepartmentIds[i] + ", ");
                        }
                    }
                }

                list.Append(" ) ");
                StringBuilder sb = new StringBuilder();

                sb.Append("SELECT ");
                sb.Append("  s.Area, ");
                sb.Append("  s.Name AS StationName, ");
                sb.Append("  s.Code AS StationCode, ");
                sb.Append("  s.Note, ");
                sb.Append("  s.LongItude AS LongItude, ");
                sb.Append("  s.LatItude AS LatItude, ");
                sb.Append("    a.UnitName , ");
                sb.Append("  a.Name, ");
                sb.Append("  a.OriginQuantity - a.RecallQuantity - a.SoldQuantity AS Quantity, ");
                sb.Append("  us.Description AS CurrentUsageStatus ");
                sb.Append("FROM Asset a ");
                sb.Append("  INNER JOIN Station s ");
                sb.Append("    ON a.StationCode = s.Code ");
                sb.Append("   INNER JOIN UsageStatus us ON a.CurrentUsageStatus=us.Id ");
                sb.Append("   WHERE a.AssetType = 20  ");
                if (department != null && !string.IsNullOrEmpty(department.TreePath))
                {
                    sb.Append(" AND a.OrganizationUnitId in ");
                    sb.Append(list);
                }

                //  sb.Append("    AND a.CreatedDate >= @fromDate AND a.CreatedDate <= @toDate ");
                sb.Append("    AND a.AllocationDate <= @toDate ");
                if (ctx.ItemCode.HasValue())
                    sb.Append("   AND a.Code=@pCode ");
                if (ctx.KeyWords.HasValue())
                    sb.Append("   AND a.StationCode=@pStationCode ");
                sb.Append(" ORDER BY a.AllocationDate desc ");
                sb.Append(" LIMIT @p2 OFFSET @p3 ");


                StringBuilder sbCount = new StringBuilder();

                sbCount.Append("SELECT ");
                sbCount.Append("  COUNT(*) ");
                sbCount.Append("FROM (SELECT ");
                sbCount.Append("  s.Area ");
                sbCount.Append("FROM Asset a ");
                sbCount.Append("  INNER JOIN Station s ");
                sbCount.Append("    ON a.StationCode = s.Code ");
                sbCount.Append("   INNER JOIN UsageStatus us ON a.CurrentUsageStatus=us.Id ");
                sbCount.Append("   WHERE a.AssetType = 20  ");
                if (department != null && !string.IsNullOrEmpty(department.TreePath))
                {
                    sbCount.Append(" AND a.OrganizationUnitId in ");
                    sbCount.Append(list);
                }

                //   sbCount.Append("    AND a.CreatedDate >= @fromDate AND a.CreatedDate <= @toDate ");
                sbCount.Append("     AND a.AllocationDate <= @toDate ");
                if (ctx.ItemCode.HasValue())
                    sbCount.Append("   AND a.Code=@pCode ");
                if (ctx.KeyWords.HasValue())
                    sbCount.Append("   AND a.StationCode=@pStationCode ");
                sbCount.Append("  ) d ");


                DataParameter OranId = new DataParameter("@OranId", ctx.OrganizationUnitId);
                DataParameter fromDate = new DataParameter("@fromDate", ConvertDateTimeToDateTimeSql(ctx.FromDate));
                DataParameter toDate = new DataParameter("@toDate", ConvertDateTimeToDateTimeSql(ctx.ToDate));
                DataParameter pCode = new DataParameter("@pCode", ctx.ItemCode);
                DataParameter p2 = new DataParameter("@p2", ctx.PageSize);
                DataParameter p3 = new DataParameter("@p3", ctx.PageIndex * ctx.PageSize);

                DataParameter pStationCode = new DataParameter("@pStationCode", ctx.KeyWords);


                var res = await _assetRepository.DataConnection.QueryToListAsync<ReportAssetInfrastructorModel>(
                    sb.ToString(), OranId, fromDate, toDate, pCode, p2, p3, pStationCode);

                var resCount = await _assetRepository.DataConnection.QueryToListAsync<int>(sbCount.ToString(), OranId,
                    fromDate, toDate, pCode, pStationCode);
                return new PagedList<ReportAssetInfrastructorModel>(res, ctx.PageIndex, ctx.PageSize,
                    resCount.FirstOrDefault());
            }

            return new PagedList<ReportAssetInfrastructorModel>(null, 0, 0);
        }

        #endregion

        public static string ConvertDateTimeToDateTimeSql(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return "";
            return "" + dateTime.Value.Year + "-" + dateTime.Value.Month + "-" + dateTime.Value.Day + " 12:0:0 AM  ";
        }

        public async Task<IList<ReportAssetInfrastructorModel>> GetExportAssetInfrastructorTreeAsync(
            ReportAssetInfrastructorSearchContext ctx, string route)
        {
            if (string.IsNullOrEmpty(route))
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (string.IsNullOrEmpty(ctx.OrganizationUnitId))
            {
                throw new ArgumentNullException(nameof(ctx.OrganizationUnitId));
            }

            if (route.Equals(reportRoute[AssetStatic.AssetInfrastructor]))
            {
                var list = new StringBuilder();
                list.Append(" ( ");
                var department =
                    _organizationRepository.Table.FirstOrDefault(x => x.Id.ToString() == ctx.OrganizationUnitId);

                if (!string.IsNullOrEmpty(department.TreePath))
                {
                    var departmentIds = _organizationRepository.Table.Where(x => !string.IsNullOrEmpty(x.TreePath) &&
                            x.TreePath.Contains(department.TreePath))
                        .Select(x => x.Id.ToString());
                    if (departmentIds?.ToList().Count > 0)
                    {
                        var listDepartmentIds = departmentIds.ToList();
                        for (int i = 0; i < listDepartmentIds.Count; i++)
                        {
                            if (i == listDepartmentIds.Count - 1)
                                list.Append(listDepartmentIds[i]);
                            else
                                list.Append(listDepartmentIds[i] + ", ");
                        }
                    }
                }

                list.Append(" ) ");
                StringBuilder sb = new StringBuilder();

                sb.Append("SELECT ");
                sb.Append("  s.Area, ");
                sb.Append("  s.Name AS StationName, ");
                sb.Append("  s.Code AS StationCode, ");
                sb.Append("  s.Note, ");
                sb.Append("  s.LongItude AS LongItude, ");
                sb.Append("  s.LatItude AS LatItude, ");
                sb.Append("    a.WareHouseItemCode AS UnitName, ");
                sb.Append("  a.Name, ");
                sb.Append("  a.OriginQuantity - a.RecallQuantity - a.SoldQuantity AS Quantity, ");
                sb.Append("  us.Description AS CurrentUsageStatus ");
                sb.Append("FROM Asset a ");
                sb.Append("  INNER JOIN Station s ");
                sb.Append("    ON a.StationCode = s.Code ");
                sb.Append("   INNER JOIN UsageStatus us ON a.CurrentUsageStatus=us.Id ");
                sb.Append("   WHERE a.AssetType = 20  ");
                if (!string.IsNullOrEmpty(department.TreePath))
                {
                    sb.Append(" AND a.OrganizationUnitId in ");
                    sb.Append(list);
                }

                sb.Append("   AND a.AllocationDate <= @toDate ");
                if (ctx.ItemCode.HasValue())
                    sb.Append("   AND a.Code=@pCode ");
                if (ctx.KeyWords.HasValue())
                    sb.Append("   AND a.StationCode=@pStationCode ");
                sb.Append(" ORDER BY a.AllocationDate desc ");


                DataParameter OranId = new DataParameter("@OranId", ctx.OrganizationUnitId);
                //    DataParameter fromDate = new DataParameter("@fromDate", ConvertDateTimeToDateTimeSql(ctx.FromDate));
                DataParameter toDate = new DataParameter("@toDate", ConvertDateTimeToDateTimeSql(ctx.ToDate));
                DataParameter pCode = new DataParameter("@pCode", ctx.ItemCode);
                DataParameter pStationCode = new DataParameter("@pStationCode", ctx.KeyWords);

                return await _assetRepository.DataConnection.QueryToListAsync<ReportAssetInfrastructorModel>(
                    sb.ToString(), OranId, toDate, pCode, pStationCode);
            }

            return new List<ReportAssetInfrastructorModel>();
        }

        public async Task<IList<Asset>> GetExportAssetOfficeTreeAsync(ReportAssetInfrastructorSearchContext ctx,
            string route)
        {
            if (string.IsNullOrEmpty(route))
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (string.IsNullOrEmpty(ctx.OrganizationUnitId))
            {
                throw new ArgumentNullException(nameof(ctx.OrganizationUnitId));
            }

            // BuildMyString.com generated code. Please enjoy your string responsibly.
            if (route.Equals(reportRoute[AssetStatic.AssetOffice]))
            {
                var list = new StringBuilder();
                list.Append(" ( ");
                var department =
                    _organizationRepository.Table.FirstOrDefault(x => x.Id.ToString() == ctx.OrganizationUnitId);

                if (department != null && !string.IsNullOrEmpty(department.TreePath))
                {
                    var departmentIds = _organizationRepository.Table.Where(x => !string.IsNullOrEmpty(x.TreePath) &&
                            x.TreePath.Contains(department.TreePath))
                        .Select(x => x.Id.ToString());
                    if (departmentIds?.ToList().Count > 0)
                    {
                        var listDepartmentIds = departmentIds.ToList();
                        for (int i = 0; i < listDepartmentIds.Count; i++)
                        {
                            if (i == listDepartmentIds.Count - 1)
                                list.Append(listDepartmentIds[i]);
                            else
                                list.Append(listDepartmentIds[i] + ", ");
                        }
                    }
                }

                list.Append(" ) ");

                StringBuilder sb = new StringBuilder();

                sb.Append("SELECT ");
                sb.Append("a.AllocationDate, ");
                sb.Append("  a.Code, ");
                sb.Append("  a.Name, ");
                sb.Append("  ac.Name AS CategoryId, ");
                sb.Append("  a.OriginQuantity, ");
                sb.Append("  a.RecallQuantity, ");
                sb.Append("  a.SoldQuantity, ");
                sb.Append("  a.BrokenQuantity, ");
                sb.Append("  a.UnitName, ");
                sb.Append("  a.StationName, ");
                sb.Append("  us.Description AS CurrentUsageStatus, ");
                sb.Append("  a.MaintenancedDate ");
                sb.Append("FROM Asset a ");
                sb.Append("  INNER JOIN UsageStatus us ");
                sb.Append("    ON a.CurrentUsageStatus = us.Id ");
                sb.Append("  inner JOIN AssetCategory ac ");
                sb.Append("    ON a.CategoryId = ac.Id ");
                sb.Append("WHERE a.AssetType = 10 ");
                if (department != null && !string.IsNullOrEmpty(department.TreePath))
                {
                    sb.Append(" AND a.OrganizationUnitId in ");
                    sb.Append(list);
                }

                if (ctx.ItemCode.HasValue())
                    sb.Append(" AND a.Code=@pCode ");
                if (ctx.KeyWords.HasValue())
                    sb.Append(" AND a.CategoryId=@aCategoryId ");
                // sb.Append("AND a.CreatedDate >= @fromDate ");
                sb.Append("AND a.AllocationDate <= @toDate ");
                sb.Append(" ORDER BY a.AllocationDate desc ");
                DataParameter OranId = new DataParameter("@OranId", ctx.OrganizationUnitId);
                //    DataParameter fromDate = new DataParameter("@fromDate", ConvertDateTimeToDateTimeSql(ctx.FromDate));
                DataParameter toDate = new DataParameter("@toDate", ConvertDateTimeToDateTimeSql(ctx.ToDate));
                DataParameter pCode = new DataParameter("@pCode", ctx.ItemCode);
                DataParameter p2 = new DataParameter("@p2", ctx.PageSize);
                DataParameter p3 = new DataParameter("@p3", ctx.PageIndex * ctx.PageSize);
                DataParameter aCategoryId = new DataParameter("@aCategoryId", ctx.KeyWords);
                return await _assetRepository.DataConnection.QueryToListAsync<Asset>(sb.ToString(), OranId,
                    toDate, pCode, aCategoryId);
            }

            return new List<Asset>();
        }

        public async Task<IPagedList<Asset>> GetReportAssetProjectTreeAsync(ReportAssetInfrastructorSearchContext ctx,
            string route)
        {
            if (string.IsNullOrEmpty(route))
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (string.IsNullOrEmpty(ctx.OrganizationUnitId))
            {
                throw new ArgumentNullException(nameof(ctx.OrganizationUnitId));
            }

            // BuildMyString.com generated code. Please enjoy your string responsibly.
            if (route.Equals(reportRoute[AssetStatic.AssetProject]))
            {
                var list = new StringBuilder();
                list.Append(" ( ");
                var department =
                    _organizationRepository.Table.FirstOrDefault(x => x.Id.ToString() == ctx.OrganizationUnitId);

                if (department != null && !string.IsNullOrEmpty(department.TreePath))
                {
                    var departmentIds = _organizationRepository.Table.Where(x => !string.IsNullOrEmpty(x.TreePath) &&
                            x.TreePath.Contains(department.TreePath))
                        .Select(x => x.Id.ToString());
                    if (departmentIds?.ToList().Count > 0)
                    {
                        var listDepartmentIds = departmentIds.ToList();
                        for (int i = 0; i < listDepartmentIds.Count; i++)
                        {
                            if (i == listDepartmentIds.Count - 1)
                                list.Append(listDepartmentIds[i]);
                            else
                                list.Append(listDepartmentIds[i] + ", ");
                        }
                    }
                }

                list.Append(" ) ");
                StringBuilder sb = new StringBuilder();

                sb.Append("SELECT ");
                sb.Append("  a.AllocationDate, ");
                sb.Append("  a.Code, ");
                sb.Append("  a.Name, ");
                sb.Append("  a.ProjectName, ");
                sb.Append("  a.ProjectCode, ");
                sb.Append("  a.CustomerName, ");
                sb.Append("  a.CustomerCode, ");
                sb.Append("  us.Description AS CurrentUsageStatus, ");
                sb.Append("  ac.Name AS CategoryId, ");
                sb.Append("  a.OriginQuantity, ");
                sb.Append("  a.RecallQuantity, ");
                sb.Append("  a.SoldQuantity, ");
                sb.Append("  a.BrokenQuantity, ");
                sb.Append("  a.UnitName, ");
                sb.Append("  a.MaintenancedDate ");
                sb.Append("FROM Asset a ");
                sb.Append("  INNER JOIN UsageStatus us ");
                sb.Append("    ON a.CurrentUsageStatus = us.Id ");
                sb.Append("  inner JOIN AssetCategory ac ");
                sb.Append("    ON a.CategoryId = ac.Id ");
                sb.Append("WHERE a.AssetType = 30 ");
                if (department != null && !string.IsNullOrEmpty(department.TreePath))
                {
                    sb.Append(" AND a.OrganizationUnitId in ");
                    sb.Append(list);
                }

                if (ctx.ItemCode.HasValue())
                    sb.Append(" AND a.Code=@pCode ");
                if (ctx.KeyWords.HasValue())
                    sb.Append("AND a.ProjectCode = @pProjectCode ");
                //  sb.Append("AND a.CreatedDate >= @fromDate ");
                sb.Append("AND a.AllocationDate <= @toDate ");
                sb.Append(" ORDER BY a.AllocationDate desc ");
                sb.Append("LIMIT @p2 OFFSET @p3 ");


                //count

                StringBuilder sbCount = new StringBuilder();
                sbCount.Append("SELECT COUNT(*) FROM( ");
                sbCount.Append("SELECT ");
                sbCount.Append("  a.Code ");
                sbCount.Append("FROM Asset a ");
                sbCount.Append("  INNER JOIN UsageStatus us ");
                sbCount.Append("    ON a.CurrentUsageStatus = us.Id ");
                sbCount.Append("  INNER JOIN AssetCategory ac ");
                sbCount.Append("    ON a.CategoryId = ac.Id ");
                sbCount.Append("WHERE a.AssetType = 30 ");
                if (department != null && !string.IsNullOrEmpty(department.TreePath))
                {
                    sbCount.Append(" AND a.OrganizationUnitId in ");
                    sbCount.Append(list);
                }

                if (ctx.ItemCode.HasValue())
                    sbCount.Append(" AND a.Code=@pCode ");
                if (ctx.KeyWords.HasValue())
                    sbCount.Append("AND a.ProjectCode = @pProjectCode ");
                //  sbCount.Append("AND a.CreatedDate >= @fromDate ");
                sbCount.Append("AND a.AllocationDate <= @toDate ");
                sbCount.Append(" )d ");

                DataParameter OranId = new DataParameter("@OranId", ctx.OrganizationUnitId);
                DataParameter fromDate = new DataParameter("@fromDate", ConvertDateTimeToDateTimeSql(ctx.FromDate));
                DataParameter toDate = new DataParameter("@toDate", ConvertDateTimeToDateTimeSql(ctx.ToDate));
                DataParameter pCode = new DataParameter("@pCode", ctx.ItemCode);
                DataParameter p2 = new DataParameter("@p2", ctx.PageSize);
                DataParameter p3 = new DataParameter("@p3", ctx.PageIndex * ctx.PageSize);
                DataParameter pStationCode = new DataParameter("@pProjectCode", ctx.KeyWords);
                var res = await _assetRepository.DataConnection.QueryToListAsync<Asset>(sb.ToString(), OranId, fromDate,
                    toDate, pCode, p2, p3, pStationCode);
                var resCount = await _assetRepository.DataConnection.QueryToListAsync<int>(sbCount.ToString(), OranId,
                    fromDate, toDate, pCode, pStationCode);
                return new PagedList<Asset>(res, ctx.PageIndex, ctx.PageSize, resCount.FirstOrDefault());
            }

            return new PagedList<Asset>(new List<Asset>(), 0, 50);
        }

        public async Task<IPagedList<Asset>> GetReportAssetOfficeTreeAsync(ReportAssetInfrastructorSearchContext ctx,
            string route)
        {
            if (string.IsNullOrEmpty(route))
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (string.IsNullOrEmpty(ctx.OrganizationUnitId))
            {
                throw new ArgumentNullException(nameof(ctx.OrganizationUnitId));
            }

            // BuildMyString.com generated code. Please enjoy your string responsibly.
            if (route.Equals(reportRoute[AssetStatic.AssetOffice]))
            {
                var list = new StringBuilder();
                list.Append(" ( ");
                var department =
                    _organizationRepository.Table.FirstOrDefault(x => x.Id.ToString() == ctx.OrganizationUnitId);

                if (department != null && !string.IsNullOrEmpty(department.TreePath))
                {
                    var departmentIds = _organizationRepository.Table.Where(x => !string.IsNullOrEmpty(x.TreePath) &&
                            x.TreePath.Contains(department.TreePath))
                        .Select(x => x.Id.ToString());
                    if (departmentIds?.ToList().Count > 0)
                    {
                        var listDepartmentIds = departmentIds.ToList();
                        for (int i = 0; i < listDepartmentIds.Count; i++)
                        {
                            if (i == listDepartmentIds.Count - 1)
                                list.Append(listDepartmentIds[i]);
                            else
                                list.Append(listDepartmentIds[i] + ", ");
                        }
                    }
                }

                list.Append(" ) ");

                StringBuilder sb = new StringBuilder();

                sb.Append("SELECT ");
                sb.Append("a.AllocationDate, ");
                sb.Append("  a.Code, ");
                sb.Append("  a.Name, ");
                sb.Append("  ac.Name AS CategoryId, ");
                sb.Append("  a.OriginQuantity, ");
                sb.Append("  a.RecallQuantity, ");
                sb.Append("  a.SoldQuantity, ");
                sb.Append("  a.BrokenQuantity, ");
                sb.Append("  a.UnitName, ");
                sb.Append("  a.StationName, ");
                sb.Append("  us.Description AS CurrentUsageStatus, ");
                sb.Append("  a.MaintenancedDate ");
                sb.Append("FROM Asset a ");
                sb.Append("  INNER JOIN UsageStatus us ");
                sb.Append("    ON a.CurrentUsageStatus = us.Id ");
                sb.Append("  inner JOIN AssetCategory ac ");
                sb.Append("    ON a.CategoryId = ac.Id ");
                sb.Append("WHERE a.AssetType = 10 ");
                if (department != null && !string.IsNullOrEmpty(department.TreePath))
                {
                    sb.Append(" AND a.OrganizationUnitId in ");
                    sb.Append(list);
                }

                if (ctx.ItemCode.HasValue())
                    sb.Append(" AND a.Code=@pCode ");
                if (ctx.KeyWords.HasValue())
                    sb.Append(" AND a.CategoryId=@aCategoryId ");
                // sb.Append("AND a.CreatedDate >= @fromDate ");
                sb.Append("AND a.AllocationDate <= @toDate ");
                sb.Append(" ORDER BY a.AllocationDate desc ");
                sb.Append(" LIMIT @p2 OFFSET @p3 ");


                //count

                StringBuilder sbCount = new StringBuilder();

                sbCount.Append("SELECT ");
                sbCount.Append("  COUNT(*) ");
                sbCount.Append("FROM (SELECT ");
                sbCount.Append("    a.Code ");
                sbCount.Append("  FROM Asset a ");
                sbCount.Append("    INNER JOIN UsageStatus us ");
                sbCount.Append("      ON a.CurrentUsageStatus = us.Id ");
                sbCount.Append("    INNER JOIN AssetCategory ac ");
                sbCount.Append("      ON a.CategoryId = ac.Id ");
                sbCount.Append("  WHERE a.AssetType = 10 ");
                if (department != null && !string.IsNullOrEmpty(department.TreePath))
                {
                    sbCount.Append(" AND a.OrganizationUnitId in ");
                    sbCount.Append(list);
                }

                if (ctx.ItemCode.HasValue())
                    sbCount.Append(" AND a.Code=@pCode ");
                if (ctx.KeyWords.HasValue())
                    sbCount.Append(" AND a.CategoryId=@aCategoryId ");
                // sbCount.Append("AND a.CreatedDate >= @fromDate ");
                sbCount.Append("AND a.AllocationDate <= @toDate ");
                sbCount.Append("  ) d ");

                DataParameter OranId = new DataParameter("@OranId", ctx.OrganizationUnitId);
                DataParameter fromDate = new DataParameter("@fromDate", ConvertDateTimeToDateTimeSql(ctx.FromDate));
                DataParameter toDate = new DataParameter("@toDate", ConvertDateTimeToDateTimeSql(ctx.ToDate));
                DataParameter pCode = new DataParameter("@pCode", ctx.ItemCode);
                DataParameter p2 = new DataParameter("@p2", ctx.PageSize);
                DataParameter p3 = new DataParameter("@p3", ctx.PageIndex * ctx.PageSize);
                DataParameter aCategoryId = new DataParameter("@aCategoryId", ctx.KeyWords);
                var res = await _assetRepository.DataConnection.QueryToListAsync<Asset>(sb.ToString(), OranId, fromDate,
                    toDate, pCode, p2, p3, aCategoryId);
                var resCount =
                    await _assetRepository.DataConnection.QueryToListAsync<int>(sbCount.ToString(), OranId, fromDate,
                        toDate, pCode, aCategoryId);
                return new PagedList<Asset>(res, ctx.PageIndex, ctx.PageSize, resCount.FirstOrDefault());
            }

            return new PagedList<Asset>(new List<Asset>(), 0, 50);
        }



        #region OutwardMisaReport

        public async Task<IPagedList<ReportOutwardMisaModel>> GetReportOutwardMisa(ReportOutwardMisaSearchContext ctx, string route)
        {
            if (string.IsNullOrEmpty(route))
            {
                throw new ArgumentNullException(nameof(route));
            }
            else if (route.Equals(reportRoute["OutwardMisa"]))
            {
                return await GetReportOutwardMisa(ctx);
            }
            else
            {
                return null;
            }
        }

        private async Task<IPagedList<ReportOutwardMisaModel>> GetReportOutwardMisa(ReportOutwardMisaSearchContext ctx)
        {
            var query = from a in _outwardRepository.Table
                        join b in _outwardDetailRepository.Table on a.Id equals b.OutwardId into ps
                        from b in ps.DefaultIfEmpty()
                        join d in _wareHouseItemRepository.Table on b.ItemId equals d.Id into pi
                        from d in pi.DefaultIfEmpty()
                        join c in _wareHouseRepository.Table on a.WareHouseID equals c.Id into pa
                        from c in pa.DefaultIfEmpty()
                        join u in _unitRepository.Table on b.UnitId equals u.Id into pb
                        from u in pb.DefaultIfEmpty()
                        join acc in _accObjectRepository.Table  on a.AccObjectId equals acc.Id into acco
                        from acc in acco.DefaultIfEmpty()

                        select new ReportOutwardMisaModel
                        {
                            ReceiverCode=acc.Code,
                            Receiver=acc.Name,
                            VoucherDate=a.VoucherDate,
                            Voucher=a.VoucherCodeReality,
                            Moment=a.VoucherDate.ToString("dd/MM/yyyy"),
                            User =a.CreatedBy,
                            WareHouseItemId = c.Name,
                            Reason= a.Reason.ToString(),
                            VoucherDateTime = a.VoucherDate.ToString("dd/MM/yyyy"),
                            VoucherCode = a.VoucherCode,
                            NoteRender = a.Description,
                            WareHouseItemCode = d.Code,
                            WareHouseItemName = d.Name,
                            AccountMore = "1541.66",
                            AccountYes = "1561",
                            UnitName = u.UnitName,
                            Amount = b.Amount,
                            Price = b.UIPrice,
                            Quantity = b.Quantity,
                            Id = b.Id,
                            DepartmentName = a.Deliver,
                            WareHouseId = c.Id,
                        };

            if (ctx.FromDate.HasValue)
            {
                query = query?
                    .Where(x => x.VoucherDate >= ctx.FromDate.Value);
            }

            if (ctx.ToDate.HasValue)
            {
                query = query?
                    .Where(x => x.VoucherDate <= ctx.ToDate.Value);
            }
            if (ctx.WareHouseId.HasValue())
            {
                var department = _wareHouseRepository.Table.FirstOrDefault(x => x.Id == ctx.WareHouseId);

                if (department != null)
                {
                    StringBuilder GetListChidren = new StringBuilder();

                    GetListChidren.Append("with recursive cte (Id, Name, ParentId) as ( ");
                    GetListChidren.Append("  select     wh.Id, ");
                    GetListChidren.Append("             wh.Name, ");
                    GetListChidren.Append("             wh.ParentId ");
                    GetListChidren.Append("  from       WareHouse wh ");
                    GetListChidren.Append("  where      wh.ParentId='" + ctx.WareHouseId + "' ");
                    GetListChidren.Append("  union all ");
                    GetListChidren.Append("  SELECT     p.Id, ");
                    GetListChidren.Append("             p.Name, ");
                    GetListChidren.Append("             p.ParentId ");
                    GetListChidren.Append("  from       WareHouse  p ");
                    GetListChidren.Append("  inner join cte ");
                    GetListChidren.Append("          on p.ParentId = cte.id ");
                    GetListChidren.Append(") ");
                    GetListChidren.Append(" select cte.Id FROM cte GROUP BY cte.Id,cte.Name,cte.ParentId; ");
                    var departmentIds =
                        await _unitRepository.DataConnection.QueryToListAsync<string>(GetListChidren.ToString());
                    departmentIds.Add(ctx.WareHouseId);
                    if (departmentIds?.ToList().Count > 0)
                    {
                        var listDepartmentIds = departmentIds.ToList();
                        query = from p in query
                                where listDepartmentIds.Contains(p.WareHouseId)
                                select p;
                    }
                }
                else
                {
                    query = from p in query
                            where p.WareHouseId.Contains(ctx.WareHouseId.Trim())
                            select p;
                }
            }


            return new PagedList<ReportOutwardMisaModel>(query, ctx.PageIndex, ctx.PageSize);
        }
        #endregion
    }
}
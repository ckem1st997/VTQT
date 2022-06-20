using LinqToDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Core.Domain;
using VTQT.Core.Domain.Master;
using VTQT.Core.Domain.Warehouse;
using VTQT.Core.Domain.Warehouse.Enum;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using VTQT.SharedMvc.Warehouse.Models;

namespace VTQT.Services.Warehouse
{
    public partial class ReportService : IReportService
    {
        #region
        private readonly IRepository<WareHouseItem> _itemRepository;
        private readonly IRepository<Unit> _unitRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<WarehouseBalance> _warehouseBalanceRepository;
        private readonly IRepository<BeginningWareHouse> _beginningWareHouseRepository;
        private readonly IRepository<OutwardDetail> _outwardDetailRepository;
        private readonly IRepository<Outward> _outwardRepository;
        private readonly IRepository<InwardDetail> _inwardDetailRepository;
        private readonly IRepository<Inward> _inwardRepository;
        private readonly IRepository<WareHouseItemCategory> _wareHouseItemCategoryRepository;

        #endregion

        public ReportService()
        {
            _outwardRepository = EngineContext.Current.Resolve<IRepository<Outward>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _outwardDetailRepository = EngineContext.Current.Resolve<IRepository<OutwardDetail>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _inwardDetailRepository = EngineContext.Current.Resolve<IRepository<InwardDetail>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _inwardRepository = EngineContext.Current.Resolve<IRepository<Inward>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _itemRepository = EngineContext.Current.Resolve<IRepository<WareHouseItem>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _unitRepository = EngineContext.Current.Resolve<IRepository<Unit>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _vendorRepository = EngineContext.Current.Resolve<IRepository<Vendor>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _warehouseBalanceRepository = EngineContext.Current.Resolve<IRepository<WarehouseBalance>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _beginningWareHouseRepository = EngineContext.Current.Resolve<IRepository<BeginningWareHouse>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _wareHouseItemCategoryRepository = EngineContext.Current.Resolve<IRepository<WareHouseItemCategory>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
        }


        #region Methods

        public IPagedList<ReportResponseModel> GetReport(ReportSearchContext ctx)
        {
            if (ctx is null)
                throw new ArgumentNullException(nameof(ctx));
            switch (ctx.ReportType)
            {
                case 1:
                    return ReportGeneral(ctx);
                case 2:
                    return ReportDetails(ctx);
                default:
                    return null;
            }
        }


        /// <summary>
        /// báo cáo tổng hợp
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private IPagedList<ReportResponseModel> ReportGeneral(ReportSearchContext ctx)
        {
            IQueryable<WareHouseItem> qItemIn;
            IQueryable<Inward> qIn, qIn1, qIn2;
            IQueryable<InwardDetail> qInDetalis;
            IQueryable<Outward> qOut, qOut1, qOut2;
            IQueryable<OutwardDetail> qOutDetalis;
            IQueryable<BeginningWareHouse> qBe, qBe1;
            ConditionReportGeneral(ctx, out qItemIn, out qIn, out qInDetalis, out qOut, out qOutDetalis, out qBe, out qBe1, out qIn1, out qIn2, out qOut1, out qOut2);
            var queryIn = GetQueryIn(ctx, qItemIn, qIn, qIn1, qIn2, qInDetalis, qOut, qOut1, qOut2, qOutDetalis, qBe, qBe1);
            return new PagedList<ReportResponseModel>(queryIn, ctx.PageIndex, ctx.PageSize);
        }

        private IQueryable<ReportResponseModel> GetQueryIn(ReportSearchContext ctx, IQueryable<WareHouseItem> qItemIn, IQueryable<Inward> qIn, IQueryable<Inward> qIn1, IQueryable<Inward> qIn2, IQueryable<InwardDetail> qInDetalis, IQueryable<Outward> qOut, IQueryable<Outward> qOut1, IQueryable<Outward> qOut2, IQueryable<OutwardDetail> qOutDetalis, IQueryable<BeginningWareHouse> qBe, IQueryable<BeginningWareHouse> qBe1)
        {
            return from i in qItemIn
                   join u in _unitRepository.Table on i.UnitId equals u.Id
                   group new { i, u } by new { i.Id, i.Code, i.Name, u.UnitName } into g
                   let sumIn = (from q in qIn
                                join qin in qInDetalis on q.Id equals qin.InwardId
                                where qin.ItemId.Equals(g.Key.Id)
                                select qin.UIQuantity).Sum()
                   let sumOut = (from q in qOut
                                 join qout in qOutDetalis on q.Id equals qout.OutwardId
                                 where qout.ItemId.Equals(g.Key.Id)
                                 select qout.UIQuantity).Sum()
                   let sumBe = (from q in qBe
                                where q.ItemId.Equals(g.Key.Id)
                                select q.Quantity).Sum()

                   let int02 = (from v1 in qBe1
                                join iu1 in qInDetalis on v1.ItemId equals iu1.ItemId
                                join inq1 in qIn2 on iu1.InwardId equals inq1.Id
                                where iu1.ItemId == g.Key.Id && inq1.CreatedDate >= v1.CreatedDate
                                select iu1.UIQuantity).Sum()
                   let out02 = (from v1 in qBe1
                                join iu1 in qOutDetalis on v1.ItemId equals iu1.ItemId
                                join inq1 in qOut2 on iu1.OutwardId equals inq1.Id
                                where iu1.ItemId == g.Key.Id && inq1.CreatedDate >= v1.CreatedDate
                                select iu1.UIQuantity).Sum()
                   let int01 = (
                                from v1 in qBe1
                                join iu1 in qInDetalis on v1.ItemId equals iu1.ItemId
                                join inq1 in qIn1 on iu1.InwardId equals inq1.Id
                                where iu1.ItemId == g.Key.Id && inq1.CreatedDate >= v1.CreatedDate
                                select iu1.UIQuantity).Sum()
                   let out01 = (
                                from v1 in qBe1
                                join iu1 in qOutDetalis on v1.ItemId equals iu1.ItemId
                                join inq1 in qOut1 on iu1.OutwardId equals inq1.Id
                                where iu1.ItemId == g.Key.Id && inq1.CreatedDate >= v1.CreatedDate
                                select iu1.UIQuantity).Sum()
                   let sumBeOne = (from i in qBe where i.ItemId.Equals(g.Key.Id) select i.Quantity).Sum()
                   let sumWb = (from i in _warehouseBalanceRepository.Table where i.ItemId.Equals(g.Key.Id) && i.WarehouseId.Equals(ctx.WareHouseId) select i.Quantity).Sum()

                   select new ReportResponseModel
                   {
                       WarehouseItemCode = g.Key.Code,
                       WarehouseItemName = g.Key.Name,
                       InwardQuantity = sumIn,
                       OutwardQuantity = sumOut,
                       BeginningQuantity = sumBe,
                       TotalQuantity = GetQuantity(int02, out02, int01, out01, ctx.ToDate, sumBeOne, sumWb),
                       UnitName = g.Key.UnitName
                   };
        }

        private void ConditionReportGeneral(ReportSearchContext ctx, out IQueryable<WareHouseItem> qItemIn, out IQueryable<Inward> qIn, out IQueryable<InwardDetail> qInDetalis, out IQueryable<Outward> qOut, out IQueryable<OutwardDetail> qOutDetalis, out IQueryable<BeginningWareHouse> qBe, out IQueryable<BeginningWareHouse> qBe1, out IQueryable<Inward> qIn1, out IQueryable<Inward> qIn2, out IQueryable<Outward> qOut1, out IQueryable<Outward> qOut2)
        {
            qItemIn = from i in _itemRepository.Table select i;
            qIn = from i in _inwardRepository.Table select i;
            qInDetalis = from i in _inwardDetailRepository.Table select i;
            qOut = from i in _outwardRepository.Table select i;
            qOutDetalis = from i in _outwardDetailRepository.Table select i;
            qBe = from i in _beginningWareHouseRepository.Table select i;
            qBe1 = from i in _beginningWareHouseRepository.Table select i;
            qIn1 = from i in _inwardRepository.Table select i;
            qIn2 = from i in _inwardRepository.Table select i;
            qOut1 = from i in _outwardRepository.Table select i;
            qOut2 = from i in _outwardRepository.Table select i;
            if (ctx.WareHouseId.HasValue())
            {
                qIn1 = from q in qIn1
                       where q.WareHouseID.Equals(ctx.WareHouseId)
                       select q;
                qOut1 = from q in qOut1
                        where q.WareHouseID.Equals(ctx.WareHouseId)
                        select q;
                qIn2 = from q in qIn2
                       where q.WareHouseID.Equals(ctx.WareHouseId)
                       select q;
                qOut2 = from q in qOut2
                        where q.WareHouseID.Equals(ctx.WareHouseId)
                        select q;
                qBe = from q in qBe
                      where q.WareHouseId.Equals(ctx.WareHouseId)
                      select q;
                qBe1 = from q in qBe1
                       where q.WareHouseId.Equals(ctx.WareHouseId)
                       select q;
                qOut = from q in qOut
                       where q.WareHouseID.Equals(ctx.WareHouseId)
                       select q;
                qIn = from q in qIn
                      where q.WareHouseID.Equals(ctx.WareHouseId)
                      select q;
            }

            if (ctx.WareHouseItemId.HasValue())
            {

                qBe = from q in qBe
                      where q.WareHouseItem.Equals(ctx.WareHouseItemId)
                      select q;
                qBe1 = from q in qBe1
                       where q.WareHouseItem.Equals(ctx.WareHouseItemId)
                       select q;
                qOutDetalis = from q in qOutDetalis
                              where q.ItemId.Equals(ctx.WareHouseItemId)
                              select q;
                qInDetalis = from q in qInDetalis
                             where q.ItemId.Equals(ctx.WareHouseItemId)
                             select q;
            }
            if (ctx.FromDate.HasValue)
            {
                qIn1 = from q in qIn1
                       where q.CreatedDate <= ctx.FromDate
                       select q;
                qOut1 = from q in qOut1
                        where q.CreatedDate <= ctx.FromDate
                        select q;
                qIn = from q in qIn
                      where q.CreatedDate <= ctx.FromDate
                      select q;
                qOut = from q in qOut
                       where q.CreatedDate <= ctx.FromDate
                       select q;
                qBe = from q in qBe
                      where q.CreatedDate >= ctx.ToDate
                      select q;
            }
            if (ctx.ToDate.HasValue)
            {
                qIn2 = from q in qIn2
                       where q.CreatedDate <= ctx.ToDate
                       select q;
                qOut2 = from q in qOut2
                        where q.CreatedDate <= ctx.ToDate
                        select q;
                qIn = from q in qIn
                      where q.CreatedDate <= ctx.ToDate
                      select q;
                qOut = from q in qOut
                       where q.CreatedDate <= ctx.ToDate
                       select q;
                qBe = from q in qBe
                      where q.CreatedDate <= ctx.ToDate
                      select q;
            }

            if (ctx.DepartmentId > 0)
            {
                qOutDetalis = from q in qOutDetalis
                              where q.DepartmentId.Equals(ctx.DepartmentId)
                              select q;
                qInDetalis = from q in qInDetalis
                             where q.DepartmentId.Equals(ctx.DepartmentId)
                             select q;
            }

            if (ctx.ProjectId > 0)
            {
                qOutDetalis = from q in qOutDetalis
                              where q.DepartmentId.Equals(ctx.ProjectId)
                              select q;
                qInDetalis = from q in qInDetalis
                             where q.DepartmentId.Equals(ctx.ProjectId)
                             select q;
            }

            if (ctx.Proposer.HasValue())
            {
                qOutDetalis = from q in qOutDetalis
                              where q.EmployeeName.Equals(ctx.Proposer)
                              select q;
                qInDetalis = from q in qInDetalis
                             where q.EmployeeName.Equals(ctx.Proposer)
                             select q;
            }
        }





        /// <summary>
        /// báo cáo chi tiết nhập, tồn
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private IPagedList<ReportResponseModel> ReportDetails(ReportSearchContext ctx)
        {
            var qItemOut = from i in _itemRepository.Table select i;
            var qOut = from i in _outwardRepository.Table select i;
            var qOutDetails = from i in _outwardDetailRepository.Table select i;
            if (ctx.WareHouseItemId.HasValue())
            {
                qItemOut = from q in qItemOut
                           where q.Id.Equals(ctx.WareHouseItemId)
                           select q;
            }
            if (ctx.WareHouseId.HasValue())
            {
                qOut = from q in qOut
                       where q.WareHouseID.Equals(ctx.WareHouseId)
                       select q;
            }
            if (ctx.FromDate.HasValue)
            {
                qOut = from q in qOut
                       where q.CreatedDate >= ctx.FromDate
                       select q;
            }
            if (ctx.ToDate.HasValue)
            {
                qOut = from q in qOut
                       where q.CreatedDate <= ctx.ToDate
                       select q;
            }

            if (ctx.DepartmentId > 0)
            {
                qOutDetails = from q in qOutDetails
                              where q.DepartmentId.Equals(ctx.DepartmentId)
                              select q;
            }

            if (ctx.ProjectId > 0)
            {
                qOutDetails = from q in qOutDetails
                              where q.DepartmentId.Equals(ctx.ProjectId)
                              select q;
            }

            if (ctx.Proposer.HasValue())
            {
                qOutDetails = from q in qOutDetails
                              where q.EmployeeName.Equals(ctx.Proposer)
                              select q;
            }
            var queryIn = (from i in qItemOut
                           join u in _unitRepository.Table on i.UnitId equals u.Id
                           join v in qOutDetails on i.Id equals v.ItemId
                           join o in qOut on v.OutwardId equals o.Id
                           join ca in _wareHouseItemCategoryRepository.Table on i.CategoryID equals ca.Id
                           group new { i, u, v, o, ca } by new { v.ItemId, v.ProjectName, o.CreatedDate, v.Id, i.Code, i.Name, u.UnitName, nameCategory = ca.Name, o.Description, o.Reason, o.ReasonDescription, v.UIQuantity, v.DepartmentName, v.EmployeeName } into g
                           let sumBE = (from i in _beginningWareHouseRepository.Table
                                        where i.ItemId.Equals(g.Key.ItemId)
                                        select i.Quantity).FirstOrDefault()
                                        +
                                        (from i in _inwardRepository.Table
                                         join ii in _inwardDetailRepository.Table on i.Id equals ii.InwardId
                                         where ii.ItemId.Equals(g.Key.ItemId) && i.CreatedDate <= g.Key.CreatedDate
                                         select ii.UIQuantity).Sum()
                                         -
                                         (from i in _outwardRepository.Table
                                          join ii in _outwardDetailRepository.Table on i.Id equals ii.OutwardId
                                          where ii.ItemId.Equals(g.Key.ItemId) && i.CreatedDate <= g.Key.CreatedDate
                                          select ii.UIQuantity).Sum()
                           select new ReportResponseModel
                           {
                               Date = g.Key.CreatedDate,
                               WarehouseItemCode = g.Key.Code,
                               WarehouseItemName = g.Key.Name,
                               Generic = g.Key.nameCategory,
                               UnitName = g.Key.UnitName,
                               OutwardQuantity = g.Key.UIQuantity,
                               Purpose = "",
                               BeginningQuantity = sumBE,
                               BalanceQuantity = sumBE - g.Key.UIQuantity,
                               InwardQuantity = 0,
                               DepartmentName = g.Key.DepartmentName,
                               Proposer = g.Key.EmployeeName,
                               Description = g.Key.Description,
                               ProjectName = g.Key.ProjectName,
                               Note = ""
                           }).UnionAll(from i in qItemOut
                                       join u in _unitRepository.Table on i.UnitId equals u.Id
                                       join v in _inwardDetailRepository.Table on i.Id equals v.ItemId
                                       join o in _inwardRepository.Table on v.InwardId equals o.Id
                                       join ca in _wareHouseItemCategoryRepository.Table on i.CategoryID equals ca.Id
                                       group new { i, u, v, o, ca } by new { v.ItemId, v.ProjectName, o.CreatedDate, v.Id, i.Code, i.Name, u.UnitName, nameCategory = ca.Name, o.Description, o.Reason, o.ReasonDescription, v.UIQuantity, v.DepartmentName, v.EmployeeName } into g
                                       let sumBE = (from i in _beginningWareHouseRepository.Table
                                                    where i.ItemId.Equals(g.Key.ItemId)
                                                    select i.Quantity).FirstOrDefault() +
                                                    (from i in _inwardRepository.Table
                                                     join ii in _inwardDetailRepository.Table on i.Id equals ii.InwardId
                                                     where ii.ItemId.Equals(g.Key.ItemId) && i.CreatedDate <= g.Key.CreatedDate
                                                     select ii.UIQuantity).Sum() -
                                                     (from i in _outwardRepository.Table
                                                      join ii in _outwardDetailRepository.Table on i.Id equals ii.OutwardId
                                                      where ii.ItemId.Equals(g.Key.ItemId) && i.CreatedDate <= g.Key.CreatedDate
                                                      select ii.UIQuantity).Sum()
                                       select new ReportResponseModel
                                       {
                                           Date = g.Key.CreatedDate,
                                           WarehouseItemCode = g.Key.Code,
                                           WarehouseItemName = g.Key.Name,
                                           Generic = g.Key.nameCategory,
                                           UnitName = g.Key.UnitName,
                                           OutwardQuantity = 0,
                                           Purpose = "",
                                           BeginningQuantity = sumBE,
                                           BalanceQuantity = sumBE + g.Key.UIQuantity,
                                           InwardQuantity = g.Key.UIQuantity,
                                           DepartmentName = g.Key.DepartmentName,
                                           Proposer = g.Key.EmployeeName,
                                           Description = g.Key.Description,
                                           ProjectName = g.Key.ProjectName,
                                           Note = ""
                                       }).ToList();

            return new PagedList<ReportResponseModel>(queryIn, ctx.PageIndex, ctx.PageSize);
        }



        private static decimal GetQuantity(decimal a, decimal b, decimal c, decimal d, DateTime? ctx, decimal qBe, int qWl)
        {
            if (a == b && b == c && c == d && c == 0 || (a - b - c + d) <= 0)
            {
                if (ctx.HasValue && ctx >= DateTime.UtcNow)
                    return qWl;
                return qBe;
            }
            return a - b - c + d;
        }

        /// <summary>
        /// so sánh date theo ngày, tháng, năm
        /// </summary>
        /// <param name="toDate">Ngày đầu</param>
        /// <param name="fromDate">Ngày hai</param>
        /// <returns></returns>
        private static bool EqualsDate(DateTime toDate, DateTime fromDate)
        {
            if (fromDate.Year > toDate.Year)
                return false;
            else if (fromDate.Year == toDate.Year)
            {
                if (fromDate.Month > toDate.Month)
                    return false;
                else if (fromDate.Month == toDate.Month)
                {
                    if (fromDate.Day >= toDate.Day)
                        return false;
                    return true;
                }
                return true;
            }
            return true;
        }

        #endregion

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
    }
}

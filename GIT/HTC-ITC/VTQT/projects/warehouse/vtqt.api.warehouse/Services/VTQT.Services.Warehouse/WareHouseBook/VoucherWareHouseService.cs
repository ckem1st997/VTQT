using System;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using VTQT.SharedMvc.Warehouse.Models;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using VTQT.SharedMvc.Warehouse;
using System.Threading.Tasks;
using LinqToDB.Data;

namespace VTQT.Services.Warehouse
{
    public partial class VoucherWareHouseService : IVoucherWareHouseService
    {
        #region Fields

        private readonly IRepository<Inward> _inwardRepository;
        private readonly IRepository<Outward> _outwardRepository;
        private readonly IRepository<InwardDetail> _inwardDetailRepository;
        private readonly IRepository<OutwardDetail> _outwardDetailRepository;
        private readonly IRepository<WareHouse> _wareHouseRepository;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public VoucherWareHouseService(IWorkContext workContext)
        {
            _workContext = workContext;
            _inwardRepository =
                EngineContext.Current.Resolve<IRepository<Inward>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _outwardRepository =
                EngineContext.Current.Resolve<IRepository<Outward>>(
                    DataConnectionHelper.ConnectionStringNames.Warehouse);
            _inwardDetailRepository =
                EngineContext.Current.Resolve<IRepository<InwardDetail>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _outwardDetailRepository =
                EngineContext.Current.Resolve<IRepository<OutwardDetail>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _wareHouseRepository =
                EngineContext.Current.Resolve<IRepository<WareHouse>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
        }

        #endregion

        #region Methods

        public async Task<IPagedList<VoucherWareHouseModel>> Get(VoucherWareHouseSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();
            var vouchers = new List<VoucherWareHouseModel>();
            const int searchAll = 0;
            const int searchByInward = 1;
            const int searchByOutward = 2;

            if (string.IsNullOrEmpty(ctx.WareHouseId))
            {
                switch (ctx.SelectedVoucherType)
                {
                    case searchAll:
                        {
                            var inwardQuery = from p in _inwardRepository.Table select p;
                            var outwardQuery = from p in _outwardRepository.Table select p;
                            await FilterQuery(inwardQuery, outwardQuery, ctx, vouchers);
                            break;
                        }
                    case searchByInward:
                        {
                            var inwardQuery = from p in _inwardRepository.Table select p;
                            await FilterQuery(inwardQuery, null, ctx, vouchers);
                            break;
                        }
                    case searchByOutward:
                        {
                            var outwardQuery = from p in _outwardRepository.Table select p;
                            await FilterQuery(null, outwardQuery, ctx, vouchers);
                            break;
                        }
                }
            }
            else
            {
                switch (ctx.SelectedVoucherType)
                {
                    case searchAll:
                        {
                            var inwardQuery = from p in _inwardRepository.Table select p;
                            var outwardQuery = from p in _outwardRepository.Table select p;
                            await FilterQuery(inwardQuery, outwardQuery, ctx, vouchers);
                            break;
                        }
                    case searchByInward:
                        {
                            var inwardQuery = from p in _inwardRepository.Table select p;
                            await FilterQuery(inwardQuery, null, ctx, vouchers);
                            break;
                        }
                    case searchByOutward:
                        {
                            var outwardQuery = from p in _outwardRepository.Table select p;
                            await FilterQuery(null, outwardQuery, ctx, vouchers);
                            break;
                        }
                }
            }

            vouchers.Sort();

            return new PagedList<VoucherWareHouseModel>(vouchers, ctx.PageIndex, ctx.PageSize);
        }

        public async Task DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var inw = from p in _inwardRepository.Table
                      where ids.Contains(p.Id)
                      select p;
            var ouw = from p in _outwardRepository.Table
                      where ids.Contains(p.Id)
                      select p;

            if (inw?.ToList().Count > 0)
            {
                IEnumerable<string> InwIds()
                {
                    foreach (var i in inw.ToList())
                    {
                        yield return i.Id;
                    }
                }

                var inwardIds = InwIds();

                var query = from p in _inwardDetailRepository.Table
                            where inwardIds.Contains(p.InwardId)
                            select p;
                var inwDetails = query.ToList();
                if (inwDetails?.Count > 0)
                {
                    IEnumerable<string> InwDetIds()
                    {
                        foreach (var inwDetail in inwDetails)
                        {
                            yield return inwDetail.Id;
                        }
                    }

                    await _inwardDetailRepository.DeleteAsync(InwDetIds());
                }

                await _inwardRepository.DeleteAsync(InwIds());
            }

            if (ouw?.ToList().Count > 0)
            {
                IEnumerable<string> OuwIds()
                {
                    foreach (var o in ouw.ToList())
                    {
                        yield return o.Id;
                    }
                }

                var outwardIds = OuwIds();

                var query = from p in _outwardDetailRepository.Table
                            where outwardIds.Contains(p.OutwardId)
                            select p;

                var ouwDetails = query.ToList();
                if (ouwDetails?.Count > 0)
                {
                    IEnumerable<string> OuwDetIds()
                    {
                        foreach (var ouwDetail in ouwDetails)
                        {
                            yield return ouwDetail.Id;
                        }
                    }

                    await _outwardDetailRepository.DeleteAsync(OuwDetIds());
                }

                await _outwardRepository.DeleteAsync(OuwIds());
            }
        }

        #endregion

        #region Utilities

        private async Task FilterQuery(IQueryable<Inward> inwardQuery,
            IQueryable<Outward> outwardQuery,
            VoucherWareHouseSearchContext ctx,
            ICollection<VoucherWareHouseModel> vouchers)
        {
            if (ctx.Keywords.HasValue())
            {
                inwardQuery = inwardQuery?
                    .Where(x => x.VoucherCode.Contains(ctx.Keywords)
                                || x.Voucher.Contains(ctx.Keywords)
                                || x.Description.Contains(ctx.Keywords)
                                || x.DeliverAddress.Contains(ctx.Keywords)
                                || x.DeliverPhone.Contains(ctx.Keywords)
                                || x.DeliverDepartment.Contains(ctx.Keywords)
                                || x.ReceiverAddress.Contains(ctx.Keywords)
                                || x.ReceiverDepartment.Contains(ctx.Keywords)
                                || x.ReceiverPhone.Contains(ctx.Keywords)
                                || x.CreatedBy.Contains(ctx.Keywords)
                                || x.ModifiedBy.Contains(ctx.Keywords)
                                || x.Deliver.Contains(ctx.Keywords)
                                || x.Receiver.Contains(ctx.Keywords))
                    .Distinct();

                outwardQuery = outwardQuery?
                    .Where(x => x.VoucherCode.Contains(ctx.Keywords)
                                || x.VoucherCodeReality.Contains(ctx.Keywords)
                                || x.Description.Contains(ctx.Keywords)
                                || x.DeliverAddress.Contains(ctx.Keywords)
                                || x.DeliverPhone.Contains(ctx.Keywords)
                                || x.DeliverDepartment.Contains(ctx.Keywords)
                                || x.ReceiverAddress.Contains(ctx.Keywords)
                                || x.ReceiverDepartment.Contains(ctx.Keywords)
                                || x.ReceiverPhone.Contains(ctx.Keywords)
                                || x.CreatedBy.Contains(ctx.Keywords)
                                || x.ModifiedBy.Contains(ctx.Keywords)
                                || x.Deliver.Contains(ctx.Keywords)
                                || x.ReceiverCode.Contains(ctx.Keywords)
                                || x.Receiver.Contains(ctx.Keywords))
                    .Distinct();
            }

            if (ctx.FromDate.HasValue)
            {
                inwardQuery = inwardQuery?
                    .Where(x => x.VoucherDate >= ctx.FromDate.Value);

                outwardQuery = outwardQuery?
                    .Where(x => x.VoucherDate >= ctx.FromDate.Value);
            }

            if (ctx.ToDate.HasValue)
            {
                inwardQuery = inwardQuery?
                    .Where(x => x.VoucherDate <= ctx.ToDate.Value);

                outwardQuery = outwardQuery?
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
                        await _inwardRepository.DataConnection.QueryToListAsync<string>(GetListChidren.ToString());
                    departmentIds.Add(ctx.WareHouseId);
                    if (departmentIds?.ToList().Count > 0)
                    {
                        var listDepartmentIds = departmentIds?.ToList();
                        if (inwardQuery?.ToList().Count > 0)
                        {
                            inwardQuery = inwardQuery.Where(p => listDepartmentIds.Contains(p.WareHouseID));
                        }

                        if (outwardQuery?.ToList().Count > 0)
                        {
                            outwardQuery = outwardQuery.Where(p => listDepartmentIds.Contains(p.WareHouseID));
                        }                                                
                    }
                }
                else
                {
                    if (inwardQuery?.ToList().Count > 0)
                    {
                        inwardQuery = inwardQuery.Where(p => p.WareHouseID.Contains(ctx.WareHouseId));
                    }

                    if (outwardQuery?.ToList().Count > 0)
                    {
                        outwardQuery = outwardQuery.Where(p => p.WareHouseID.Contains(ctx.WareHouseId));
                    }                                        
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("WITH RECURSIVE cte (Id, Name, ParentId) ");
                sb.Append("AS ");
                sb.Append("(SELECT ");
                sb.Append("      wh.Id, ");
                sb.Append("      wh.Name, ");
                sb.Append("      wh.ParentId ");
                sb.Append("    FROM WareHouse wh ");
                sb.Append("    INNER JOIN WareHouseUser whu ");
                sb.Append("    ON wh.Id = whu.WarehouseId ");
                sb.Append("    WHERE whu.UserId='" + _workContext.UserId + "' ");
                sb.Append("  UNION ALL ");
                sb.Append("  SELECT ");
                sb.Append("    p.Id, ");
                sb.Append("    p.Name, ");
                sb.Append("    p.ParentId ");
                sb.Append("  FROM WareHouse p ");
                sb.Append("    INNER JOIN cte ");
                sb.Append("      ON p.ParentId = cte.Id) ");
                sb.Append("SELECT ");
                sb.Append("  cte.Id ");
                sb.Append("FROM cte ");
                sb.Append("GROUP BY cte.Id, ");
                sb.Append("         cte.Name, ");
                sb.Append("         cte.ParentId; ");

                var departmentIds = await _inwardRepository.DataConnection.QueryToListAsync<string>(sb.ToString());
                if (departmentIds?.ToList().Count > 0)
                {
                    var listDepartmentIds = departmentIds.ToList();
                    if (inwardQuery?.ToList().Count > 0)
                    {
                        inwardQuery = from p in inwardQuery where listDepartmentIds.Contains(p.WareHouseID) select p;
                    }
                    
                    if (outwardQuery?.ToList().Count > 0)
                    {
                        outwardQuery = from p in outwardQuery where listDepartmentIds.Contains(p.WareHouseID) select p;
                    }                                        
                }

            }

            if (ctx.SelectedInwardReason != 0 && ctx.SelectedOutwardReason != 0)
            {
                inwardQuery = inwardQuery?
                    .Where(x => x.Reason == ctx.SelectedInwardReason);

                outwardQuery = outwardQuery?
                    .Where(x => x.Reason == ctx.SelectedOutwardReason);

                inwardQuery?.ToList()?.ForEach(e =>
                {
                    var m = e.InwardToVoucherModel();
                    m.VoucherDate = e.VoucherDate.ToLocalTime();
                    m.VoucherType = nameof(Inward);
                    vouchers.Add(m);
                });

                outwardQuery?.ToList()?.ForEach(e =>
                {
                    var m = e.OutwardToVoucherModel();
                    m.VoucherDate = e.VoucherDate.ToLocalTime();
                    m.VoucherType = nameof(Outward);
                    vouchers.Add(m);
                });
            }
            else if (ctx.SelectedInwardReason != 0 && ctx.SelectedOutwardReason == 0)
            {
                inwardQuery = inwardQuery?
                    .Where(x => x.Reason == ctx.SelectedInwardReason);

                inwardQuery?.ToList()?.ForEach(e =>
                {
                    var m = e.InwardToVoucherModel();
                    m.VoucherDate = e.VoucherDate.ToLocalTime();
                    m.VoucherType = nameof(Inward);
                    vouchers.Add(m);
                });
            }
            else if (ctx.SelectedOutwardReason != 0 && ctx.SelectedInwardReason == 0)
            {
                outwardQuery = outwardQuery?
                    .Where(x => x.Reason == ctx.SelectedOutwardReason);

                outwardQuery?.ToList()?.ForEach(e =>
                {
                    var m = e.OutwardToVoucherModel();
                    m.VoucherDate = e.VoucherDate.ToLocalTime();
                    m.VoucherType = nameof(Outward);
                    vouchers.Add(m);
                });
            }
            else
            {
                inwardQuery?.ToList()?.ForEach(e =>
                {
                    var m = e.InwardToVoucherModel();
                    m.VoucherDate = e.VoucherDate.ToLocalTime();
                    m.VoucherType = nameof(Inward);
                    m.Voucher = e.Voucher;
                    vouchers.Add(m);
                });

                outwardQuery?.ToList()?.ForEach(e =>
                {
                    var m = e.OutwardToVoucherModel();
                    m.VoucherDate = e.VoucherDate.ToLocalTime();
                    m.VoucherType = nameof(Outward);
                    m.Voucher = e.VoucherCodeReality;
                    vouchers.Add(m);
                });
            }
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;
using System.Text;
using LinqToDB.Data;
using VTQT.Caching;
using VTQT.Core.Domain.FbmCrm;
using VTQT.SharedMvc.Warehouse.Models;

namespace VTQT.Services.Warehouse
{
    public partial class WareHouseUserService : IWareHouseUserService
    {
        private readonly IRepository<WareHouseUser> _repository;
        private readonly IRepository<WareHouse> _repositoryWh;
        private readonly IIntRepository<ApplicationUser> _customerRepository;
        private readonly IXBaseCacheManager _cacheManager;
        private readonly IWareHouseService _organizationalUnitService;


        public WareHouseUserService(IXBaseCacheManager cacheManager, IWareHouseService organizationalUnitService)
        {
            _repository =
                EngineContext.Current.Resolve<IRepository<WareHouseUser>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _customerRepository =
                EngineContext.Current.Resolve<IIntRepository<ApplicationUser>>(DataConnectionHelper
                    .ConnectionStringNames.FbmCrm);
            _repositoryWh =
                EngineContext.Current.Resolve<IRepository<WareHouse>>(DataConnectionHelper.ConnectionStringNames
                    .Warehouse);
            _cacheManager = cacheManager;
            _organizationalUnitService = organizationalUnitService;
        }

        public async Task<long> InsertRangeAsync(IEnumerable<Core.Domain.Warehouse.WareHouseUser> entities)
        {
            if (entities is null)
                throw new ArgumentNullException(nameof(entities));
            var result = await _repository.InsertAsync(entities);
            return result;
        }

        public async Task<int> UpdateRangeAsync(IEnumerable<Core.Domain.Warehouse.WareHouseUser> entities)
        {
            if (entities is null)
                throw new ArgumentNullException(nameof(entities));
            var result = await _repository.UpdateAsync(entities);
            return result;
        }

        public async Task<WareHouseUser> GetByIdAsync(string id)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));
            var result = await _repository.GetByIdAsync(id);
            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids is null)
                throw new ArgumentNullException(nameof(ids));
            var result = await _repository.DeleteAsync(ids);
            return result;
        }

        public async Task<bool> ExistAsync(string idWareHouse, string idUser)
        {
            if (idWareHouse.IsEmpty() || idUser.IsEmpty())
                throw new ArgumentException("Param is null !");
            var check = await _repository.Table.AnyAsync(
                a => a.WarehouseId.Equals(idWareHouse)
                     && a.UserId.Equals(idUser));
            return check;
        }

        public async Task<List<WareHouseModel>> GetListByUser(string idUser)
        {
            var cacheKey = WarehouseCacheKeys.Warehouses.AllCacheKey.FormatWith(false);
            var models = _cacheManager.GetDb(cacheKey, () =>
            {
                var result = _organizationalUnitService.GetAll(false);
                return result.ToList();
            });
            

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
            sb.Append("    WHERE whu.UserId='" + idUser + "' ");
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

            var departmentIds = await _repository.DataConnection.QueryToListAsync<string>(sb.ToString());
            if (departmentIds?.ToList().Count > 0)
            {
                var listDepartmentIds = departmentIds.ToList();
                models = models.Where(x => listDepartmentIds.Contains(x.Id)).ToList();
                var res = new List<WareHouseModel>();
                if (models?.Count > 0)
                {
                    models.ForEach(x =>
                    {
                        if (x.Inactive.Equals(false))
                        {
                            var m = new WareHouseModel
                            {
                                Code = x.Code,
                                Name = x.Name
                            };
                            res.Add(m);
                        }
                    });
                }

                return res;
            }
            return new List<WareHouseModel>();
        }

        public async Task<IPagedList<Core.Domain.Warehouse.WareHouseUser>> Get(WareHouseUserSearchContext ctx)
        {
            if (ctx is null)
                throw new ArgumentNullException(nameof(ctx));
            var query = from a in _repository.Table
                join p in _repositoryWh.Table on a.WarehouseId equals p.Id
                select new WareHouseUser()
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    WarehouseId = p.Name
                };
            if (ctx.Keywords.HasValue())
                query = from a in query
                    where a.UserId.Equals(ctx.Keywords)
                    select a;
            if (ctx.WareHouseId.HasValue())
            {
                var department = _repository.Table.FirstOrDefault(x => x.Id == ctx.WareHouseId);

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
                        await _repository.DataConnection.QueryToListAsync<string>(GetListChidren.ToString());
                    departmentIds.Add(ctx.WareHouseId);
                    if (departmentIds?.ToList().Count > 0)
                    {
                        var listDepartmentIds = departmentIds.ToList();
                        query = from p in query
                            where listDepartmentIds.Contains(p.WarehouseId)
                            select p;
                    }
                }
                else
                {
                    query = from p in query
                        where p.WarehouseId.Contains(ctx.WareHouseId.Trim())
                        select p;
                }
            }

            return new PagedList<WareHouseUser>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IQueryable<WareHouseUser> GetListRole(string idWareHouse)
        {
            if (idWareHouse is null)
                throw new ArgumentNullException(nameof(idWareHouse));
            return from a in _repository.Table
                where a.WarehouseId.Equals(idWareHouse)
                select new WareHouseUser
                {
                    UserId = a.UserId
                };
        }

        public async Task<int> DeletesAsync(string idUser, string idWareHouse)
        {
            if (idWareHouse is null || idUser is null)
                throw new ArgumentNullException("Tham so null");
            var user = _repository.Table.FirstOrDefault(x =>
                x.UserId.Equals(idUser) && x.WarehouseId.Equals(idWareHouse));
            if (user != null)
                return await _repository.DeleteAsync(user);
            return 0;
        }

        public async Task<bool> ExistAsync(string idUser)
        {
            if (string.IsNullOrEmpty(idUser))
                throw new NotImplementedException(nameof(idUser));
            var user = _repository.Table.FirstOrDefault(x => x.UserId.Equals(idUser));
            if (user != null)
                return true;
            return false;
        }
    }
}
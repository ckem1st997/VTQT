using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;
using System.Text;
using LinqToDB.Data;
using VTQT.Caching;
using VTQT.Core.Domain.FbmCrm;
using VTQT.Services.Dashboard;
using VTQT.Core.Domain.Dashboard;
using VTQT.SharedMvc.Dashboard.Models;

namespace VTQT.Services.Dashboard
{
    public partial class DashboardUserService : IDashboardUserService
    {
        private readonly IRepository<DashBoardUser> _repository;
        private readonly IRepository<TypeValue> _repositoryWh;
        private readonly IXBaseCacheManager _cacheManager;
        private readonly ITypeValueService _organizationalUnitService;


        public DashboardUserService(IXBaseCacheManager cacheManager, ITypeValueService organizationalUnitService)
        {
            _repository =
                EngineContext.Current.Resolve<IRepository<DashBoardUser>>(DataConnectionHelper.ConnectionStringNames
                    .Dashboard);
            _repositoryWh =
                EngineContext.Current.Resolve<IRepository<TypeValue>>(DataConnectionHelper.ConnectionStringNames
                    .Dashboard);
            _cacheManager = cacheManager;
            _organizationalUnitService = organizationalUnitService;
        }

        public async Task<long> InsertRangeAsync(IEnumerable<DashBoardUser> entities)
        {
            if (entities is null)
                throw new ArgumentNullException(nameof(entities));
            var result = await _repository.InsertAsync(entities);
            return result;
        }

        public async Task<int> UpdateRangeAsync(IEnumerable<DashBoardUser> entities)
        {
            if (entities is null)
                throw new ArgumentNullException(nameof(entities));
            var result = await _repository.UpdateAsync(entities);
            return result;
        }

        public async Task<DashBoardUser> GetByIdAsync(string id)
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

        public async Task<bool> ExistAsync(string idTypeValue, string idUser)
        {
            if (idTypeValue.IsEmpty() || idUser.IsEmpty())
                throw new ArgumentException("Param is null !");
            var check = await _repository.Table.AnyAsync(
                a => a.TypeValueId.Equals(idTypeValue)
                     && a.UserId.Equals(idUser));
            return check;
        }

        public async Task<List<TypeValueModel>> GetListByUser(string idUser)
        {
            var models = _organizationalUnitService.GetAll(false).ToList();

            StringBuilder sb = new StringBuilder();

            sb.Append("WITH RECURSIVE cte (Id, Name, ParentId) ");
            sb.Append("AS ");
            sb.Append("(SELECT ");
            sb.Append("      wh.Id, ");
            sb.Append("      wh.Name, ");
            sb.Append("      wh.ParentId ");
            sb.Append("    FROM TypeValue wh ");
            sb.Append("    INNER JOIN DashBoardUser whu ");
            sb.Append("    ON wh.Id = whu.TypeValueId ");
            sb.Append("    WHERE whu.UserId='" + idUser + "' ");
            sb.Append("  UNION ALL ");
            sb.Append("  SELECT ");
            sb.Append("    p.Id, ");
            sb.Append("    p.Name, ");
            sb.Append("    p.ParentId ");
            sb.Append("  FROM TypeValue p ");
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
                var res = new List<TypeValueModel>();
                if (models?.Count > 0)
                {
                    models.ForEach(x =>
                    {
                        if (x.Inactive.Equals(false))
                        {
                            var m = new TypeValueModel
                            {
                                Name = x.Name
                            };
                            res.Add(m);
                        }
                    });
                }

                return res;
            }
            return new List<TypeValueModel>();
        }

        public async Task<IPagedList<DashBoardUser>> Get(DashboardUserSearchContext ctx)
        {
            if (ctx is null)
                throw new ArgumentNullException(nameof(ctx));
            var query = from a in _repository.Table
                        join p in _repositoryWh.Table on a.TypeValueId equals p.Id
                        select new DashBoardUser()
                        {
                            Id = a.Id,
                            UserId = a.UserId,
                            TypeValueId = p.Name
                        };
            if (ctx.Keywords.HasValue())
                query = from a in query
                        where a.UserId.Equals(ctx.Keywords)
                        select a;
            if (ctx.TypeValueId.HasValue())
            {
                var department = _repository.Table.FirstOrDefault(x => x.Id == ctx.TypeValueId);

                if (department != null)
                {
                    StringBuilder GetListChidren = new StringBuilder();

                    GetListChidren.Append("with recursive cte (Id, Name, ParentId) as ( ");
                    GetListChidren.Append("  select     wh.Id, ");
                    GetListChidren.Append("             wh.Name, ");
                    GetListChidren.Append("             wh.ParentId ");
                    GetListChidren.Append("  from       TypeValue wh ");
                    GetListChidren.Append("  where      wh.ParentId='" + ctx.TypeValueId + "' ");
                    GetListChidren.Append("  union all ");
                    GetListChidren.Append("  SELECT     p.Id, ");
                    GetListChidren.Append("             p.Name, ");
                    GetListChidren.Append("             p.ParentId ");
                    GetListChidren.Append("  from       TypeValue  p ");
                    GetListChidren.Append("  inner join cte ");
                    GetListChidren.Append("          on p.ParentId = cte.id ");
                    GetListChidren.Append(") ");
                    GetListChidren.Append(" select cte.Id FROM cte GROUP BY cte.Id,cte.Name,cte.ParentId; ");
                    var departmentIds =
                        await _repository.DataConnection.QueryToListAsync<string>(GetListChidren.ToString());
                    departmentIds.Add(ctx.TypeValueId);
                    if (departmentIds?.ToList().Count > 0)
                    {
                        var listDepartmentIds = departmentIds.ToList();
                        query = from p in query
                                where listDepartmentIds.Contains(p.TypeValueId)
                                select p;
                    }
                }
                else
                {
                    query = from p in query
                            where p.TypeValueId.Contains(ctx.TypeValueId.Trim())
                            select p;
                }
            }

            return new PagedList<DashBoardUser>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IQueryable<DashBoardUser> GetListRole(string idTypeValue)
        {
            if (idTypeValue is null)
                throw new ArgumentNullException(nameof(idTypeValue));
            return from a in _repository.Table
                   where a.TypeValueId.Equals(idTypeValue)
                   select new DashBoardUser
                   {
                       UserId = a.UserId
                   };
        }

        public async Task<int> DeletesAsync(string idUser, string idTypeValue)
        {
            if (idTypeValue is null || idUser is null)
                throw new ArgumentNullException("Tham so null");
            var user = _repository.Table.FirstOrDefault(x =>
                x.UserId.Equals(idUser) && x.TypeValueId.Equals(idTypeValue));
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Domain.Dashboard;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Dashboard
{
    public class StorageValueService : IStorageValueService
    {
        private readonly IRepository<StorageValue> _repository;
        private readonly IRepository<TypeValue> _repositoryV;
        private readonly IWorkContext _workContext;


        #region Ctor

        public StorageValueService(IWorkContext workContext)
        {
            _repository =
                EngineContext.Current.Resolve<IRepository<StorageValue>>(DataConnectionHelper.ConnectionStringNames
                    .Dashboard);
            _repositoryV =
                EngineContext.Current.Resolve<IRepository<TypeValue>>(DataConnectionHelper.ConnectionStringNames
                    .Dashboard);
            _workContext = workContext;
        }

        #endregion

        public async Task<string> RunQueryAsync(string query)
        {
            if (query == null) 
                throw new ArgumentNullException(nameof(query));
            var res = await _repository.DataConnection.QueryToListAsync<string>(query);
            return res.FirstOrDefault();
        }    
        
        public async Task<object> RunQueryToObjectAsync(string query)
        {
            if (query == null) 
                throw new ArgumentNullException(nameof(query));
            var res = await _repository.DataConnection.QueryToListAsync<object>(query);
            return res;
        }
        public async Task<int> RunQueryCounttAsync(string query)
        {
            if (query == null) 
                throw new ArgumentNullException(nameof(query));
            var res = await _repository.DataConnection.QueryToListAsync<int>(query);
            return res.FirstOrDefault();
        }
        public async Task<int> InsertAsync(StorageValue entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var result = await _repository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(StorageValue entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var result = await _repository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeleteAsync(IEnumerable<string> ids)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));
            var result = await _repository.DeleteAsync(ids);
            return result;
        }

        public Task<bool> ExistAsync(string code)
        {
            throw new System.NotImplementedException();
        }

        public async Task<StorageValue> GetByIdAsync(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _repository.GetByIdAsync(id);

            return result;
        }

        public IList<StorageValue> GetAll(int assetType)
        {
            throw new System.NotImplementedException();
        }

        public IList<StorageValue> GetListDropDownd()
        {
            throw new System.NotImplementedException();
        }

        public async Task<IPagedList<StorageValue>> GetAsync(StorageValueSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _repository.Table select p;

            if (ctx.Keywords.HasValue())
            {
                query = from q in query
                    where q.Name.Contains(ctx.Keywords)
                    select q;
            }

            //if (ctx.TypeValueId.HasValue())
            //{
            //    query = from q in query
            //        where q.TypeValueId.Equals(ctx.TypeValueId)
            //        select q;
            //}

            if (ctx.TypeValueId.HasValue())
            {
                var department = _repositoryV.Table.FirstOrDefault(x => x.Id == ctx.TypeValueId);

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

            else
            {
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
                sb.Append("    WHERE whu.UserId='" + _workContext.UserId + "' ");
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
                    query = from p in query where listDepartmentIds.Contains(p.TypeValueId) select p;
                }
                else
                    return new PagedList<StorageValue>(new List<StorageValue>(), ctx.PageIndex, ctx.PageSize);


            }


            if (ctx.ToDate.HasValue)
            {
                query = from q in query
                    where q.VoucherDate >= ctx.ToDate
                    select q;
            }

            if (ctx.FromDate.HasValue)
            {
                query = from q in query
                    where q.VoucherDate <= ctx.FromDate
                    select q;
            }

            query =
                from p in query
                orderby p.CreatedDate
                select new StorageValue()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Status = p.Status,
                    ModifiedBy = p.ModifiedBy,
                    ModifiedDate = p.ModifiedDate,
                    TimeDay = p.TimeDay,
                    TimeMouth = p.TimeMouth,
                    TimeYear = p.TimeYear,
                    VoucherBy = p.VoucherBy,
                    VoucherDate = p.VoucherDate,
                    ModifiedByName = p.ModifiedByName,
                    VoucherByName = p.VoucherByName,
                    FileName = p.FileName,
                    FileContent = p.FileContent
                };

            return new PagedList<StorageValue>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<SelectListItem> GetMvcListItems(string idTypeValue)
        {
            if (string.IsNullOrEmpty(idTypeValue))
            {
                throw new ArgumentException($"'{nameof(idTypeValue)}' cannot be null or empty.", nameof(idTypeValue));
            }

            var results = new List<SelectListItem>();

            var query = from p in _repository.Table select p;
            query =
                from p in query
                where p.TypeValueId.Equals(idTypeValue)
                select p;

            if (query?.ToList()?.Count > 0)
            {
                query.ToList().ForEach(x =>
                {
                    var m = new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.Name
                    };
                    results.Add(m);
                });
            }

            return results;
        }

        public IList<StorageValue> GetByIdsAsync(IEnumerable<string> ids)
        {
            throw new System.NotImplementedException();
        }
    }
}
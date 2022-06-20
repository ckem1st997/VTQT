using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Domain.Dashboard;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using VTQT.SharedMvc.Dashboard.Models;
using LinqToDB.Data;
using VTQT.Caching;

namespace VTQT.Services.Dashboard
{
    public class TypeValueService : ITypeValueService
    {
        private readonly IRepository<TypeValue> _repository;
        private readonly IXBaseCacheManager _cacheManager;

        #region Ctor

        public TypeValueService(IXBaseCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            _repository =
                EngineContext.Current.Resolve<IRepository<TypeValue>>(DataConnectionHelper.ConnectionStringNames
                    .Dashboard);
        }

        #endregion

        public async Task<int> InsertAsync(TypeValue entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var result = await _repository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(TypeValue entity)
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
        public virtual async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _repository.Table
                .Where(w => ids.Contains(w.Id))
                .Set(x => x.Inactive, !active)
                .UpdateAsync();
            return result;
        }

        public IList<SelectListItem> GetMvcListItems()
        {
            var results = new List<SelectListItem>();

            var query = from p in _repository.Table select p;
            query =
                from p in query
                where !p.Inactive
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

        public Task<bool> ExistAsync(string code)
        {
            throw new System.NotImplementedException();
        }

        public async Task<TypeValue> GetByIdAsync(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _repository.GetByIdAsync(id);

            return result;
        }

        public IList<TypeValue> GetAll(bool show)
        {
            var query = from p in _repository.Table select p;
            if (!show)
                query =
                    from p in query
                    where !p.Inactive
                    select p;

            query =
                from p in query
                orderby p.Name
                select p;
            return query.ToList();
        }

        public IList<TypeValue> GetListDropDownd()
        {
            throw new System.NotImplementedException();
        }

        public IPagedList<TypeValue> Get(TypeValueSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _repository.Table select p;

            if (ctx.Keywords.HasValue())
            {
                query = from q in query
                        where q.Name.Contains(ctx.Keywords)
                        select q;
            }


            query =
                from p in query
                orderby p.Name
                select p;

            return new PagedList<TypeValue>(query, ctx.PageIndex, ctx.PageSize, query.Count());
        }


        public IList<TypeValue> GetByIdsAsync(IEnumerable<string> ids)
        {
            throw new System.NotImplementedException();
        }

        public string GetLastSelectedNodeTree(string appId, string userId, string path, string typeValueId)
        {
            var cacheKey = ModelCacheKeys.TypeValuesTreeModelCacheKey.FormatWith(appId, userId, path);
            return _cacheManager.HybridProvider.Get(cacheKey, () =>
            {
                return typeValueId;
            }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;
        }
    }
}
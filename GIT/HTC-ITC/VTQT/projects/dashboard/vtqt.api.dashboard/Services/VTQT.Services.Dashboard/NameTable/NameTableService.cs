using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VTQT.Core.Domain.Dashboard;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;
using LinqToDB;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Core.Domain;
using VTQT.Core.Domain.Master;
using VTQT.SharedMvc.Dashboard.Models;

namespace VTQT.Services.Dashboard
{
    public class NameTableService : INameTableService
    {
        private readonly IRepository<NameTableExist> _repository;

        public NameTableService()
        {
            _repository =
                EngineContext.Current.Resolve<IRepository<NameTableExist>>(DataConnectionHelper.ConnectionStringNames
                    .Dashboard);
        }
        public async Task<long> InsertAsync(NameTableExist entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var res = await _repository.InsertAsync(entity);
            return res;
        }

        public async Task<long> UpdateAsync(NameTableExist entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var res = await _repository.UpdateAsync(entity);
            return res;
        }

        public async Task<long> DeleteAsync(IEnumerable<string> ids)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));
            var res = await _repository.DeleteAsync(ids);
            return res;
        }

        public IList<SelectListItem> GetMvcListItems()
        {
            var results = new List<SelectListItem>();

            var query = from p in _repository.Table select p;

            if (query?.ToList()?.Count > 0)
            {
                query.ToList().ForEach(x =>
                {
                    var m = new SelectListItem
                    {
                        Value = x.Name,
                        Text = $"[{x.Name}] {x.NameDes}"
                    };
                    results.Add(m);
                });
            }

            return results;
        }

        public IPagedList<NameTableExist> Get(NameTableSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _repository.Table select p;
            if (!string.IsNullOrEmpty(ctx.Keywords))
                query = from p in query
                        where p.Name.Contains(ctx.Keywords)
                        select p;
            return new PagedList<NameTableExist>(query, ctx.PageIndex, ctx.PageSize);

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Dashboard;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Dashboard
{
    public class SelectTableService : ISelectTableService
    {
        private readonly IRepository<VTQT.Core.Domain.Dashboard.SelectTable> _repository;

        public SelectTableService()
        {
            _repository =
                EngineContext.Current.Resolve<IRepository<VTQT.Core.Domain.Dashboard.SelectTable>>(DataConnectionHelper.ConnectionStringNames
                    .Dashboard);
        }
        public async Task<long> DeleteAsync(IEnumerable<string> ids)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));
            var res = await _repository.DeleteAsync(ids);
            return res;
        }

        public IPagedList<SelectTable> Get(SelectTableSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _repository.Table select p;
            if (!string.IsNullOrEmpty(ctx.Keywords))
                query = from p in query
                        where p.TextShow.Contains(ctx.Keywords) || p.TableShow.Contains(ctx.Keywords)
                        select p;
            return new PagedList<SelectTable>(query, ctx.PageIndex, ctx.PageSize);

        }

        public async Task<Core.Domain.Dashboard.SelectTable> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public IList<Core.Domain.Dashboard.SelectTable> GetSelect(string NameTable)
        {
            if (string.IsNullOrEmpty(NameTable))
            {
                throw new ArgumentException($"'{nameof(NameTable)}' cannot be null or empty.", nameof(NameTable));
            }

            var res = _repository.Table.Where(x => x.TableShow.Equals(NameTable)).ToList();
            return res;
        }

        public async Task<long> InsertAsync(IEnumerable<Core.Domain.Dashboard.SelectTable> entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            var res = await _repository.InsertAsync(entity);
            return res;
        }

        public async Task<long> UpdateAsync(IEnumerable<Core.Domain.Dashboard.SelectTable> entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            var res = await _repository.UpdateAsync(entity);
            return res;
        }
    }
}

using LinqToDB;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Ticket
{
    public class NetworkLinkService : INetworkLinkService
    {
        #region Fields
        private readonly IRepository<NetworkLink> _networkLinkRepository;
        private readonly IRepository<Cable> _cableRepository;

        #endregion

        #region Ctor

        public NetworkLinkService()
        {
            _networkLinkRepository = EngineContext.Current.Resolve<IRepository<NetworkLink>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _cableRepository = EngineContext.Current.Resolve<IRepository<Cable>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }

        #endregion

        #region Methods

        public async Task<int> InsertAsync(NetworkLink entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _networkLinkRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(NetworkLink entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _networkLinkRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _networkLinkRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<NetworkLink> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _networkLinkRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<NetworkLink> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = await _networkLinkRepository.Table.FirstOrDefaultAsync(x => x.Code == code);

            return result;
        }

        public async Task<bool> ExistedAsync(string code)
        {
            return await _networkLinkRepository.Table
                .AnyAsync(x => x.Code != null &&
                          x.Code.Equals(code));
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _networkLinkRepository.Table
                         .Where(w => ids.Contains(w.Id))
                         .Set(x => x.Inactive, !active)
                         .UpdateAsync();

            return result;
        }

        #endregion

        #region List

        public IPagedList<NetworkLink> Get(NetworkLinkSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _networkLinkRepository.Table
                        join d in _cableRepository.Table on i.CableId equals d.Id into ps
                        from e in ps.DefaultIfEmpty()
                        select new NetworkLink
                        {
                            Id = i.Id,
                            Code = i.Code,
                            Name = i.Name,
                            StartPoint = i.StartPoint,
                            EndPoint = i.EndPoint,
                            CableId = e.Name,
                            AreaA = i.AreaA,
                            AreaB = i.AreaB,
                            Inactive = i.Inactive,
                        };

            if (ctx.Keywords.HasValue())
            {
                query = query.Where(x => x.Name.Contains(ctx.Keywords));
            }

            if (ctx.Status == (int)ActiveStatus.Activated)
            {
                query =
                    from p in query
                    where !p.Inactive
                    select p;
            }
            if (ctx.Status == (int)ActiveStatus.Deactivated)
            {
                query =
                    from p in query
                    where p.Inactive
                    select p;
            }

            query =
                from p in query
                orderby p.Code
                select p;

            return new PagedList<NetworkLink>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<NetworkLink> GetAll(bool showHidden = false)
        {
            var query = from p in _networkLinkRepository.Table select p;
            if (!showHidden)
            {
                query = from p in query where !p.Inactive select p;
            }
            query = from p in query orderby p.Name select p;
            return query.ToList();
        }

        public IList<SelectListItem> GetMvcListItems(bool showHidden)
        {
            var results = new List<SelectListItem>();

            var query = from p in _networkLinkRepository.Table select p;
            if (!showHidden)
            {
                query = from p in query
                        where !p.Inactive
                        select p;
            }

            query = from p in query
                    orderby p.Code
                    select p;

            if (query?.ToList()?.Count > 0)
            {
                query.ToList().ForEach(x =>
                {
                    var m = new SelectListItem
                    {
                        Value = x.Id,
                        Text = $"[{x.Code}] {x.Name}"
                    };
                    results.Add(m);
                });
            }

            return results;
        }
        #endregion
    }
}

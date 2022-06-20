using LinqToDB;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Ticket
{
    public partial class TicketReasonService : ITicketReasonService
    {
        #region Fields
        private readonly IRepository<TicketReason> _ticketReasonRepository;
        #endregion

        #region Ctor
        public TicketReasonService()
        {
            _ticketReasonRepository = EngineContext.Current.Resolve<IRepository<TicketReason>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(TicketReason entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _ticketReasonRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(TicketReason entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _ticketReasonRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _ticketReasonRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<TicketReason> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _ticketReasonRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<TicketReason> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = await _ticketReasonRepository.Table
                .FirstOrDefaultAsync(x => x.Code == code);

            return result;
        }

        public async Task<bool> ExistedAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            return await _ticketReasonRepository.Table
                .AnyAsync(x => x.Code != null &&
                          x.Code.Equals(code));
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            return await _ticketReasonRepository.Table
                .Where(x => ids.Contains(x.Id))
                .Set(x => x.Inactive, !active)
                .UpdateAsync();
        }
        #endregion

        #region List
        public IPagedList<TicketReason> Get(TicketReasonSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _ticketReasonRepository.Table select p;

            if (ctx.Keywords.HasValue())
            {
                query = from p in query
                        where p.Code.Contains(ctx.Keywords) ||
                              p.Description.Contains(ctx.Keywords)
                        select p;
            }
            if (ctx.Status == (int)ActiveStatus.Activated)
            {
                query = from p in query
                        where !p.Inactive
                        select p;
            }
            if (ctx.Status == (int)ActiveStatus.Deactivated)
            {
                query = from p in query
                        where p.Inactive
                        select p;
            }

            query = from p in query
                    orderby p.Code
                    select p;

            return new PagedList<TicketReason>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<SelectListItem> GetMvcListItems(bool showHidden)
        {
            var results = new List<SelectListItem>();

            var query = from p in _ticketReasonRepository.Table 
                        where p.ParentId == null
                        select p;
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
                        Text = $"[{x.Code}] {x.Description}"
                    };
                    results.Add(m);
                });
            }

            return results;
        }

        public IList<SelectListItem> GetDetailReasonsByReasonId(string reasonId)
        {
            var results = new List<SelectListItem>();

            var query = from p in _ticketReasonRepository.Table
                        where p.ParentId == reasonId && p.ParentId != null
                        select p;

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
                        Text = $"[{x.Code}] {x.Description}"
                    };
                    results.Add(m);
                });
            }

            return results;
        }

        public IList<TicketReason> GetAll(bool showHidden = false)
        {
            var query = from p in _ticketReasonRepository.Table select p;
            if (!showHidden)
            {
                query = from p in query where !p.Inactive select p;
            }
            query = from p in query orderby p.Code select p;
            return query.ToList();
        }

        public IList<SelectListItem> GetMvcList(bool showHidden, string projectId)
        {
            var results = new List<SelectListItem>();

            var query = from p in _ticketReasonRepository.Table
                        where p.ProjectId == projectId
                        select p;
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
                        Text = $"{x.Description}"
                    };
                    results.Add(m);
                });
            }

            return results;
        }

        public IList<TicketReason> GetAvailable(bool showHidden, string projectId)
        {
            var query = from p in _ticketReasonRepository.Table where p.ProjectId == projectId select p;
            if (!showHidden)
            {
                query = from p in query where !p.Inactive select p;
            }
            query = from p in query orderby p.Code select p;
            return query.ToList();
        }
        #endregion

        #region Utilities

        #endregion
    }
}

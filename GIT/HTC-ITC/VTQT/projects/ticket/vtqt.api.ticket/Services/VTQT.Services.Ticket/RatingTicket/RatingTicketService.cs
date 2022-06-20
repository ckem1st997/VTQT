using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Ticket
{
    public partial class RatingTicketService : IRatingTicketService
    {
        #region Fields
        private readonly IRepository<RatingTicket> _ratingTicketRepository;
        private readonly IRepository<Rating> _ratingRepository;
        private readonly IRepository<Status> _statusRepository;
        private readonly IRepository<Core.Domain.Ticket.Ticket> _ticketRepository;
        #endregion

        #region Ctor
        public RatingTicketService()
        {
            _ratingRepository = EngineContext.Current.Resolve<IRepository<Rating>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _ratingTicketRepository = EngineContext.Current.Resolve<IRepository<RatingTicket>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _ticketRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.Ticket>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _statusRepository = EngineContext.Current.Resolve<IRepository<Status>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(RatingTicket entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _ratingTicketRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(RatingTicket entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof (entity));
            }

            var result = await _ratingTicketRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                throw new ArgumentNullException(nameof(ids));
            }

            return await _ratingTicketRepository.DeleteAsync(ids);
        }

        public async Task<RatingTicket> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _ratingTicketRepository.GetByIdAsync(id);

            return result;
        }
        #endregion

        #region List
        public async Task<IPagedList<RatingTicket>> Get(RatingTicketSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from ratingTicket in _ratingTicketRepository.Table
                        join ticket in _ticketRepository.Table on ratingTicket.TicketId equals ticket.Id into a
                        from ratingTicketTicket in a.DefaultIfEmpty()
                        join rating in _ratingRepository.Table on ratingTicket.StarRating equals rating.Star into b
                        from rating in b.DefaultIfEmpty()                      
                        join status in _statusRepository.Table on ratingTicketTicket.Status equals status.Id into c
                        from ticketStatus in c.DefaultIfEmpty()
                        orderby ticketStatus.Az
                        select new RatingTicket
                        {
                            Id = ratingTicket.Id,
                            Note = ratingTicket.Note,
                            StarRating = ratingTicket.StarRating,
                            RatingBy = ratingTicket.RatingBy,
                            ModifiedBy = ratingTicket.ModifiedBy,
                            CreatedDate = ratingTicket.CreatedDate,
                            ModifiedDate = ratingTicket.ModifiedDate,
                            Ticket = new Core.Domain.Ticket.Ticket
                            {
                                Id = ratingTicketTicket.Id,
                                Code = ratingTicketTicket.Code,
                                Subject = ratingTicketTicket.Subject,
                                StartDate = ratingTicketTicket.StartDate,
                                FinishDate = ratingTicketTicket.FinishDate,
                                Note = ratingTicketTicket.Note,
                                ProjectId = ratingTicketTicket.ProjectId,
                                Status = ratingTicketTicket.Status,
                                Solution = ratingTicketTicket.Solution,
                                Detail = ratingTicketTicket.Detail,
                                Priority = ratingTicketTicket.Priority,
                                Assignee = ratingTicketTicket.Assignee,
                                CreatedBy = ratingTicketTicket.CreatedBy,
                                ModifiedBy= ratingTicketTicket.ModifiedBy,
                                CreatedDate = ratingTicketTicket.CreatedDate,
                                ModifiedDate= ratingTicketTicket.ModifiedDate,
                                FirstReason = ratingTicketTicket.FirstReason,
                                LastReason = ratingTicketTicket.LastReason                                
                            }
                        };

            if (ctx.Keywords.HasValue())
            {
                query = from p in query
                        where p.Ticket.Subject.Contains(ctx.Keywords) ||
                              p.Ticket.Code.Contains(ctx.Keywords) ||
                              p.Ticket.Detail.Contains(ctx.Keywords) ||
                              p.Ticket.Note.Contains(ctx.Keywords) ||
                              p.Ticket.Solution.Contains(ctx.Keywords) ||
                              p.Ticket.LastReason.Contains(ctx.Keywords) ||
                              p.Ticket.FirstReason.Contains(ctx.Keywords)
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.ProjectId))
            {
                query = from p in query
                        where p.Ticket.ProjectId == ctx.ProjectId
                        select p;
            }

            if (ctx.StartDate.HasValue)
            {
                query = from p in query
                        where p.Ticket.StartDate.Value >= ctx.StartDate.Value
                        select p;
            }

            if (ctx.FinishDate.HasValue)
            {
                query = from p in query
                        where p.Ticket.FinishDate.Value <= ctx.FinishDate.Value
                        select p;
            }

            return new PagedList<RatingTicket>(query, ctx.PageIndex, ctx.PageSize);
        }
        #endregion
    }
}

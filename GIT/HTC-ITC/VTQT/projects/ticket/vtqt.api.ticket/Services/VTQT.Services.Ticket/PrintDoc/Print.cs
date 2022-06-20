using System;
using System.Threading.Tasks;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;

namespace VTQT.Services.Ticket
{
    public class Print : IPrint
    {
        #region Fields
        private readonly IRepository<Core.Domain.Ticket.Status> _statusRepository;
        private readonly IRepository<Core.Domain.Ticket.CR> _crRepository;
        private readonly IRepository<Core.Domain.Ticket.CRCategory> _crCategoryRepository;
        private readonly IRepository<Core.Domain.Ticket.Priority> _priorityRepository;
        private readonly IRepository<Core.Domain.Ticket.Project> _projectRepository;
        private readonly IRepository<Core.Domain.Ticket.TicketCategory> _ticketCategoryRepository;
        private readonly IRepository<Core.Domain.Ticket.Ticket> _ticketRepository;

        #endregion

        #region Ctor

        public Print()
        {
            _statusRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.Status>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _crRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.CR>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _crCategoryRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.CRCategory>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _priorityRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.Priority>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _projectRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.Project>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _ticketCategoryRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.TicketCategory>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _ticketRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.Ticket>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }

        #endregion

        #region Method

        public async Task<Core.Domain.Ticket.CR> GetByIdToWordCRAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var query = from p in _crRepository.Table
                        join j in _crCategoryRepository.Table on p.Category equals j.Id into jn
                        from jt in jn.DefaultIfEmpty()
                        join k in _statusRepository.Table on p.Status equals k.Id into kn
                        from kt in kn.DefaultIfEmpty()
                        where p.Id.Equals(id)

                        select new Core.Domain.Ticket.CR
                        {
                            Id = p.Id,
                            Code = p.Code,
                            Name = p.Name,
                            Category = jt.Name,
                            Status = kt.Name,
                            CreatedBy = p.CreatedBy,
                            CreatedDate = p.CreatedDate,
                            ModifiedBy = p.ModifiedBy,
                            ModifiedDate = p.ModifiedDate,
                            Detail = p.Detail,
                            Note = p.Note,
                            Inactive = p.Inactive,
                            ProjectId = p.ProjectId,
                            StartDate = p.StartDate,
                            FinishDate = p.FinishDate,
                            ImplementationSteps = p.ImplementationSteps,
                            CrReason = p.CrReason,
                            FieldHandler = p.FieldHandler,
                            InfluenceChannel = p.InfluenceChannel
                        };

            return query.FirstOrDefault();
        }

        public async Task<Core.Domain.Ticket.Ticket> GetByIdToWordTicketAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var query = from p in _ticketRepository.Table
                        join j in _ticketCategoryRepository.Table on p.Category equals j.Id into jn
                        from jt in jn.DefaultIfEmpty()
                        join k in _statusRepository.Table on p.Status equals k.Id into kn
                        from kt in kn.DefaultIfEmpty()
                        join u in _projectRepository.Table on p.ProjectId equals u.Id into un
                        from ut in un.DefaultIfEmpty()
                        join l in _priorityRepository.Table on p.Priority equals l.Id into ln
                        from lt in ln.DefaultIfEmpty()
                        join e in _ticketCategoryRepository.Table on p.Category equals e.Id into en
                        from et in en.DefaultIfEmpty()
                        where p.Id.Equals(id)

                        select new Core.Domain.Ticket.Ticket
                        {
                            Id = p.Id,
                            Code = p.Code,
                            Subject = p.Subject,
                            Detail = p.Detail,
                            StartDate = p.StartDate,
                            FinishDate = p.FinishDate,
                            Deadline = p.Deadline,
                            Duration = p.Duration,
                            Status = kt.Name,
                            Priority = lt.Name,
                            ProjectId = ut.Name,
                            Category = et.Name,
                            CreatedDate = p.CreatedDate,
                            FirstReason = p.FirstReason,
                            LastReason = p.LastReason,
                            Note = p.Note,
                            Solution = p.Solution,
                            PendingHourTime = p.PendingHourTime
                        };

            return query.FirstOrDefault();
        }

        #endregion Method
    }
}
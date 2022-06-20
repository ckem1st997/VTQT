using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;

namespace VTQT.Services.Ticket
{
    public class InfrastructorFeeCRService : IInfrastructorFeeCRService
    {
        #region Fields
        private readonly IRepository<InfrastructorFeeCR> _infrastructorFeeCRRepository;
        #endregion

        #region Ctor
        public InfrastructorFeeCRService()
        {
            _infrastructorFeeCRRepository = EngineContext.Current.Resolve<IRepository<InfrastructorFeeCR>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(InfrastructorFeeCR entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _infrastructorFeeCRRepository.InsertAsync(entity);

            return result;
        }

        public async Task<long> InsertsAsync(IEnumerable<InfrastructorFeeCR> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _infrastructorFeeCRRepository.InsertAsync(entities);

            return result;
        }

        public async Task<int> UpdateAsync(InfrastructorFeeCR entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _infrastructorFeeCRRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> UpdatesAsync(IEnumerable<InfrastructorFeeCR> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _infrastructorFeeCRRepository.UpdateAsync(entities);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _infrastructorFeeCRRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<InfrastructorFeeCR> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _infrastructorFeeCRRepository.GetByIdAsync(id);

            return result;
        }
        #endregion

        #region List

        public IPagedList<InfrastructorFeeCR> Get(InfrastructorFeeCRSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _infrastructorFeeCRRepository.Table
                        where i.CrId == ctx.CrId
                        select i;

            if (ctx.Keywords.HasValue())
            {
                query = from p in query
                        where p.Name.Contains(ctx.Keywords)
                        select p;
            }

            query =
                from p in query
                orderby p.Name
                select p;

            return new PagedList<InfrastructorFeeCR>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<InfrastructorFeeCR> GetByCRIdAsync(string crId)
        {
            if (string.IsNullOrEmpty(crId))
            {
                throw new ArgumentNullException(nameof(crId));
            }

            var results = _infrastructorFeeCRRepository.Table
                .Where(x => x.CrId == crId);

            return results.ToList();
        }

        public IList<InfrastructorFeeCR> GetByInfrastructorFeeCRId(InfrastructorFeeCRSearchContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.CrId))
                throw new ArgumentNullException(nameof(ctx.CrId));

            var query =
                from x in _infrastructorFeeCRRepository.Table
                where x.CrId== ctx.CrId
                select x;

            return query.ToList();
        }
        #endregion

        #region Utilities

        #endregion
    }
}
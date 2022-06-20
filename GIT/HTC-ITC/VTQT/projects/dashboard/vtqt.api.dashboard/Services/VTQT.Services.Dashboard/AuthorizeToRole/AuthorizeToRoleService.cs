using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Dashboard;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;
using LinqToDB.Data;
using VTQT.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VTQT.Services.Dashboard
{
    public class AuthorizeToRoleService : IAuthorizeToRoleService
    {
        private readonly IRepository<AuthorizeToRole> _repository;

        public AuthorizeToRoleService()
        {
            _repository =
                EngineContext.Current.Resolve<IRepository<AuthorizeToRole>>(DataConnectionHelper.ConnectionStringNames
                    .Dashboard);
        }
        public async Task<long> InsertAsync(IEnumerable<AuthorizeToRole> entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var res = await _repository.InsertAsync(entity);
            return res;
        }

        public async Task<long> UpdateAsync(IEnumerable<AuthorizeToRole> entity)
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

        public async Task<AuthorizeToRole> GetByIdAsync(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            return await _repository.GetByIdAsync(id);
        }

        public IList<AuthorizeToRole> GetAll(string idTypeValue, string idDelegator)
        {
            if (string.IsNullOrEmpty(idTypeValue))
            {
                throw new ArgumentException($"'{nameof(idTypeValue)}' cannot be null or empty.", nameof(idTypeValue));
            }

            if (string.IsNullOrEmpty(idDelegator))
            {
                throw new ArgumentException($"'{nameof(idDelegator)}' cannot be null or empty.", nameof(idDelegator));
            }

            var query = from a in _repository.Table
                        where a.TypeValueId.Equals(idTypeValue) && a.DelegatorId.Equals(idDelegator)
                        select a;
            return query.ToList();
        }

        public Task<int> GetCountQuery()
        {
            throw new NotImplementedException();
        }

        public Task<IList<AuthorizeToRole>> GetObject()
        {
            throw new NotImplementedException();
        }

        public Task<IList<AuthorizeToRole>> GetList()
        {
            throw new NotImplementedException();
        }

        public async Task<IPagedList<AuthorizeToRole>> GetAllQuery(AuthorizeToRoleSearchContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var query = from p in _repository.Table select p;

            return new PagedList<AuthorizeToRole>(query, context.PageIndex, context.PageSize);

        }
    }
}
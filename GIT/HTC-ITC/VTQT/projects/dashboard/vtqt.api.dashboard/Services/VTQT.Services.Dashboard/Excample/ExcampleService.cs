using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Dashboard;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;

namespace VTQT.Services.Dashboard
{
    public class ExcampleService:IExcampleService
    {
        private readonly IRepository<Example> _repository;

        public ExcampleService()
        {
            _repository =
                EngineContext.Current.Resolve<IRepository<Example>>(DataConnectionHelper.ConnectionStringNames
                    .Dashboard);
        }
        
        public async Task<long> InsertAsync(IEnumerable<Example> entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var res = await _repository.InsertAsync(entity);
            return res;
        }

        public async Task<long> UpdateAsync(IEnumerable<Example> entity)
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

        public async Task<Example> GetByIdAsync(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            return await _repository.GetByIdAsync(id);
        }

        public IList<Example> GetAll()
        {
            var query = from a in _repository.Table
                select a;
            return query.ToList();
        }
    }
}
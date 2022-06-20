using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Dashboard;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;
using LinqToDB.Data;

namespace VTQT.Services.Dashboard
{
    public class DynamicService : IDynamicService
    {
        private readonly IRepository<Example> _repository;

        public DynamicService()
        {
            _repository =
                EngineContext.Current.Resolve<IRepository<Example>>(DataConnectionHelper.ConnectionStringNames
                    .Dashboard);
        }

        public async Task<List<object>> GetListAll(string table)
        {
            if (table == null || string.IsNullOrEmpty(table))
                throw new ArgumentNullException(nameof(table));
            try
            {
                var sql = "select * from " + table + "";
                DataParameter p1 = new DataParameter("@table", table);
                var res = await _repository.DataConnection.QueryToListAsync<object>(sql, p1);
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
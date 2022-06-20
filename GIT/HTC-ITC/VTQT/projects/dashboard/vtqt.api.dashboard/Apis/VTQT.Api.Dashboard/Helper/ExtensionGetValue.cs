using LinqToDB.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Dashboard;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Api.Dashboard.Helper
{
    public class ExtensionGetValue : IExtensionGetValue
    {
        private readonly IRepository<TypeValue> _repository;
        public ExtensionGetValue()
        {
           _repository =
                EngineContext.Current.Resolve<Data.IRepository<TypeValue>>(DataConnectionHelper
                    .ConnectionStringNames.Dashboard);
        }



        /// <summary>
        /// return all name table to database
        /// </summary>
        /// <param name="name">name datable</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public async Task<IList<GetColumnNameModel>> GetColumnName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }
            name = name.Replace("=", "");
            var sql = "SHOW COLUMNS FROM " + name+"";
            var res = await _repository.DataConnection.QueryToListAsync<GetColumnNameModel>(sql);
            return res;

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Data;
using VTQT.Core.Domain.Warehouse;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Warehouse
{
    public partial class AccObjectService : IAccObjectService
    {
        #region Fields
        private readonly IRepository<AccObject> _repository;
        private readonly IRepository<WareHouse> _warehouseRepository;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor
        public AccObjectService(IWorkContext workContext)
        {
              _repository = EngineContext.Current.Resolve<IRepository<AccObject>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _workContext = workContext;

        }
        #endregion

        #region Methods
        
        #endregion

        #region List

      

        #endregion

       
        public IList<SelectListItem> GetMvcListItems(bool showHidden = false)
        {
            var results = new List<SelectListItem>();

            var query = from p in _repository.Table select p;
            if (!showHidden)
                query =
                    from p in query
                    where !p.Inactive
                    select p;

            query =
                from p in query
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
        
    }
}

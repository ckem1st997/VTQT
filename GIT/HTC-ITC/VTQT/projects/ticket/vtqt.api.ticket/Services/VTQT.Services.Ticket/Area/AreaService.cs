using System.Collections.Generic;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VTQT.Services.Ticket
{
    public class AreaService : IAreaService
    {
        #region Fields

        private readonly IRepository<Area> _areaServiceRepository;

        #endregion

        #region Ctor

        public AreaService()
        {
            _areaServiceRepository = EngineContext.Current.Resolve<IRepository<Area>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }
        #endregion

        #region Methods        

        #endregion

        #region List
        public IList<SelectListItem> GetMvcListItems(bool showHidden)
        {
            var results = new List<SelectListItem>();

            var query = from p in _areaServiceRepository.Table select p;
            if (!showHidden)
            {
                query = from p in query
                        where !p.Inactive
                        select p;
            }

            if (query?.ToList()?.Count > 0)
            {
                query.ToList().ForEach(x =>
                {
                    var m = new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.Name
                    };
                    results.Add(m);
                });
            }

            return results;
        }
        #endregion
    }
}
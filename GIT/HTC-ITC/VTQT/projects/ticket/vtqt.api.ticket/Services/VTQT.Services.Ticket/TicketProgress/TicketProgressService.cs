using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Ticket
{
    public partial class TicketProgressService : ITicketProgressService
    {
        #region Fields
        private readonly IRepository<TicketProgress> _ticketProgressRepository;
        #endregion

        #region Ctor
        public TicketProgressService()
        {
            _ticketProgressRepository = EngineContext.Current.Resolve<IRepository<TicketProgress>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }
        #endregion

        #region Methods

        #endregion

        #region List
        public IList<SelectListItem> GetMvcListItems(bool showHidden)
        {
            var results = new List<SelectListItem>();

            var query = from p in _ticketProgressRepository.Table select p;
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
                        Text = $"{x.Name}"
                    };
                    results.Add(m);
                });
            }

            return results;
        }
        #endregion
    }
}

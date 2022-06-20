using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;

namespace VTQT.Services.Ticket
{
    public class ApprovalProgressService : IApprovalProgressService
    {
        #region Fields
        private readonly IRepository<ApprovalProgress> _approvalProgressRepository;
        #endregion

        #region Ctor
        public ApprovalProgressService()
        {
            _approvalProgressRepository = EngineContext.Current.Resolve<IRepository<ApprovalProgress>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }
        #endregion

        #region List
        public IList<SelectListItem> GetMvcListItems(bool showHidden)
        {
            var results = new List<SelectListItem>();

            var query = from p in _approvalProgressRepository.Table select p;
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
                        Text = $"[{x.Code}] {x.Name}"
                    };
                    results.Add(m);
                });
            }

            return results;
        }
        #endregion
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Collections.Generic;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Threading.Tasks;
using System;

namespace VTQT.Services.Asset
{
    public partial class DecreaseReasonService : IDecreaseReasonService
    {
        #region Fields
        private readonly IRepository<DecreaseReason> _decreaseReasonRepository;
        #endregion

        #region Ctor
        public DecreaseReasonService()
        {
            _decreaseReasonRepository = EngineContext.Current.Resolve<IRepository<DecreaseReason>>(DataConnectionHelper.ConnectionStringNames.Asset);
        }
        #endregion

        #region Methods
        public async Task<DecreaseReason> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var entity = await _decreaseReasonRepository.GetByIdAsync(id);

            return entity;
        }
        #endregion

        #region List
        public IList<SelectListItem> GetMvcListItem()
        {
            var query = from p in _decreaseReasonRepository.Table select p;

            var results = new List<SelectListItem>();

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

        #region Utilities

        #endregion
    }
}

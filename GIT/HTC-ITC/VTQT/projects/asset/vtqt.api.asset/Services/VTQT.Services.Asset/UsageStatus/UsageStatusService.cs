using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace VTQT.Services.Asset
{
    public partial class UsageStatusService : IUsageStatusService
    {
        #region Fields
        private readonly IRepository<UsageStatus> _usageStatusRepository;
        #endregion

        #region Ctor
        public UsageStatusService()
        {
            _usageStatusRepository = EngineContext.Current.Resolve<IRepository<UsageStatus>>(DataConnectionHelper.ConnectionStringNames.Asset);
        }
        #endregion

        #region Methods
        public async Task<UsageStatus> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var results = await _usageStatusRepository.GetByIdAsync(id);

            return results;
        }
        #endregion

        #region List
        /// <summary>
        /// Lấy danh sách trạng thái tài sản
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> GetMvcListItem()
        {
            var query = from p in _usageStatusRepository.Table select p;

            List<SelectListItem> results = new List<SelectListItem>();

            if (query?.ToList().Count > 0)
            {
                var usageStatus = query.ToList();
                usageStatus.ForEach(x =>
                {
                    var m = new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.Description
                    };
                    results.Add(m);
                });
            }

            return results;
        }
        #endregion
    }
}

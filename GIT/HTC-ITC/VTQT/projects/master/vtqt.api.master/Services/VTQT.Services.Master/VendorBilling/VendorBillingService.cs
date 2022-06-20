using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.FbmContract;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Master
{
    public partial class VendorBillingService : IVendorBillingService
    {
        #region Fields
        private readonly IIntRepository<Contractor> _vendorRepository;
        #endregion

        #region Ctor
        public VendorBillingService()
        {
            _vendorRepository = EngineContext.Current.Resolve<IIntRepository<Contractor>>(DataConnectionHelper.ConnectionStringNames.FbmContract);
        }
        #endregion

        #region Methods
        public IPagedList<Contractor> Get(VendorBillingSearchContext ctx)
        {
            var query = from p in _vendorRepository.Table where !p.IsBuyer select p;

            ctx.Keywords = ctx.Keywords?.Trim();

            if (ctx.Keywords != null && ctx.Keywords.HasValue())
            {
                query = from p in query
                        where (p.ContractorCode != null && p.ContractorCode.Contains(ctx.Keywords)) || 
                              (p.ContractorFullName != null && p.ContractorFullName.Contains(ctx.Keywords))
                        select p;
            }

            query = from p in query orderby p.ContractorCode select p;

            return new PagedList<Contractor>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<Contractor> GetAvailable()
        {
            var query = from p in _vendorRepository.Table where !p.IsBuyer select p;

            query = from p in query
                    orderby p.ContractorCode
                    select p;

            return query.ToList();
        }

        public async Task<Contractor> GetByIdAsync(int id)
        {
            var result = await _vendorRepository.GetByIdAsync(id);

            return result;
        }
        #endregion
    }
}

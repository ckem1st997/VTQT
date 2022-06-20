using System;
using System.Threading.Tasks;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;

namespace VTQT.Services.Asset
{
    public class Print : IPrint
    {
        #region Fields

        private readonly IRepository<Audit> _auditRepository;

        #endregion Fields

        #region Ctor

        public Print()
        {
            _auditRepository = EngineContext.Current.Resolve<IRepository<Audit>>(DataConnectionHelper.ConnectionStringNames.Asset);
        }

        #endregion

        #region Methods

        public async Task<Audit> GetByIdToWordAuditAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var query = from i in _auditRepository.Table
                        where i.Id.Equals(id)
                        select new Audit
                        {
                            Id = i.Id,
                            VoucherCode = i.VoucherCode,
                            VoucherDate = i.VoucherDate,
                            AuditLocation=i.AuditLocation,
                            Description = i.Description,
                            CreatedBy = i.CreatedBy,
                            CreatedDate = i.CreatedDate,
                            ModifiedDate = i.ModifiedDate,
                            AssetType=i.AssetType,
                        };
            return query.FirstOrDefault();
        }

        #endregion
    }
}
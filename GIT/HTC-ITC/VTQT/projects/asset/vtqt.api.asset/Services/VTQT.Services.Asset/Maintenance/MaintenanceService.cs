using System;
using System.Threading.Tasks;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Asset
{
    public partial class MaintenanceService : IMaintenanceService
    {
        #region Fields
        private readonly IRepository<Core.Domain.Asset.Maintenance> _maintenanceRepository;
        #endregion

        #region Ctor
        public MaintenanceService()
        {
            _maintenanceRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Asset.Maintenance>>(DataConnectionHelper.ConnectionStringNames.Asset);
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(Core.Domain.Asset.Maintenance entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _maintenanceRepository.InsertAsync(entity);

            return result;
        }
        #endregion

        #region List

        #endregion

        #region Utilities

        #endregion
    }
}

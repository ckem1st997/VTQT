using System.Collections.Generic;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;

namespace VTQT.Services.Ticket
{
    public class StationLevelService : IStationLevelService
    {
        #region Fields

        private readonly IRepository<StationLevel> _stationLevelServiceRepository;

        #endregion

        #region Ctor

        public StationLevelService()
        {
            _stationLevelServiceRepository = EngineContext.Current.Resolve<IRepository<StationLevel>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }

        #endregion

        #region Methods

        public IList<StationLevel> GetAll(bool showHidden = false)
        {
            var query = from p in _stationLevelServiceRepository.Table select p;
            if (!showHidden)
            {
                query = from p in query where !p.Inactive select p;
            }
            query = from p in query orderby p.Name select p;
            return query.ToList();
        }
        #endregion
    }
}
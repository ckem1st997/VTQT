using System.Collections.Generic;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface IStationLevelService
    {
        IList<StationLevel> GetAll(bool showHidden = false);
    }
}
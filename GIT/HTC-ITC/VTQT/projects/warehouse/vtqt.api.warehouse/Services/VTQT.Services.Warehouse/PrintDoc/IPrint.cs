using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Services.Warehouse
{
    public partial interface IPrint
    {
        Task<Outward> GetByIdToWordOutAsync(string id);
        Task<Inward> GetByIdToWordInAsync(string id);
        Task<IList<OutwardDetail>> GetByIdOutDetailsAsync(string id);

        Task<Audit> GetByIdToWordAuditAsync(string id);
    }
}

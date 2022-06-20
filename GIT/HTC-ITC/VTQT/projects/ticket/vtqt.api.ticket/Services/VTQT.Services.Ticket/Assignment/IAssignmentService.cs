using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface IAssignmentService
    {
        Task<int> InsertAsync(Assignment entity);

        Task<int> UpdateAsync(Assignment entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);
    }
}

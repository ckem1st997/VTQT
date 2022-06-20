using System;
using System.Collections.Generic;
using System.Text;
using VTQT.Core;
using VTQT.SharedMvc.Warehouse.Models;

namespace VTQT.Services.Warehouse
{
    public partial interface IReportService
    {
        IPagedList<ReportResponseModel> GetReport(ReportSearchContext ctx);

    }
}

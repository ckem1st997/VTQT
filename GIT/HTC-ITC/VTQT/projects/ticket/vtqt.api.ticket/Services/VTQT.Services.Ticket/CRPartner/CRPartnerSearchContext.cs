using System;

namespace VTQT.Services.Ticket
{
    public class CRPartnerSearchContext
    {
        public string Keywords { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string LanguageId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? FinishDate { get; set; }

        public string ProjectId { get; set; }

        public string CreatedBy { get; set; }
        public string Priority { get; set; }
        public string Trangthai { get; set; }
        public string Vungsuco { get; set; }

        public string CategoryId { get; set; }

        public string CrId { get; set; }

        public bool OverTimeRegister { get; set; }

        public DateTime? StartTimeAction { get; set; }

        public DateTime? RestoreTimeService { get; set; }
    }
}
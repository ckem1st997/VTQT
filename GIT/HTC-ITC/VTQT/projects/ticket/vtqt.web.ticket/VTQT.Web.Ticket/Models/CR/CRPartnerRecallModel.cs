using System;

namespace VTQT.Web.Ticket.Models.CR
{
    public class CRPartnerRecallModel : BaseStt
    {
        public string StartTimeAction { get; set; } // datetime

        public string? RestoreTimeService { get; set; } // datetime

        public int? HourTimeMinus { get; set; } // int

        public int? MinuteTimeMinus { get; set; } // int

        public int? SecondTimeMinus { get; set; } // int

        public bool OverTimeRegister { get; set; } // bit(1)

        public string Supervisor { get; set; } // varchar(36)
    }
}
﻿namespace VTQT.Services.Ticket
{
    public class ChannelTicketSearchContext
    {
        public string Keywords { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string LanguageId { get; set; }

        public string TicketId { get; set; }
    }
}
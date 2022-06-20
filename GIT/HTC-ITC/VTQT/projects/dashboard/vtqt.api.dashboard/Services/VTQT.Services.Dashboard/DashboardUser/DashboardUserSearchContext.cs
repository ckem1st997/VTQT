namespace VTQT.Services.Dashboard
{
    public class DashboardUserSearchContext
    {
        public string Keywords { get; set; }
        public string TypeValueId { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
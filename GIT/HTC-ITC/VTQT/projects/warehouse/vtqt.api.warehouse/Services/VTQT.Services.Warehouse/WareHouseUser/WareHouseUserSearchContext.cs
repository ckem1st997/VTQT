namespace VTQT.Services.Warehouse
{
    public class WareHouseUserSearchContext
    {
        public string Keywords { get; set; }
        public string WareHouseId { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
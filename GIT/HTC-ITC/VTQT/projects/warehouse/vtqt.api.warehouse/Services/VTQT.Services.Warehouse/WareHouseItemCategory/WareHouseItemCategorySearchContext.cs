namespace VTQT.Services.Warehouse
{
    public partial class WareHouseItemCategorySearchContext
    {
        public string Keywords { get; set; }

        public int Status { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string LanguageId { get; set; }
    }
}

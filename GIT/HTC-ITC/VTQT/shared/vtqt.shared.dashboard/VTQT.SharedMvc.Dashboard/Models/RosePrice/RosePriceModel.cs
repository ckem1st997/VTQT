using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Dashboard.Models
{
    public class RosePriceModel : BaseEntityModel
    {
        public string Loại { get; set; } // varchar(50)
        public string Miền { get; set; } // varchar(50)
        public string ĐốiTượngKýHĐ { get; set; } // varchar(50)
        public string TìnhTrạngHĐ { get; set; } // varchar(50)
        public string TênĐốiTác { get; set; } // varchar(100)
        public string SốHợpĐồng { get; set; } // varchar(100)
        public string KháchHàngĐầuRa { get; set; } // varchar(150)
        public string HĐĐầuRa { get; set; } // varchar(150)
        public string LoạiChiPhí { get; set; } // varchar(50)
        public string BPPhụTrách { get; set; } // varchar(50)
        public string ĐốiTượngKHĐầuRa { get; set; } // varchar(50)
        public string NgàyThanhLý_NghiệmThu { get; set; } // varchar(50)
        public string TỷLệ { get; set; } // varchar(50)
        public string NhómHH_PCDT { get; set; } // varchar(50)
        public string GiáTrịDoanhThuĐầuRaThángVNĐ { get; set; } // varchar(50)
        public decimal? Tháng1 { get; set; } // decimal(15,4)
        public decimal? Tháng2 { get; set; } // decimal(15,4)
        public decimal? Tháng3 { get; set; } // decimal(15,4)
        public decimal? Tháng4 { get; set; } // decimal(15,4)
        public decimal? Tháng5 { get; set; } // decimal(15,4)
        public decimal? Tháng6 { get; set; } // decimal(15,4)
        public decimal? Tháng7 { get; set; } // decimal(15,4)
        public decimal? Tháng8 { get; set; } // decimal(15,4)
        public decimal? Tháng9 { get; set; } // decimal(15,4)
        public decimal? Tháng10 { get; set; } // decimal(15,4)
        public decimal? Tháng11 { get; set; } // decimal(15,4)
        public decimal? Tháng12 { get; set; } // decimal(15,4)
        public decimal? Năm2021 { get; set; } // decimal(15,4)
    }
}
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VTQT.Core.Domain.Localization
{
    /// <summary>Danh sách Ngôn ngữ hệ thống</summary>
    // NotMapped: để tránh lỗi "Invalid column name 'Discriminator'" khi query paging ToList
    [NotMapped]
    [Serializable]
    public class LanguageSvcEntity : BaseEntity //: Language
    {
        /// <summary>Tên ngôn ngữ</summary>
        public string Name { get; set; }

        /// <summary>Mã ngôn ngữ địa phương (VD: en-US, vi-VN)</summary>
        public string LanguageCulture { get; set; }

        /// <summary>Mã ngôn ngữ ISO 2 ký tự (VD: en, vi)</summary>
        public string UniqueSeoCode { get; set; }

        /// <summary>Tên file cờ quốc gia</summary>
        public string FlagImageFileName { get; set; }

        /// <summary>Bố cục phải-qua-trái</summary>
        public bool Rtl { get; set; }

        /// <summary>Giới hạn cho các App</summary>
        public bool LimitedToApps { get; set; }

        /// <summary>Tiền tệ mặc định (Ref: Currency)</summary>
        public string DefaultCurrencyId { get; set; }

        /// <summary>Phát hành</summary>
        public bool Published { get; set; }

        /// <summary>Thứ tự hiển thị</summary>
        public int DisplayOrder { get; set; }
    }
}

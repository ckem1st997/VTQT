using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VTQT.Core.Domain.Apps
{
    /// <summary>Danh sách Ứng dụng</summary>
    // NotMapped: để tránh lỗi "Invalid column name 'Discriminator'" khi query paging ToList
    [NotMapped]
    [Serializable]
    public class AppSvcEntity : BaseEntity //: App
    {
        /// <summary>Loại ứng dụng</summary>
        public string AppType { get; set; }

        /// <summary>Tên ứng dụng (đa ngôn ngữ)</summary>
        public string Name { get; set; }

        /// <summary>Tên viết tắt</summary>
        public string ShortName { get; set; }

        /// <summary>Mô tả (đa ngôn ngữ)</summary>
        public string Description { get; set; }

        /// <summary>Icon</summary>
        public string Icon { get; set; }

        /// <summary>Màu nền</summary>
        public string BackgroundColor { get; set; }

        /// <summary>Đường dẫn</summary>
        public string Url { get; set; }

        /// <summary>Hosts</summary>
        public string Hosts { get; set; }

        /// <summary>Kích hoạt SSL</summary>
        public bool SslEnabled { get; set; }

        /// <summary>Cdn Url</summary>
        public string CdnUrl { get; set; }

        /// <summary>Ngôn ngữ mặc định (Ref: Language)</summary>
        public string DefaultLanguageId { get; set; }

        /// <summary>Hiển thị lên menu</summary>
        public bool ShowOnMenu { get; set; }

        /// <summary>Thứ tự hiển thị</summary>
        public int DisplayOrder { get; set; }
    }
}

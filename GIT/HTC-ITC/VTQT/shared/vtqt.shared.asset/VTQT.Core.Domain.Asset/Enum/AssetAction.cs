using System.ComponentModel;

namespace VTQT.Core.Domain.Asset
{
    /// <summary>
    /// Hoạt động với tài sản
    /// </summary>
    public enum AssetAction
    {
        /// <summary>
        /// Cấp phát
        /// </summary>
        [Description("Cấp phát")]
        Allocation = 10,
        /// <summary>
        /// Sửa chữa
        /// </summary>
        [Description("Sửa chữa")]
        Repair = 20,
        /// <summary>
        /// Bảo dưỡng
        /// </summary>
        [Description("Bảo dưỡng")]
        Maintenance = 30,
        /// <summary>
        /// Thu hồi
        /// </summary>
        [Description("Thu hồi")]
        Recall = 40,
        /// <summary>
        /// Thanh lý
        /// </summary>
        [Description("Thanh lý")]
        Sold = 50,
        /// <summary>
        /// Hỏng
        /// </summary>
        [Description("Hỏng")]
        Broken = 60,
        /// <summary>
        /// Khác
        /// </summary>
        [Description("Khác")]
        Other = 9999
    }
}

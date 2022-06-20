using System.ComponentModel;

namespace VTQT.Core.Domain.Warehouse.Enum
{
    /// <summary>
    /// Outward reason
    /// </summary>
    public enum OutwardReason
    {
        /// <summary>
        /// Cấp phát tài sản hành chính
        /// </summary>
        [Description("Cấp phát tài sản hành chính")]
        Office = 10,
        /// <summary>
        /// Cấp phát tài sản trạm
        /// </summary>
        [Description("Cấp phát tài sản trạm")]
        Station = 20,
        /// <summary>
        /// Cấp phát tài sản dự án
        /// </summary>
        [Description("Cấp phát tài sản dự án")]
        Project = 30,
        /// <summary>
        /// Cấp phát tài sản khách hàng
        /// </summary>
        [Description("Cấp phát tài sản khách hàng")]
        Customer = 40,
        /// <summary>
        /// Cấp vật tư
        /// </summary>
        [Description("Cấp vật tư")]
        Items = 50,
        /// <summary>
        /// Cấp công cụ dụng cụ
        /// </summary>
        [Description("Cấp công cụ dụng cụ")]
        Tool = 60,
        /// <summary>
        /// Chuyển kho
        /// </summary>
        [Description("Chuyển kho")]
        ChangeWarehouse = 70,
        /// <summary>
        /// Khác
        /// </summary>
        [Description("Khác")]
        Other = 9999
    }
}
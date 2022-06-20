using System.ComponentModel;

namespace VTQT.Core.Domain.Master.Enum
{
    /// <summary>
    /// Loại báo cáo
    /// </summary>
    public enum ReportType
    {
        /// <summary>
        /// Tồn kho
        /// </summary>
        [Description("Tồn kho")]
        Balance = 1,
        /// <summary>
        /// Tổng hợp nhập kho
        /// </summary>
        [Description("Nhập kho")]
        InwardTotal = 2,
        /// <summary>
        /// Tổng hợp xuất kho
        /// </summary>
        [Description("Xuất kho")]
        OutwardTotal = 3,
        /// <summary>
        /// Chi tiết nhập kho
        /// </summary>
        [Description("Nhập kho")]
        InwardDetail = 4,
        /// <summary>
        /// Chi tiết xuất kho
        /// </summary>
        [Description("Xuất kho")]
        OutwardDetail = 5
    }
}

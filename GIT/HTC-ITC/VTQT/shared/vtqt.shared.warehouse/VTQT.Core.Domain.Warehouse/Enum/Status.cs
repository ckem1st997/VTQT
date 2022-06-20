using System.ComponentModel;

namespace VTQT.Core.Domain.Warehouse.Enum
{
    public enum Status
    {
        /// <summary>
        /// Mới
        /// </summary>
        [Description("Mới")]
        New = 1,

        /// <summary>
        /// Đã Duyệt
        /// </summary>
        [Description("Đã duyệt")]
        Used = 2,

        /// <summary>
        /// Đã thực hiện
        /// </summary>
        [Description("Đã thực hiện")]
        Good = 3,
    }
}
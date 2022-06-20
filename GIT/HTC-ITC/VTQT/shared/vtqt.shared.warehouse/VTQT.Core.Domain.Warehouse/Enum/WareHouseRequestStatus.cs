using System.ComponentModel;

namespace VTQT.Core.Domain.Warehouse.Enum
{
    public enum WareHouseRequestStatus
    {
        /// <summary>
        /// Đã Xuất Kho
        /// </summary>
        [Description("Đã Xuất Kho")]
        New = 1,

        /// <summary>
        /// Tán Thành
        /// </summary>
        [Description("Approved")]
        Used = 2,

        /// <summary>
        /// Trưởng bộ phận kho phê duyệt
        /// </summary>
        [Description("Trưởng bộ phận kho phê duyệt")]
        Good = 3,

        /// <summary>
        /// Đã nhận đủ vật tư
        /// </summary>
        [Description("Đã nhận đủ vật tư")]
        Broken = 4,

        /// <summary>
        /// Trả vật tư thừa
        /// </summary>
        [Description("Trả vật tư thừa")]
        Sold = 5
    }
}
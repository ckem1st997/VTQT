using System.ComponentModel;

namespace VTQT.Core.Domain.Warehouse.Enum
{
    /// <summary>
    /// Reason Inward
    /// </summary>
    public enum InwardReason
    {
        /// <summary>
        /// Mua hàng
        /// </summary>
        [Description("Mua hàng")]
        Purchase = 10,
        /// <summary>
        /// Thu hồi
        /// </summary>
        [Description("Thu hồi")]
        Revoke = 20,
        /// <summary>
        /// Trả lại
        /// </summary>
        [Description("Trả lại")]
        Return = 30,
        /// <summary>
        /// Khác
        /// </summary>
        [Description("Khác")]
        Other = 9999
    }
}
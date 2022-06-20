using System.ComponentModel;

namespace VTQT.Core.Domain.Warehouse.Enum
{
    /// <summary>
    /// Loại phiếu kho 
    /// </summary>
    public enum VoucherType
    {
        /// <summary>
        /// Phiếu nhập
        /// </summary>
        [Description("Phiếu nhập")]
        Inward = 1,
        /// <summary>
        /// Phiếu xuất
        /// </summary>
        [Description("Phiếu xuất")]
        Outward = 2
    }
}
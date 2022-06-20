using System.ComponentModel;

namespace VTQT.Core.Domain.Asset.Enum
{
    /// <summary>
    /// Mốc thời gian
    /// </summary>
    public enum Duration
    {
        /// <summary>
        /// Ngày
        /// </summary>
        [Description("Ngày")]
        Date = 1,
        /// <summary>
        /// Tháng
        /// </summary>
        [Description("Tháng")]
        Month = 2,
        /// <summary>
        /// Năm
        /// </summary>
        [Description("Năm")]
        Year = 3
    }
}

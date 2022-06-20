using System.ComponentModel;

namespace VTQT.Core.Domain.Asset.Enum
{
    /// <summary>
    /// Loại tài sản
    /// </summary>
    public enum AssetType
    {
        /// <summary>
        /// Tài sản hành chính
        /// </summary>
        [Description("Tài sản hành chính")]
        Office = 10,
        /// <summary>
        /// Tài sản hạ tầng
        /// </summary>
        [Description("Tài sản hạ tầng")]
        Infrastructure = 20,
        /// <summary>
        /// Tài sản dự án
        /// </summary>
        [Description("Tài sản dự án")]
        Project = 30
    }
}

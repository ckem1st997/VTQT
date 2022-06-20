using VTQT.Core.Configuration;

namespace VTQT.Core.Domain.Common
{
    /// <summary>
    /// Catalog settings
    /// </summary>
    public class CatalogSettings : ISettings
    {
        public CatalogSettings()
        {
            HtmlTextCollapsedHeight = 260;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore "limit per store" rules (side-wide). It can significantly improve performance when enabled.
        /// </summary>
        public bool IgnoreAppLimitations { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating if html long text should be collapsed
        /// </summary>
        public bool EnableHtmlTextCollapser { get; set; }

        /// <summary>
        /// Gets or sets the height of collapsed text
        /// </summary>
        public int HtmlTextCollapsedHeight { get; set; }
    }
}

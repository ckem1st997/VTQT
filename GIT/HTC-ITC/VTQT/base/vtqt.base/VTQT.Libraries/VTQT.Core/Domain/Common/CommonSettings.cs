using System.Collections;
using VTQT.Core.Configuration;

namespace VTQT.Core.Domain
{
    public class AdminAreaSettings : ISettings
    {
        public AdminAreaSettings()
        {
            GridPageSize = 50;
            GridButtonCount = 5;
            GridPageSizeOptions = new[] { "50", "100", "200", "500" };
            RichEditorFlavor = "RichEditor";
        }

        public int GridPageSize { get; set; }

        public int GridButtonCount { get; set; }

        public IEnumerable GridPageSizeOptions { get; set; }

        public string RichEditorFlavor { get; set; }
    }

    /// <summary>
    /// Common settings
    /// </summary>
    public class CommonSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to compress response (gzip by default). 
        /// You may want to disable it, for example, If you have an active IIS Dynamic Compression Module configured at the server level
        /// </summary>
        public bool UseResponseCompression { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable markup minification
        /// </summary>
        public bool EnableHtmlMinification { get; set; }

        /// <summary>
        /// A value indicating whether JS file bundling and minification is enabled
        /// </summary>
        public bool EnableJsBundling { get; set; }

        /// <summary>
        /// A value indicating whether CSS file bundling and minification is enabled
        /// </summary>
        public bool EnableCssBundling { get; set; }
    }

    public class GetDataSettings : ISettings
    {
        public int TopSize { get; set; } = 10;

        public int MaxTopSize { get; set; } = 50;
    }
}

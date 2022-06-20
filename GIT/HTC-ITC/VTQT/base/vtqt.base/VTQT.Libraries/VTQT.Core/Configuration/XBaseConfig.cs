namespace VTQT.Core.Configuration
{
    public partial class XBaseConfig
    {
        public const string XBase = "XBase";

        /// <summary>
        /// Gets or sets a value indicating whether to display the full error in production environment.
        /// It's ignored (always enabled) in development environment
        /// </summary>
        public bool DisplayFullErrorStack { get; set; }

        /// <summary>
        /// Loại Project App (AppProjectType)
        /// </summary>
        public string AppProjectType { get; set; }

        /// <summary>
        /// App Type (AppHelperBase)
        /// </summary>
        public string AppType { get; set; }

        public bool UseAuthentication { get; set; }

        public string RedisDataProtectionConnection { get; set; }

        public string RedisSessionConnection { get; set; }

        public string AppAssembliesPath { get; set; } = "/wwwroot/AppData/AppAssemblies";
    }
}

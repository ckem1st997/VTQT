namespace VTQT.Core.Domain.Apps
{
    /// <summary>
    /// Represents an entity which supports store mapping
    /// </summary>
    public partial interface IAppMappingSupported
    {
        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        bool LimitedToApps { get; set; }
    }
}

using VTQT.Core.Configuration;

namespace VTQT.Core.Domain.Directory
{
    /// <summary>
    /// Measure settings
    /// </summary>
    public class MeasureSettings : ISettings
    {
        /// <summary>
        /// Base dimension identifier
        /// </summary>
        public string BaseDimensionId { get; set; }

        /// <summary>
        /// Base weight identifier
        /// </summary>
        public string BaseWeightId { get; set; }
    }
}

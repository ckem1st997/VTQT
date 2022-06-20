using VTQT.Core.Configuration;

namespace VTQT.Core.Domain.Directory
{
    /// <summary>
    /// Currency settings
    /// </summary>
    public class CurrencySettings : ISettings
    {
        /// <summary>
        /// Primary currency identifier
        /// </summary>
        public string PrimaryCurrencyId { get; set; }

        /// <summary>
        ///  Primary exchange rate currency identifier
        /// </summary>
        public string PrimaryExchangeRateCurrencyId { get; set; }
    }
}

namespace VTQT.Core.Localization
{
    public static class NullLocalizer
    {
        private static readonly Localizer _instance;

        static NullLocalizer()
        {
            _instance = (format, args) => new LocalizedString((args == null || args.Length == 0) ? format : string.Format(format, args));
        }

        public static Localizer Instance => _instance;
    }
}

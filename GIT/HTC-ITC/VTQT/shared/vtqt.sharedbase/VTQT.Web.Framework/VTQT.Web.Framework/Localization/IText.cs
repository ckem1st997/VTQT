using VTQT.Core.Localization;

namespace VTQT.Web.Framework.Localization
{
    public interface IText
    {
        LocalizedString Get(string key, params object[] args);
    }
}

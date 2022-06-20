using Microsoft.AspNetCore.Mvc.Rendering;

namespace VTQT.Web.Framework.UI
{
    public static class HtmlHelperExtensions
    {
        public static ComponentFactory XBase(this IHtmlHelper helper)
        {
            return new ComponentFactory(helper);
        }
    }
}

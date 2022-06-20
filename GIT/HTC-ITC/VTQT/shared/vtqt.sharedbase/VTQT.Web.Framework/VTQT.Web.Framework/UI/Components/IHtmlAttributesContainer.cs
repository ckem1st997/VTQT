using System.Collections.Generic;

namespace VTQT.Web.Framework.UI
{
    public interface IHtmlAttributesContainer
    {
        IDictionary<string, object> HtmlAttributes
        {
            get;
        }
    }

}

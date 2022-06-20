using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor;

namespace VTQT.Web.Framework.UI
{
    public interface IContentContainer
    {
        IDictionary<string, object> ContentHtmlAttributes
        {
            get;
        }

        HelperResult Content
        {
            get;
            set;
        }
    }

}

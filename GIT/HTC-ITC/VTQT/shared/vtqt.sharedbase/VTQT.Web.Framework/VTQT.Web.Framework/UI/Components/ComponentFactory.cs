using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VTQT.Web.Framework.UI
{
    public class ComponentFactory : IHideObjectMembers
    {
        public ComponentFactory(IHtmlHelper helper)
        {
            this.HtmlHelper = helper;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IHtmlHelper HtmlHelper
        {
            get;
            set;
        }

        #region Builders

        public virtual TabStripBuilder TabStrip()
        {
            return new TabStripBuilder(new TabStrip(), this.HtmlHelper);
        }

        #endregion

    }
}

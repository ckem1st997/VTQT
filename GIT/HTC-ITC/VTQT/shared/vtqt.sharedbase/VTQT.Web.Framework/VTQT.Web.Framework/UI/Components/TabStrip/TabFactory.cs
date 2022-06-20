using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VTQT.Web.Framework.UI
{
    public class TabFactory : IHideObjectMembers
    {
        private readonly IList<Tab> _items;
        private readonly IHtmlHelper _htmlHelper;

        public TabFactory(IList<Tab> items, IHtmlHelper htmlHelper)
        {
            Guard.ArgumentNotNull(() => htmlHelper);

            _items = items;
            _htmlHelper = htmlHelper;
        }

        public virtual TabBuilder Add()
        {
            var item = new Tab();
            _items.Add(item);
            return new TabBuilder(item, _htmlHelper);
        }
    }
}

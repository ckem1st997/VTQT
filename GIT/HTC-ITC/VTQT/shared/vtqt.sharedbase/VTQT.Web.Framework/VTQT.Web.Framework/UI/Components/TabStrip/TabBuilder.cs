using Microsoft.AspNetCore.Mvc.Rendering;

namespace VTQT.Web.Framework.UI
{

    public class TabBuilder : NavigationItemtWithContentBuilder<Tab, TabBuilder>
    {

        public TabBuilder(Tab item, IHtmlHelper htmlHelper)
            : base(item, htmlHelper)
        {
        }

        public TabBuilder Name(string value)
        {
            this.Item.Name = value;
            return this;
        }

        public TabBuilder Pull(TabPull value)
        {
            this.Item.Pull = value;
            return this;
        }


    }

}

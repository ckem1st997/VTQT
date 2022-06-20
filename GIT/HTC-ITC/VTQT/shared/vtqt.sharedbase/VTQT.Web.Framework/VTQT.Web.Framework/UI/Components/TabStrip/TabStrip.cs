using System.Collections.Generic;

namespace VTQT.Web.Framework.UI
{
    public enum TabsPosition
    {
        Top,
        Right,
        Below,
        Left
    }

    public enum TabsStyle
    {
        Tabs,
        Pills
    }

    public class TabStrip : Component
    {

        public TabStrip()
        {
            this.Items = new List<Tab>();
            this.Fade = true;
            this.XBaseTabSelection = true;
        }

        public List<Tab> Items
        {
            get;
            private set;
        }

        public TabsPosition Position
        {
            get;
            set;
        }

        public TabsStyle Style
        {
            get;
            set;
        }

        public bool Stacked
        {
            get;
            set;
        }

        public bool Fade
        {
            get;
            set;
        }

        public bool XBaseTabSelection
        {
            get;
            set;
        }

        public string OnAjaxBegin
        {
            get;
            set;
        }

        public string OnAjaxSuccess
        {
            get;
            set;
        }

        public string OnAjaxFailure
        {
            get;
            set;
        }

        public string OnAjaxComplete
        {
            get;
            set;
        }

        public override bool NameIsRequired
        {
            get
            {
                return true;
            }
        }


    }

}

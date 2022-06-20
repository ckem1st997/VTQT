﻿namespace VTQT.Web.Framework.UI
{
    public interface IUiComponent : IHtmlAttributesContainer
    {
        string Id
        {
            get;
        }

        string Name
        {
            get;
        }

        bool NameIsRequired
        {
            get;
        }
    }

}

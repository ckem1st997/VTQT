using System;

namespace VTQT.Web.Framework.Security
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AppApiControllerAttribute : Attribute
    {
        public string ResourceKey { get; set; }

        public int Order { get; set; }

        public AppApiControllerAttribute(string resourceKey)
        {
            ResourceKey = resourceKey;
        }

        public AppApiControllerAttribute(string resourceKey, int order)
        {
            ResourceKey = resourceKey;
            Order = order;
        }
    }
}

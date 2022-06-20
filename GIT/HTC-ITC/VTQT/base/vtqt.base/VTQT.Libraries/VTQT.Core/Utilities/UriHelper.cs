using System;

namespace VTQT.Utilities
{
    public static class UriHelper
    {
        public static Uri Combine(string baseUri, string relativeUri)
        {
            return new Uri(new Uri(baseUri), relativeUri);
        }
    }
}

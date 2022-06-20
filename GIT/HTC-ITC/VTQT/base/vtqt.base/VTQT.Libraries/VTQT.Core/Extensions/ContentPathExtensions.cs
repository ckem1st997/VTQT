using System;
using VTQT.Utilities;

namespace VTQT
{
    public static class ContentPathExtensions
    {
        public static Uri CombineRelativeUri(this string baseUri, string relativeUri)
        {
            return UriHelper.Combine(baseUri, relativeUri);
        }

        public static Uri CombineBaseUri(this string relativeUri, string baseUri)
        {
            return UriHelper.Combine(baseUri, relativeUri);
        }
    }
}

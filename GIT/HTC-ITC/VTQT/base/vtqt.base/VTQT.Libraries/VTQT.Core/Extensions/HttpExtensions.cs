using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using VTQT.Core;
using VTQT.Core.Infrastructure;

namespace VTQT
{
    public static class HttpExtensions
    {
        const string CacheRegionName = "XBase:";
        const string RememberPathKey = "AppRelativeCurrentExecutionFilePath.Original";

        private static readonly List<Tuple<string, string>> _sslHeaders = new List<Tuple<string, string>>
        {
            new Tuple<string, string>("HTTP_CLUSTER_HTTPS", "on"),
            new Tuple<string, string>("HTTP_X_FORWARDED_PROTO", "https"),
            new Tuple<string, string>("X-Forwarded-Proto", "https"),
            new Tuple<string, string>("x-arr-ssl", null),
            new Tuple<string, string>("X-Forwarded-Protocol", "https"),
            new Tuple<string, string>("X-Forwarded-Ssl", "on"),
            new Tuple<string, string>("X-Url-Scheme", "https")
        };

        /// <summary>
        /// Returns wether the specified url is local to the host or not
        /// </summary>
        /// <param name="request"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsAppLocalUrl(this HttpRequest request, string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            url = url.Trim();

            if (url.StartsWith("~/"))
            {
                return true;
            }

            if (url.StartsWith("//") || url.StartsWith("/\\"))
            {
                return false;
            }

            // At this point when the url starts with "/" it is local
            if (url.StartsWith("/"))
            {
                return true;
            }

            // At this point, check for a fully qualified url
            try
            {
                var uri = new Uri(url);

                if (!uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) && !uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (uri.Authority.Equals(request.Headers["Host"], StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                // Finally, check the base url from the settings
                var appContext = EngineContext.Current.Resolve<IAppContext>();
                if (appContext != null)
                {
                    var baseUrl = appContext.CurrentApp.Url;
                    if (baseUrl.HasValue())
                    {
                        if (uri.Authority.Equals(new Uri(baseUrl).Authority, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            catch
            {
                // mall-formed url e.g, "abcdef"
                return false;
            }
        }

        /// <summary>
        /// Gets a value which indicates whether the HTTP connection uses secure sockets (HTTPS protocol). 
        /// Works with Cloud's load balancers.
        /// </summary>
        public static bool IsHttps(this HttpRequest request)
        {
            if (request.IsHttps)
            {
                return true;
            }

            foreach (var tuple in _sslHeaders)
            {
                var serverVar = request.HttpContext.GetServerVariable(tuple.Item1);
                if (serverVar != null)
                {
                    return tuple.Item2 == null || tuple.Item2.Equals(serverVar, StringComparison.OrdinalIgnoreCase);
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a value which indicates whether the current request requests a static resource, like .txt, .pdf, .js, .css etc.
        /// </summary>
        public static bool IsStaticResourceRequested(this HttpContext context)
        {
            if (context?.Request == null)
                return false;

            return context.GetItem<bool>(
                "IsStaticResourceRequested",
                () => EngineContext.Current.Resolve<IWebHelper>().IsStaticResource(),
                true);
        }

        public static T GetItem<T>(this HttpContext httpContext, string key, Func<T> factory = null, bool forceCreation = true)
        {
            Guard.NotEmpty(key, nameof(key));

            var items = httpContext?.Items;
            if (items == null)
            {
                return default(T);
            }

            if (items.ContainsKey(key))
            {
                return (T)items[key];
            }
            else
            {
                if (forceCreation)
                {
                    var item = items[key] = (factory ?? (() => Activator.CreateInstance<T>())).Invoke();
                    return (T)item;
                }
                else
                {
                    return default(T);
                }
            }
        }

        /// <summary>Determines whether the specified HTTP request is an AJAX request.</summary>
        /// <returns>true if the specified HTTP request is an AJAX request; otherwise, false.</returns>
        /// <param name="request">The HTTP request.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> parameter is null (Nothing in Visual Basic).</exception>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            return request.Headers != null && request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        public static string GetValue(this HttpRequest request, string key)
        {
            string value = string.Empty;

            if (request.HasFormContentType && request.Form != null && request.Form.Any())
                value = request.Form[key];
            if (value.IsEmpty())
                value = request.Query[key];
            if (value.IsEmpty())
                value = request.Cookies[key];
            if (value.IsEmpty())
                value = request.HttpContext.GetServerVariable(key);

            return value ?? string.Empty;
        }
    }
}

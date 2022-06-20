﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using VTQT.Web.Framework.Routing;

namespace VTQT.Web.Master.Infrastructure
{
    /// <summary>
    /// Represents provider that provided generic routes
    /// </summary>
    public partial class GenericUrlRouteProvider : BaseRouteProvider, IRouteProvider
    {
        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var lang = GetLanguageRoutePattern();

            //default routes
            //these routes are not generic, they are just default to map requests that don't match other patterns, 
            //but we define them here since this route provider is with the lowest priority, to allow to add additional routes before them
            if (!string.IsNullOrEmpty(lang))
            {
                endpointRouteBuilder.MapControllerRoute(name: "DefaultWithLanguageCode",
                    pattern: $"{lang}/{{controller=Home}}/{{action=Index}}/{{id?}}");
            }

            endpointRouteBuilder.MapControllerRoute(name: "Default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            //generic routes
            var genericPattern = $"{lang}/{{SeName}}";

            endpointRouteBuilder.MapDynamicControllerRoute<SlugRouteTransformer>(genericPattern);

            endpointRouteBuilder.MapControllerRoute(name: "GenericUrl",
                pattern: "{genericSeName}",
                defaults: new { controller = "Common", action = "GenericUrl" });

            endpointRouteBuilder.MapControllerRoute(name: "GenericUrlWithParameter",
                pattern: "{genericSeName}/{genericParameter}",
                defaults: new { controller = "Common", action = "GenericUrl" });

            //endpointRouteBuilder.MapControllerRoute(name: "Product",
            //    pattern: genericPattern,
            //    defaults: new { controller = "Product", action = "ProductDetails" });

            //endpointRouteBuilder.MapControllerRoute(name: "Category",
            //    pattern: genericPattern,
            //    defaults: new { controller = "Catalog", action = "Category" });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        /// <remarks>
        /// it should be the last route. we do not set it to -int.MaxValue so it could be overridden (if required)
        /// </remarks>
        public int Priority => -1000000;

        #endregion
    }
}
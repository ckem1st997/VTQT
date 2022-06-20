using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using Elastic.Apm.NetCoreAll;
using VTQT.Core.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using VTQT.Core;
using VTQT.Core.Domain;
using VTQT.Core.Logging;
using VTQT.Services.Localization;
using VTQT.Services.Logging;
using VTQT.Web.Framework.Globalization;
using VTQT.Web.Framework.Routing;

namespace VTQT.Web.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of IApplicationBuilder
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Configure the application HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void ConfigureRequestPipeline(this IApplicationBuilder application, IConfiguration configuration)
        {
            EngineContext.Current.ConfigureRequestPipeline(application, configuration);
        }

        public static void StartEngine(this IApplicationBuilder application)
        {
            var engine = EngineContext.Current;
        }

        /// <summary>
        /// Add exception handling
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseXBaseExceptionHandler(this IApplicationBuilder application)
        {
            var xbaseConfig = CommonHelper.XBaseConfig;
            var webHostEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();
            var useDetailedExceptionPage = xbaseConfig.DisplayFullErrorStack || webHostEnvironment.IsDevelopment();
            if (useDetailedExceptionPage)
            {
                //get detailed exceptions for developing and testing purposes
                application.UseDeveloperExceptionPage();
            }
            else
            {
                //or use special exception handler
                if (xbaseConfig.AppProjectType == AppProjectType.API)
                {
                    application.UseExceptionHandler("/ApiError/Error");
                }
                else if (xbaseConfig.AppProjectType == AppProjectType.MVC)
                {
                    application.UseExceptionHandler("/MvcError/Error");
                    application.UseHsts();
                }
            }

            //log errors
            application.UseExceptionHandler(handler =>
            {
                handler.Run(async context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    if (exception == null)
                        return;

                    try
                    {
                        //log error
                        EngineContext.Current.Resolve<ILogger>().Error(exception);
                    }
                    finally
                    {
                        //rethrow the exception to show the error page
                        ExceptionDispatchInfo.Throw(exception);
                    }
                });
            });
        }

        /// <summary>
        /// Configure middleware for dynamically compressing HTTP responses
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseXBaseResponseCompression(this IApplicationBuilder application, IConfiguration configuration)
        {
            //whether to use compression (gzip by default)
            // TODO-XBase-Settings
            //if (EngineContext.Current.Resolve<CommonSettings>().UseResponseCompression)
            //    application.UseResponseCompression();
        }

        /// <summary>
        /// Configure the request localization feature
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseXBaseRequestLocalization(this IApplicationBuilder application)
        {
            application.UseRequestLocalization(async options =>
            {
                //prepare supported cultures
                var cultures = (await EngineContext.Current.Resolve<ILanguageService>().GetAllLanguagesAsync())
                    .OrderBy(language => language.DisplayOrder)
                    .Select(language => new CultureInfo(language.LanguageCulture)).ToList();

                options.DefaultRequestCulture = new RequestCulture(cultures.FirstOrDefault());
                // Formatting numbers, dates, etc.
                options.SupportedCultures = cultures;
                // UI strings that we have localized.
                options.SupportedUICultures = cultures;

                options.AddInitialRequestCultureProvider(new XBaseRequestCultureProvider(options));
            });
        }

        public static void UseXBaseStaticFiles(this IApplicationBuilder application)
        {
            //application.UseStaticFiles();
            var options = new StaticFileOptions();
            var typeProvider = new FileExtensionContentTypeProvider();
            if (!typeProvider.Mappings.ContainsKey(".ttf"))
            {
                typeProvider.Mappings.Add(".ttf", "application/octet-stream");
            }
            if (!typeProvider.Mappings.ContainsKey(".woff"))
            {
                typeProvider.Mappings.Add(".woff", "font/x-woff");
            }
            if (!typeProvider.Mappings.ContainsKey(".woff2"))
            {
                typeProvider.Mappings.Add(".woff2", "application/font-woff2");
            }
            options.ContentTypeProvider = typeProvider;
            application.UseStaticFiles(options);
        }

        public static void UseXBaseSwagger(this IApplicationBuilder application, string url, string title)
        {
            application.UseSwagger(c =>
                {
                    c.SerializeAsV2 = true;
                })
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint(url, title);
                    c.DocumentTitle = title;
                });
        }

        /// <summary>
        /// Configure API routing
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseXBaseApi(this IApplicationBuilder application)
        {
            var xbaseConfig = CommonHelper.XBaseConfig;

            if (CommonHelper.ElasticConfig.UseElasticApm)
                application.UseAllElasticApm(CommonHelper.Configuration);

            application.UseStaticFiles();

            application.UseRouting();

            application.UseCors("AllowAll");

            if (xbaseConfig.UseAuthentication)
            {
                application.UseAuthentication();
                application.UseAuthorization();
            }

            //Execute the endpoint selected by the routing middleware
            application.UseEndpoints(endpoints =>
            {
                // Custom
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute(); // == endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

                //register all routes
                EngineContext.Current.Resolve<IRoutePublisher>().RegisterRoutes(endpoints);
            });

            application.UseHealthChecks("/health");
        }

        public static void UseXBaseMvc(this IApplicationBuilder application)
        {
            var xbaseConfig = CommonHelper.XBaseConfig;

            if (CommonHelper.ElasticConfig.UseElasticApm)
                application.UseAllElasticApm(CommonHelper.Configuration);

            application.UseXBaseStaticFiles();

            application.UseRouting();

            application.UseCors("AllowAll");

            if (xbaseConfig.UseAuthentication)
            {
                application.UseAuthentication();
                application.UseAuthorization();
            }

            application.UseSession();

            application.UseXBaseRequestLocalization();

            //Execute the endpoint selected by the routing middleware
            application.UseEndpoints(endpoints =>
            {
                // Custom
                endpoints.MapControllers();
                // Comment để routing đúng trong Area, vì map route này sẽ ghi đè areaRoute
                // Nếu không khi Url.Action("Index", "Home", new { area = "Admin" }) => /?area=Admin => sai, mà đúng phải là: /Admin
                //endpoints.MapDefaultControllerRoute(); // == endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();

                //register all routes
                EngineContext.Current.Resolve<IRoutePublisher>().RegisterRoutes(endpoints);
            });

            application.UseHealthChecks("/health");
        }
    }
}

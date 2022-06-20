using VTQT.Core.Infrastructure;
using VTQT.Web.Framework.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VTQT.Core;
using VTQT.Core.Domain;

namespace VTQT.Web.Framework.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring API/MVC on application startup
    /// </summary>
    public class XBaseWebStartup : IXBaseStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var xbaseConfig = CommonHelper.XBaseConfig;

            //add data protection
            services.AddXBaseDataProtection();

            //add and configure API/MVC feature
            if (xbaseConfig.AppProjectType == AppProjectType.API)
            {
                services.AddXBaseApi();
            }
            else if (xbaseConfig.AppProjectType == AppProjectType.MVC)
            {
                services.AddXBaseMvc();
            }
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application, IConfiguration configuration)
        {
            var xbaseConfig = CommonHelper.XBaseConfig;

            if (xbaseConfig.AppProjectType == AppProjectType.API)
            {
                application.UseXBaseApi();
            }
            else if (xbaseConfig.AppProjectType == AppProjectType.MVC)
            {
                application.UseXBaseMvc();
            }
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 1000; //API/MVC should be loaded last
    }
}

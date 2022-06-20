using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.StaticFiles;

namespace XBase.Cdn
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(x => x.AddPolicy("AllowAll", p => p
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("AllowAll");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseStaticFiles();
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
            app.UseStaticFiles(options);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

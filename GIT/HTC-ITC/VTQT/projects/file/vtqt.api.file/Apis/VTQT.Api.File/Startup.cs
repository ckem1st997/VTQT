using System.IdentityModel.Tokens.Jwt;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using VTQT.Core;
using VTQT.Core.Configuration;
using VTQT.Core.Infrastructure;
using VTQT.Core.Infrastructure.Mapper;
using VTQT.Data;
using VTQT.Web.Framework.Infrastructure;
using VTQT.Web.Framework.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VTQT.Elastic;
using VTQT.Web.Framework.Infrastructure.AutoMapperProfiles;
using VTQT.SharedMvc.Master.Infrastructure.AutoMapperProfiles;
using VTQT.Web.Framework.Helpers;

namespace VTQT.Api.File
{
    public class Startup
    {
        private IEngine _engine;
        private XBaseConfig _config;

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
#if DEBUG
            var xbaseRootPath = Path.GetFullPath(Path.Combine(webHostEnvironment.ContentRootPath, @"..\.."));
#else
            var xbaseRootPath = Path.GetFullPath(Path.Combine(webHostEnvironment.ContentRootPath, @".."));
#endif
            Configuration = CommonHelper.AddWebConfiguration(webHostEnvironment, xbaseRootPath);

            WebHostEnvironment = webHostEnvironment;

            CommonHelper.Configuration = Configuration;
            CommonHelper.WebHostEnvironment = webHostEnvironment;

            CommonHelper.InitAppSettings(Configuration);

            AppHelper.Init();
        }

        public IConfiguration Configuration { get; private set; }

        public IWebHostEnvironment WebHostEnvironment { get; private set; }

        public ILifetimeScope AutofacContainer { get; private set; }

        // ConfigureServices is where you register dependencies. This gets
        // called by the runtime before the ConfigureContainer method, below.
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            AutoMapperConfiguration.Profiles.AddRange(new Profile[]
            {
                new CommonProfile(),
                new MasterProfile()
            });

            (_engine, _config) = services.ConfigureApplicationServices(Configuration, WebHostEnvironment);

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            // DataConnections
            services.AddXBaseDbContext(Configuration, new[]
            {
                DataConnectionHelper.ConnectionStringNames.Master,
                DataConnectionHelper.ConnectionStringNames.File
            });

            // Cache
            services.AddXBaseCaching(Configuration);

            services.AddXBaseSwagger("v1", "VTQT File API", "VTQT.Api.File.xml");
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            _engine.RegisterDependencies(builder, _config);

            // Register your own things directly with Autofac, like:
            builder.RegisterModule(new CoreModule(EngineContext.Current.TypeFinder));
            builder.RegisterModule(new LocalizationModule());
            builder.RegisterModule(new EventModule(EngineContext.Current.TypeFinder));
            builder.RegisterModule(new WebModule(EngineContext.Current.TypeFinder));
            builder.RegisterModule(new UiModule(EngineContext.Current.TypeFinder));
            //builder.RegisterModule(new ElasticModule(new[]
            //{
            //    ElasticHelper.ConnectionNames.Default
            //}));
            builder.RegisterModule(new ElasticModule());
            //builder.RegisterModule(new MongoDbModule());
            builder.RegisterModule(new LoggingModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();

            // TODO-XBase-Log
            //CommonHelper.AddLogger(configuration);

            // If, for some reason, you need a reference to the built container, you
            // can use the convenience extension method GetAutofacRoot.
            AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            app.ConfigureRequestPipeline(Configuration);

            app.StartEngine();

            app.UseXBaseSwagger("/swagger/v1/swagger.json", "VTQT File API");
        }
    }
}

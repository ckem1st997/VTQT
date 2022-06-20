using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using IdentityModel;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Core.Configuration;
using VTQT.Core.Domain.Master;
using VTQT.Core.Infrastructure;
using VTQT.Core.Security;
using VTQT.Data;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Domain.FbmContract;
using VTQT.Core.Domain.FbmCrm;
using VTQT.Core.Domain.FbmOrganization;
using VTQT.Core.Domain.File;
using VTQT.Core.Domain.Notify;
using VTQT.Core.Domain.Qlsc;
using VTQT.Core.Domain.Security;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Domain.Warehouse;
using VTQT.Services.Security;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Routing;
using VTQT.Web.Framework.Validators;

namespace VTQT.Web.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add services to the application and configure service provider
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        /// <param name="webHostEnvironment">Hosting environment</param>
        /// <returns>Configured service provider</returns>
        public static (IEngine, XBaseConfig) ConfigureApplicationServices(this IServiceCollection services,
            IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            ////let the operating system decide what TLS protocol version to use
            ////see https://docs.microsoft.com/dotnet/framework/network-programming/tls
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
            // SSL & TLS
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, errors) => true;

            IdentityModelEventSource.ShowPII = true;

            //add NopConfig configuration parameters
            services.Configure<XBaseConfig>(configuration.GetSection(XBaseConfig.XBase));

            //add hosting configuration parameters
            services.Configure<HostingConfig>(configuration.GetSection(HostingConfig.Hosting));

            services.Configure<SsoConfig>(configuration.GetSection(SsoConfig.Sso));

            //add accessor to HttpContext
            services.AddHttpContextAccessor();

            services.AddActionContextAccessor();

            services.AddUrlHelper();

            //create default file provider
            CommonHelper.DefaultFileProvider = new XBaseFileProvider(webHostEnvironment);

            //initialize plugins
            var mvcCoreBuilder = services.AddMvcCore();

            //create engine and configure service provider
            var engine = EngineContext.Create();

            engine.ConfigureServices(services, configuration, CommonHelper.XBaseConfig);

            return (engine, CommonHelper.XBaseConfig);
        }

        /// <summary>
        /// Create, bind and register as service the specified configuration parameters 
        /// </summary>
        /// <typeparam name="TConfig">Configuration parameters</typeparam>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Set of key/value application configuration properties</param>
        /// <returns>Instance of configuration parameters</returns>
        public static TConfig AddConfig<TConfig>(this IServiceCollection services, IConfiguration configuration)
            where TConfig : class, new()
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            //create instance of config
            var config = new TConfig();

            //bind it to the appropriate section of configuration
            configuration.Bind(config);

            //and register it as a service
            services.AddSingleton(config);

            return config;
        }

        /// <summary>
        /// Register HttpContextAccessor
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static void AddActionContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        }

        public static void AddUrlHelper(this IServiceCollection services)
        {
            services.AddScoped<IUrlHelper>(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });
        }

        /// <summary>
        /// Adds services required for anti-forgery support
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddXBaseAntiForgery(this IServiceCollection services)
        {
            services.AddAntiforgery();
        }

        /// <summary>
        /// Adds services required for application session state
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddXBaseSession(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Unspecified;
                //options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            });
        }

        public static void ConfigureXBaseCookie(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Unspecified;
            });
        }

        /// <summary>
        /// Adds data protection services
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddXBaseDataProtection(this IServiceCollection services)
        {
            var xbaseConfig = CommonHelper.XBaseConfig;

            var redis = ConnectionMultiplexer.Connect(xbaseConfig.RedisDataProtectionConnection);
            //check whether to persist data protection in Redis
            services.AddDataProtection()
                .SetApplicationName(XBaseConfig.XBase)
                .PersistKeysToStackExchangeRedis(redis, CachingDefaults.DataProtectionKeysName);
        }

        public static void AddXBaseApiAuthentication(this IServiceCollection services)
        {
            var ssoConfig = CommonHelper.SsoConfig;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = ssoConfig.Authority;
                    options.RequireHttpsMetadata = ssoConfig.RequireHttpsMetadata;

                    options.Audience = ssoConfig.ClientId;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,

                    };
                });
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //}).AddJwtBearer(options =>
            //{
            //    options.Authority = ssoConfig.Authority;
            //    options.Audience = ssoConfig.ClientId;
            //});
        }

        /// <summary>
        /// Adds authentication service
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddXBaseMvcAuthentication(this IServiceCollection services)
        {
            var ssoConfig = CommonHelper.SsoConfig;

            // Chú ý size Cookie lớn sẽ bị lỗi ở proxy => SSO redirect về App lỗi.
            // Cách fix hiệu quả nhất là tăng size buffer ở proxy
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultAuthenticateScheme = "Cookies";
                options.DefaultSignInScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            }).AddCookie("Cookies", options =>
            {
                options.AccessDeniedPath = ssoConfig.AccessDeniedPath;
                options.Cookie.SameSite = SameSiteMode.Unspecified;
                options.CookieManager = new ChunkingCookieManager();
            }).AddOpenIdConnect("oidc", options =>
            {
                options.Authority = ssoConfig.Authority;
                options.RequireHttpsMetadata = ssoConfig.RequireHttpsMetadata;

                options.ClientId = ssoConfig.ClientId;
                options.ClientSecret = ssoConfig.ClientSecret;

                //options.ResponseType = "token id_token";
                //options.ResponseType = OpenIdConnectResponseType.Code;
                options.ResponseType = ssoConfig.ResponseType;
                //options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                //options.CallbackPath = config.CallbackPath;
                //options.RemoteSignOutPath = config.RemoteSignOutPath;
                //options.SignedOutCallbackPath = config.SignedOutCallbackPath;
                options.SignedOutRedirectUri = ssoConfig.SignedOutRedirectUri;
                options.AccessDeniedPath = ssoConfig.AccessDeniedPath;

                //options.RemoteAuthenticationTimeout = TimeSpan.FromSeconds(10);
                options.NonceCookie.SameSite = SameSiteMode.Unspecified;
                //options.NonceCookie.SecurePolicy = CookieSecurePolicy.None;
                options.CorrelationCookie.SameSite = SameSiteMode.Unspecified;
                //options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.None;

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                //options.Scope.Add("api");
                //options.Scope.Add("web");
                //options.Scope.Add("admin");
                options.Scope.Add("offline_access");
                //options.Scope.Add("all_claims");

                if (ssoConfig.Scopes != null && ssoConfig.Scopes.Any())
                {
                    ssoConfig.Scopes.Each(scope =>
                    {
                        if (!options.Scope.Contains(scope))
                            options.Scope.Add(scope);
                    });
                }

                options.ClaimActions.MapAllExcept("iss", "nbf", "exp", "aud", "nonce", "iat", "c_hash");
                //options.ClaimActions.MapAll();

                options.GetClaimsFromUserInfoEndpoint = ssoConfig.GetClaimsFromUserInfoEndpoint; // true => OnUserInformationReceived
                //options.SaveTokens = ssoConfig.SaveTokens; // Không lưu token vào AuthenticationProperties để tránh size Cookie lớn, lưu tập trung ở Claim/Cache

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.Name,
                    RoleClaimType = JwtClaimTypes.Role,
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnUserInformationReceived = context =>
                    {
                        var rawAccessToken = context.ProtocolMessage.AccessToken;
                        var handler = new JwtSecurityTokenHandler();
                        LogHelper.LogInformation("Begin OnUserInformationReceived ...");
                        LogHelper.LogInformation(rawAccessToken);
                        var accessToken = handler.ReadJwtToken(rawAccessToken);
                        //var idToken = handler.ReadJwtToken(rawIdToken);

                        var additionalClaims = new List<Claim>();
                        additionalClaims.Add(new Claim(OidcConstants.TokenTypes.AccessToken, rawAccessToken));
                        additionalClaims.Add(new Claim(OidcConstants.TokenTypes.IdentityToken, context.ProtocolMessage.IdToken));
                        additionalClaims.Add(new Claim(OidcConstants.TokenTypes.RefreshToken, context.ProtocolMessage.RefreshToken));
                        //additionalClaims.Add(new Claim(JwtClaimTypes.IssuedAt, accessToken.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.IssuedAt)?.Value ?? ""));
                        additionalClaims.Add(new Claim(nameof(JwtClaimTypes.IssuedAt), accessToken.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.IssuedAt)?.Value ?? ""));
                        additionalClaims.Add(new Claim(OidcConstants.AuthorizeResponse.ExpiresIn, context.ProtocolMessage.ExpiresIn));

                        //additionalClaims.Add(new Claim("username", accessToken.Claims.FirstOrDefault(x => x.Type == "username")?.Value ?? ""));
                        //additionalClaims.Add(new Claim("fullname", accessToken.Claims.FirstOrDefault(x => x.Type == "fullname")?.Value ?? ""));
                        //additionalClaims.Add(new Claim(JwtClaimTypes.Email, accessToken.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ?? ""));
                        //additionalClaims.Add(new Claim(JwtClaimTypes.PhoneNumber, accessToken.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.PhoneNumber)?.Value ?? ""));
                        //additionalClaims.Add(new Claim("url_photo", accessToken.Claims.FirstOrDefault(x => x.Type == "url_photo")?.Value ?? ""));

                        //additionalClaims.Add(new Claim(JwtClaimTypes.PreferredUserName, accessToken.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.PreferredUserName)?.Value ?? ""));
                        //additionalClaims.Add(new Claim(JwtClaimTypes.Name, accessToken.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value ?? ""));
                        //additionalClaims.Add(new Claim(JwtClaimTypes.Email, accessToken.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ?? ""));
                        ////additionalClaims.Add(new Claim("mobile", accessToken.Claims.FirstOrDefault(x => x.Type == "mobile")?.Value ?? ""));

                        //var newIdentity = new ClaimsIdentity(context.Principal.Identity, additionalClaims, "pwd", "name", "role");
                        //context.Principal = new ClaimsPrincipal(newIdentity);

                        var id = context.Principal.Identity as ClaimsIdentity;
                        id?.AddClaims(additionalClaims);

                        //var userClaims = context.HttpContext.User.Identity as ClaimsIdentity;
                        //userClaims?.AddClaims(additionalClaims);

                        return Task.CompletedTask;
                    },
                    // handle correlation failed errors caused by stale lock login pages.
                    OnRemoteFailure = (context) =>
                    {
                        //if (context.Failure.Message == "Correlation failed.")
                        //{
                        //    // [ log error ]
                        //    context.HandleResponse();
                        //    context.Response.Redirect("/Account/CorrelationFailed");
                        //}
                        LogHelper.LogInformation("Begin OnRemoteFailure ...");
                        return Task.CompletedTask;
                    },
                    OnRedirectToIdentityProvider = n =>
                    {
                        if (!string.IsNullOrWhiteSpace(ssoConfig.RedirectUri))
                        {
                            n.ProtocolMessage.RedirectUri = ssoConfig.RedirectUri;
                        }

                        // If signing out, add the id_token_hint
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                        {
                            var idTokenClaim = n.HttpContext.User.FindFirst("id_token");

                            if (idTokenClaim != null)
                            {
                                n.ProtocolMessage.IdTokenHint = idTokenClaim.Value;
                            }
                        }

                        return Task.CompletedTask;
                    },
                    OnSignedOutCallbackRedirect = context =>
                    {
                        context.Response.Redirect(context.Options.SignedOutRedirectUri);
                        context.HandleResponse();

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        LogHelper.LogInformation("Begin OnAuthenticationFailed ...");
                        LogHelper.LogWarning(context.ProtocolMessage.AccessToken);
                        return Task.CompletedTask;
                    },
                    OnTokenResponseReceived = context =>
                    {
                        LogHelper.LogInformation("Begin OnTokenResponseReceived ...");
                        LogHelper.LogWarning(context.ProtocolMessage.AccessToken);
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        LogHelper.LogInformation("Begin OnMessageReceived ...");
                        LogHelper.LogWarning(context.ProtocolMessage.AccessToken);
                        return Task.CompletedTask;
                    },
                    OnAuthorizationCodeReceived = context =>
                    {
                        LogHelper.LogInformation("Begin OnAuthorizationCodeReceived ...");
                        LogHelper.LogWarning(context.ProtocolMessage.AccessToken);
                        return Task.CompletedTask;
                    },
                    OnTicketReceived = context =>
                    {
                        LogHelper.LogInformation("Begin OnTicketReceived ...");
                        LogHelper.LogWarning(context.ReturnUri);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        LogHelper.LogInformation("Begin OnTicketReceived ...");
                        LogHelper.LogWarning(context.ProtocolMessage.AccessToken);
                        return Task.CompletedTask;
                    }
                };
            });
        }

        /// <summary>
        /// Register custom RedirectResultExecutor
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddXBaseRedirectResultExecutor(this IServiceCollection services)
        {
            //we use custom redirect executor as a workaround to allow using non-ASCII characters in redirect URLs
            services.AddScoped<IActionResultExecutor<RedirectResult>, XBaseRedirectResultExecutor>();
        }

        public static void AddXBaseDbContext(this IServiceCollection services, IConfiguration configuration, IEnumerable<string> connectionStringNames)
        {
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.Master))
            {
                services.AddLinqToDbContext<MasterDataConnection>((provider, options) =>
                {
                    options
                        .UseMySql(configuration.GetConnectionString(DataConnectionHelper.ConnectionStringNames.Master))
                        .UseDefaultLogging(provider);
                });
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.Warehouse))
            {
                services.AddLinqToDbContext<WarehouseDataConnection>((provider, options) =>
                {
                    options
                        .UseMySql(configuration.GetConnectionString(DataConnectionHelper.ConnectionStringNames.Warehouse))
                        .UseDefaultLogging(provider);
                });
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.Asset))
            {
                services.AddLinqToDbContext<AssetDataConnection>((provider, options) =>
                {
                    options
                        .UseMySql(configuration.GetConnectionString(DataConnectionHelper.ConnectionStringNames.Asset))
                        .UseDefaultLogging(provider);
                });
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.Ticket))
            {
                services.AddLinqToDbContext<TicketDataConnection>((provider, options) =>
                {
                    options
                        .UseMySql(configuration.GetConnectionString(DataConnectionHelper.ConnectionStringNames.Ticket))
                        .UseDefaultLogging(provider);
                });
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.File))
            {
                services.AddLinqToDbContext<FileDataConnection>((provider, options) =>
                {
                    options
                        .UseMySql(configuration.GetConnectionString(DataConnectionHelper.ConnectionStringNames.File))
                        .UseDefaultLogging(provider);
                });
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.Notify))
            {
                services.AddLinqToDbContext<NotifyDataConnection>((provider, options) =>
                {
                    options
                        .UseMySql(configuration.GetConnectionString(DataConnectionHelper.ConnectionStringNames.Notify))
                        .UseDefaultLogging(provider);
                });
            }

            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.Qlsc))
            {
                services.AddLinqToDbContext<QlscDataConnection>((provider, options) =>
                {
                    options
                        .UseMySql(configuration.GetConnectionString(DataConnectionHelper.ConnectionStringNames.Qlsc))
                        .UseDefaultLogging(provider);
                });
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.FbmOrganization))
            {
                services.AddLinqToDbContext<FbmOrganizationDataConnection>((provider, options) =>
                {
                    options
                        .UseMySql(configuration.GetConnectionString(DataConnectionHelper.ConnectionStringNames.FbmOrganization))
                        .UseDefaultLogging(provider);
                });
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.FbmContract))
            {
                services.AddLinqToDbContext<FbmContractDataConnection>((provider, options) =>
                {
                    options
                        .UseMySql(configuration.GetConnectionString(DataConnectionHelper.ConnectionStringNames.FbmContract))
                        .UseDefaultLogging(provider);
                });
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.FbmCrm))
            {
                services.AddLinqToDbContext<FbmCrmDataConnection>((provider, options) =>
                {
                    options
                        .UseMySql(configuration.GetConnectionString(DataConnectionHelper.ConnectionStringNames.FbmCrm))
                        .UseDefaultLogging(provider);
                });
            }
        }

        public static void AddXBaseCaching(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEasyCaching(options =>
            {
                // local
                options.UseInMemory(configuration,
                    CachingHelper.Configs.ProviderNames.InMemory,
                    CachingHelper.Configs.ConfigSectionNames.InMemory);
                // distributed
                options.UseRedis(configuration,
                    CachingHelper.Configs.ProviderNames.Redis,
                    CachingHelper.Configs.ConfigSectionNames.Redis);

                // combine local and distributed
                options.UseHybrid(config =>
                {
                    config.TopicName = CachingHelper.Configs.HybridTopicName;
#if DEBUG
                    config.EnableLogging = true;
#endif
                    config.LocalCacheProviderName = CachingHelper.Configs.ProviderNames.InMemory;
                    config.DistributedCacheProviderName = CachingHelper.Configs.ProviderNames.Redis;
                }, CachingHelper.Configs.ProviderNames.Hybrid)
                // use redis bus
                .WithRedisBus(configuration, CachingHelper.Configs.ConfigSectionNames.RedisBus);

                Action<JsonSerializerSettings> serializerSettings = x =>
                {
                    x.MissingMemberHandling = MissingMemberHandling.Ignore;
                    x.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    x.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
                    x.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    x.MaxDepth = 32;
                };
                options.WithJson(serializerSettings, CachingHelper.SerializerNames.Json);
            });
        }

        public static void AddApplicationParts(this IServiceCollection services, IMvcBuilder mvcBuilder)
        {
            var appPartAssemblies = new List<Assembly>();
            if (CommonHelper.ApplicationParts != null && CommonHelper.ApplicationParts.Any())
            {
                foreach (var applicationPart in CommonHelper.ApplicationParts)
                {
                    var assembly = Assembly.Load(applicationPart);
                    mvcBuilder.AddApplicationPart(assembly);

                    appPartAssemblies.Add(assembly);
                }
            }

            mvcBuilder.AddRazorRuntimeCompilation();

            services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
            {
                foreach (var assembly in appPartAssemblies)
                {
                    options.FileProviders.Add(new EmbeddedFileProvider(assembly));
                }
            });

            //// The following code provides an alternative approach to configuring ApplicationPartManager using AssemblyPart:
            //if (CommonHelper.ApplicationParts != null && CommonHelper.ApplicationParts.Any())
            //{
            //    var appParts = new List<AssemblyPart>();
            //    foreach (var applicationPart in CommonHelper.ApplicationParts)
            //    {
            //        var assembly = Assembly.Load(applicationPart);
            //        // This creates an AssemblyPart, but does not create any related parts for items such as views.
            //        var part = new AssemblyPart(assembly);

            //        appParts.Add(part);
            //    }

            //    mvcBuilder
            //        .ConfigureApplicationPartManager(apm => apm.ApplicationParts.AddRange(appParts));
            //}
        }

        public static void AddXBaseFluentValidation(this IMvcBuilder mvcBuilder, XBaseConfig config)
        {
            //add fluent validation
            mvcBuilder.AddFluentValidation(configuration =>
            {
                //register all available validators from Nop assemblies
                var assemblies = mvcBuilder.PartManager.ApplicationParts
                    .OfType<AssemblyPart>()
                    .Where(part => part.Name.StartsWith("VTQT", StringComparison.InvariantCultureIgnoreCase))
                    .Select(part => part.Assembly);
                configuration.RegisterValidatorsFromAssemblies(assemblies);

                //implicit/automatic validation of child properties
                configuration.ImplicitlyValidateChildProperties = true;

                // It sais 'not recommended', but who cares: SAVE RAM!
                configuration.ValidatorOptions.DisableAccessorCache = true;

                // Setup custom resources
                //configuration.ValidatorOptions.LanguageManager = ValidatorLanguageManager();

                // Setup our custom DisplayName handling
                var originalDisplayNameResolver = ValidatorOptions.Global.DisplayNameResolver;
                configuration.ValidatorOptions.DisplayNameResolver = (type, member, expression) =>
                {
                    string name = null;

                    if (member != null)
                    {
                        var attr = member.GetAttribute<XBaseResourceDisplayName>(true);
                        if (attr != null)
                        {
                            name = attr.DisplayName;
                        }
                    }

                    return name ?? originalDisplayNameResolver.Invoke(type, member, expression);
                };
            });
        }

        public static void AddXBaseSwagger(this IServiceCollection services, string version, string title, string commentFilePath)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(version, new OpenApiInfo
                {
                    Title = title,
                    Version = version
                });

                var filePath = Path.Combine(AppContext.BaseDirectory, commentFilePath);
                c.IncludeXmlComments(filePath);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    In = ParameterLocation.Header
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "OAuth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                c.CustomSchemaIds(x => x.FullName);
            });
            services.AddSwaggerGenNewtonsoftSupport(); // explicit opt-in - needs to be placed after AddSwaggerGen()
        }

        /// <summary>
        /// Add and configure API for the application
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <returns>A builder for configuring API services</returns>
        public static IMvcBuilder AddXBaseApi(this IServiceCollection services)
        {
            var xbaseConfig = CommonHelper.XBaseConfig;

            services.AddCors(x => x.AddPolicy("AllowAll", p => p
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));

            services.AddHttpClient();

            if (xbaseConfig.UseAuthentication)
            {
                services.AddXBaseApiAuthentication();
            }

            //add basic MVC feature
            var mvcBuilder = services
                .AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    //options.SuppressConsumesConstraintForFormFileParameters = true;
                    //options.SuppressInferBindingSourcesForParameters = true;
                    options.SuppressModelStateInvalidFilter = true;
                    //options.SuppressMapClientErrors = true;
                    //options.ClientErrorMapping[StatusCodes.Status404NotFound].Link =
                    //    "https://httpstatuses.com/404";
                });

            //MVC now serializes JSON with camel case names by default, use this code to avoid it
            mvcBuilder.AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            ////set some options
            //mvcBuilder.AddMvcOptions(options =>
            //{
            //    //add custom display metadata provider
            //    options.ModelMetadataDetailsProviders.Add(new XBaseMetadataProvider());

            //    //in .NET model binding for a non-nullable property may fail with an error message "The value '' is invalid"
            //    //here we set the locale name as the message, we'll replace it with the actual one later when not-null validation failed
            //    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => XBaseValidationDefaults.NotNullValidationLocaleName);
            //});

            mvcBuilder.AddXBaseFluentValidation(xbaseConfig);

            //register controllers as services, it'll allow to override them
            mvcBuilder.AddControllersAsServices();

            services.AddHealthChecks();

            return mvcBuilder;
        }

        /// <summary>
        /// Add and configure MVC for the application
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <returns>A builder for configuring MVC services</returns>
        public static IMvcBuilder AddXBaseMvc(this IServiceCollection services)
        {
            var xbaseConfig = CommonHelper.XBaseConfig;

            services.AddCors(x => x.AddPolicy("AllowAll", p => p
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));

            services.ConfigureNonBreakingSameSiteCookies();

            services.ConfigureXBaseCookie();

            if (xbaseConfig.UseAuthentication)
            {
                services.AddXBaseMvcAuthentication();
            }

            //add anti-forgery
            //services.AddXBaseAntiForgery();

            //add basic MVC feature
            var mvcBuilder = services
                .AddControllersWithViews()
                .ConfigureApiBehaviorOptions(options =>
                {
                    //options.SuppressConsumesConstraintForFormFileParameters = true;
                    //options.SuppressInferBindingSourcesForParameters = true;
                    options.SuppressModelStateInvalidFilter = true;
                    //options.SuppressMapClientErrors = true;
                    //options.ClientErrorMapping[StatusCodes.Status404NotFound].Link =
                    //    "https://httpstatuses.com/404";
                });

            services.AddApplicationParts(mvcBuilder);

            //use session-based temp data provider
            mvcBuilder.AddSessionStateTempDataProvider();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = xbaseConfig.RedisSessionConnection;
                options.InstanceName = "Session_";
            });

            //add HTTP sesion state feature
            services.AddXBaseSession();

            services.AddRazorPages();

            //add custom redirect result executor
            services.AddXBaseRedirectResultExecutor();

            //MVC now serializes JSON with camel case names by default, use this code to avoid it
            mvcBuilder.AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            //set some options
            mvcBuilder.AddMvcOptions(options =>
            {
                options.ModelBinderProviders.Insert(0, new CustomBinderProvider());

                //add custom display metadata provider
                options.ModelMetadataDetailsProviders.Add(new XBaseMetadataProvider());

                //in .NET model binding for a non-nullable property may fail with an error message "The value '' is invalid"
                //here we set the locale name as the message, we'll replace it with the actual one later when not-null validation failed
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => XBaseValidationDefaults.NotNullValidationLocaleName);
            });

            mvcBuilder.AddXBaseFluentValidation(xbaseConfig);

            //register controllers as services, it'll allow to override them
            mvcBuilder.AddControllersAsServices();

            //add routing
            services.AddRouting(options =>
            {
                //add constraint key for language
                options.ConstraintMap[XBasePathRouteDefaults.LanguageParameterTransformer] = typeof(LanguageParameterTransformer);
            });

            services.AddHealthChecks();

            return mvcBuilder;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Core.Resolving.Pipeline;
using VTQT.Core;
using VTQT.Core.Configuration;
using VTQT.Core.Infrastructure;
using VTQT.Core.Infrastructure.DependencyManagement;
using VTQT.Services.Configuration;
using VTQT.Services.Events;
using VTQT.Services.Helpers;
using VTQT.Services.Localization;
using VTQT.ComponentModel;
using VTQT.Core.Events;
using VTQT.Core.Localization;
using VTQT.Core.Logging;
using VTQT.Services;
using VTQT.Services.Apps;
using VTQT.Services.Logging;
using VTQT.Services.Security;
using VTQT.Services.Seo;
using VTQT.Web.Framework.Localization;
using VTQT.Web.Framework.Routing;
using VTQT.Web.Framework.UI;
using VTQT.Web.Framework.Utilities;
using Module = Autofac.Module;

namespace VTQT.Web.Framework.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, XBaseConfig config)
        {

        }

        /// <summary>
        /// Gets order of this dependency registrar implementation
        /// </summary>
        public int Order => 0;
    }

    #region Modules

    public class CoreModule : Module
    {
        private readonly ITypeFinder _typeFinder;

        public CoreModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Common
            builder.RegisterType<MvcNotifier>().As<IMvcNotifier>().InstancePerLifetimeScope();
            builder.RegisterType<XBaseFileProvider>().As<IXBaseFileProvider>().InstancePerLifetimeScope();
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerLifetimeScope();
            // TODO-XBase-Log
            //builder.RegisterType<SystemLogger>().As<ISystemLogger>().InstancePerLifetimeScope();

            // Work context
            builder.RegisterType<WebWorkContext>().As<IWorkContext>().InstancePerLifetimeScope();

            //store context
            builder.RegisterType<WebAppContext>().As<IAppContext>().InstancePerLifetimeScope();

            // Services
            builder.RegisterType<DateTimeHelper>().As<IDateTimeHelper>().InstancePerLifetimeScope();
            builder.RegisterType<AppService>().As<IAppService>().InstancePerLifetimeScope();
            builder.RegisterType<AppActionService>().As<IAppActionService>().InstancePerLifetimeScope();
            builder.RegisterType<AppMappingService>().As<IAppMappingService>().InstancePerLifetimeScope();
            builder.RegisterType<KeycloakService>().As<IKeycloakService>().InstancePerLifetimeScope();
            builder.RegisterType<PermissionService>().As<IPermissionService>().InstancePerLifetimeScope();
            builder.RegisterType<RoleService>().As<IRoleService>().InstancePerLifetimeScope();

            builder.RegisterType<UrlRecordService>().As<IUrlRecordService>().InstancePerLifetimeScope();
            builder.RegisterType<SettingService>().As<ISettingService>().InstancePerLifetimeScope();
            builder.RegisterType<CommonServices>().As<ICommonServices>().InstancePerLifetimeScope();

            // Replaced by: services.AddActionContextAccessor()
            //builder.RegisterType<ActionContextAccessor>().As<IActionContextAccessor>().SingleInstance();

            builder.RegisterType<ViewRenderService>().As<IViewRenderService>().InstancePerLifetimeScope();

            // Settings
            builder.RegisterSource(new SettingsSource());

            #region DB Hooks

            #endregion
        }

        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration)
        {
            // Look for first settable property of type "ICommonServices" and inject
            var servicesProperty = FindCommonServicesProperty(registration.Activator.LimitType);

            if (servicesProperty == null)
                return;

            registration.Metadata.Add("Property.ICommonServices", FastProperty.Create(servicesProperty));

            registration.PipelineBuilding += (sender2, pipeline) =>
            {
                pipeline.Use(PipelinePhase.Activation, MiddlewareInsertionMode.EndOfPhase, (c, next) =>
                {
                    next(c);

                    // Do something with the component instance
                    var prop = c.Registration.Metadata.Get("Property.ICommonServices") as FastProperty;
                    var services = c.Resolve<ICommonServices>();
                    prop.SetValue(c.Instance, services);
                });
            };
        }

        private static PropertyInfo FindCommonServicesProperty(Type type)
        {
            var prop = type
                .GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new
                {
                    PropertyInfo = p,
                    p.PropertyType,
                    IndexParameters = p.GetIndexParameters(),
                    Accessors = p.GetAccessors(false)
                })
                .Where(x => x.PropertyType == typeof(ICommonServices)) // must be ICommonServices
                .Where(x => x.IndexParameters.Count() == 0) // must not be an indexer
                .Where(x => x.Accessors.Length != 1 || x.Accessors[0].ReturnType == typeof(void)) //must have get/set, or only set
                .Select(x => x.PropertyInfo)
                .FirstOrDefault();

            return prop;
        }

        // TODO-XBase-Log
        //private IEnumerable<Action<IComponentContext, object>> BuildLoggerInjectors(Type componentType)
        //{
        //    // Look for first settable property of type "ICommonServices" 
        //    var loggerProperties = componentType
        //        .GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance)
        //        .Select(p => new
        //        {
        //            PropertyInfo = p,
        //            p.PropertyType,
        //            IndexParameters = p.GetIndexParameters(),
        //            Accessors = p.GetAccessors(false)
        //        })
        //        .Where(x => x.PropertyType == typeof(ILogger)) // must be a logger
        //        .Where(x => x.IndexParameters.Count() == 0) // must not be an indexer
        //        .Where(x => x.Accessors.Length != 1 || x.Accessors[0].ReturnType == typeof(void)) //must have get/set, or only set
        //        .Select(x => FastProperty.Create(x.PropertyInfo));

        //    // Return an array of actions that resolve a logger and assign the property
        //    foreach (var prop in loggerProperties)
        //    {
        //        yield return (ctx, instance) =>
        //        {
        //            string component = componentType.ToString();
        //            var logger = ctx.Resolve<ILogger>();
        //            prop.SetValue(instance, logger);
        //        };
        //    }
        //}
    }

    public class LocalizationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LanguageService>().As<ILanguageService>().InstancePerLifetimeScope();
            builder.RegisterType<LocalizationService>().As<ILocalizationService>().InstancePerLifetimeScope();

            builder.RegisterType<Text>().As<IText>().InstancePerLifetimeScope();
            builder.Register<Localizer>(c => c.Resolve<IText>().Get).InstancePerLifetimeScope();

            builder.RegisterType<LocalizedEntityService>().As<ILocalizedEntityService>().InstancePerLifetimeScope();
        }

        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration)
        {
            var userProperty = FindUserProperty(registration.Activator.LimitType);

            if (userProperty == null)
                return;

            registration.Metadata.Add("Property.T", FastProperty.Create(userProperty));

            registration.PipelineBuilding += (sender2, pipeline) =>
            {
                pipeline.Use(PipelinePhase.Activation, MiddlewareInsertionMode.EndOfPhase, (c, next) =>
                {
                    next(c);

                    // Do something with the component instance
                    if (c.Registration.Metadata.Get("Property.T") is FastProperty prop)
                    {
                        //try
                        {
                            var iText = c.Resolve<IText>();
                            if (prop.Property.PropertyType == typeof(Localizer))
                            {
                                Localizer localizer = c.Resolve<IText>().Get;
                                prop.SetValue(c.Instance, localizer);
                            }
                        }
                        //catch { }
                    }
                });
            };
        }

        private static PropertyInfo FindUserProperty(Type type)
        {
            return type.GetProperty("T", typeof(Localizer));
        }
    }

    public class EventModule : Module
    {
        private readonly ITypeFinder _typeFinder;

        public EventModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EventPublisher>().As<IEventPublisher>().SingleInstance();

            // Event Consumers
            var consumers = _typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            foreach (var consumer in consumers)
            {
                builder.RegisterType(consumer)
                    .As(consumer.FindInterfaces((type, criteria) =>
                    {
                        var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                        return isMatch;
                    }, typeof(IConsumer<>)))
                    .InstancePerLifetimeScope();
            }
        }
    }

    public class WebModule : Module
    {
        private readonly ITypeFinder _typeFinder;

        public WebModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var foundAssemblies = _typeFinder.GetAssemblies().ToArray();

            builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();
        }
    }

    public class UiModule : Module
    {
        private readonly ITypeFinder _typeFinder;

        public UiModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // register UI component renderers
            builder.RegisterType<TabStripRenderer>().As<ComponentRenderer<TabStrip>>().InstancePerLifetimeScope();
        }
    }

    public class LoggingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Set default Logger
            var loggerType = CommonHelper.LoggingConfig.LoggerType;
            if (loggerType == LoggingHelper.LoggerTypes.SeriLogger)
            {
                builder.RegisterType<SeriLogger>().As<ILogger>().SingleInstance();
            }
            if (loggerType == LoggingHelper.LoggerTypes.ElasticLogger)
            {
                builder.RegisterType<ElasticLogger>().As<ILogger>().SingleInstance();
            }

            // Register Loggers
            builder.RegisterType<SeriLogger>()
                .Named<ILogger>(LoggingHelper.LoggerTypes.SeriLogger)
                .SingleInstance();

            builder.RegisterType<ElasticLogger>()
                .Named<ILogger>(LoggingHelper.LoggerTypes.ElasticLogger)
                .SingleInstance();

            // Register resolving delegate
            builder.Register<Func<string, ILogger>>(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                return type => cc.ResolveNamed<ILogger>(loggerType);
            });

            builder.Register<Func<string, Lazy<ILogger>>>(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                return type => cc.ResolveNamed<Lazy<ILogger>>(loggerType);
            });
        }

        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration)
        {
            var servicesProperty = FindLoggerProperty(registration.Activator.LimitType);

            if (servicesProperty == null)
                return;

            registration.Metadata.Add("Property.Logger", FastProperty.Create(servicesProperty));

            registration.PipelineBuilding += (sender2, pipeline) =>
            {
                pipeline.Use(PipelinePhase.Activation, MiddlewareInsertionMode.EndOfPhase, (c, next) =>
                {
                    next(c);

                    // Do something with the component instance
                    var prop = c.Registration.Metadata.Get("Property.Logger") as FastProperty;
                    var services = c.Resolve<ILogger>();
                    prop.SetValue(c.Instance, services);
                });
            };
        }

        private static PropertyInfo FindLoggerProperty(Type type)
        {
            return type.GetProperty("Logger", typeof(ILogger));
        }
    }

    #endregion

    #region Sources

    /// <summary>
    /// Setting source
    /// </summary>
    public class SettingsSource : IRegistrationSource
    {
        private static readonly MethodInfo BuildMethod =
            typeof(SettingsSource).GetMethod(
                "BuildRegistration",
                BindingFlags.Static | BindingFlags.NonPublic);

        /// <summary>
        /// Registrations for
        /// </summary>
        /// <param name="service">Service</param>
        /// <param name="registrations">Registrations</param>
        /// <returns>Registrations</returns>
        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service,
            Func<Service, IEnumerable<ServiceRegistration>> registrations)
        {
            if (service is TypedService ts && typeof(ISettings).IsAssignableFrom(ts.ServiceType))
            {
                var buildMethod = BuildMethod.MakeGenericMethod(ts.ServiceType);
                yield return (IComponentRegistration)buildMethod.Invoke(null, null);
            }
        }

        //[SuppressMessage("CodeQuality", "IDE0051", Justification = "Called by reflection")]
        private static IComponentRegistration BuildRegistration<TSettings>() where TSettings : ISettings, new()
        {
            return RegistrationBuilder
                .ForDelegate((c, p) =>
                {
                    var currentAppId = SettingDefaults.AllAppsId;
                    //try
                    {
                        if (c.TryResolve(out IAppContext appContext))
                        {
                            var appId = appContext.CurrentApp?.Id;
                            if (!string.IsNullOrWhiteSpace(appId))
                                currentAppId = appId;
                        }
                    }
                    //catch { }

                    //try
                    {
                        return c.Resolve<ISettingService>().LoadSetting<TSettings>(currentAppId);
                    }
                    //catch
                    //{
                    //    // Unit tests & tooling
                    //    return new TSettings();
                    //    //throw;
                    //}
                })
                .InstancePerLifetimeScope()
                .CreateRegistration();
        }

        /// <summary>
        /// Is adapter for individual components
        /// </summary>
        public bool IsAdapterForIndividualComponents => false;
    }

    #endregion
}

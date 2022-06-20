using Autofac;
using System.Net.Http;
using VTQT.Core.Configuration;
using VTQT.Core.Infrastructure;
using VTQT.Core.Infrastructure.DependencyManagement;
using VTQT.SharedMvc.Helpers;
using VTQT.SharedMvc.Infrastructure.Caching;

namespace VTQT.SharedMvc.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, XBaseConfig config)
        {
            builder.RegisterType<SendDiscordHelper>().As<ISendDiscordHelper>().InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<IHttpClientFactory>().CreateClient()).As<HttpClient>();
        }

        public int Order => 1;
    }
}

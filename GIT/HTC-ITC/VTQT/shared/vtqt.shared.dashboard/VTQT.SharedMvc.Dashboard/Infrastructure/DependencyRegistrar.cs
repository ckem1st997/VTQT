using Autofac;
using VTQT.Core.Configuration;
using VTQT.Core.Infrastructure;
using VTQT.Core.Infrastructure.DependencyManagement;

namespace VTQT.SharedMvc.Dashboard.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, XBaseConfig config)
        {
            
        }

        public int Order => 1;
    }
}

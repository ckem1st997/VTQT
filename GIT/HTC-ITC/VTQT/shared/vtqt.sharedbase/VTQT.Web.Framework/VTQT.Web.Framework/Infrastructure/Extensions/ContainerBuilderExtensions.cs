using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using EasyCaching.Core;
using VTQT.Caching;
using VTQT.Core.Domain.Master;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using LinqToDB.Data;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Domain.FbmContract;
using VTQT.Core.Domain.FbmCrm;
using VTQT.Core.Domain.FbmOrganization;
using VTQT.Core.Domain.File;
using VTQT.Core.Domain.Notify;
using VTQT.Core.Domain.Qlsc;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Web.Framework.Infrastructure.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterDataConnection(this ContainerBuilder builder, IEnumerable<string> connectionStringNames)
        {
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.Master))
            {
                builder.RegisterType<MasterDataConnection>()
                    .Named<DataConnection>(DataConnectionHelper.ConnectionStringNames.Master)
                    .InstancePerDependency();
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.Warehouse))
            {
                builder.RegisterType<WarehouseDataConnection>()
                    .Named<DataConnection>(DataConnectionHelper.ConnectionStringNames.Warehouse)
                    .InstancePerDependency();
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.Asset))
            {
                builder.RegisterType<AssetDataConnection>()
                    .Named<DataConnection>(DataConnectionHelper.ConnectionStringNames.Asset)
                    .InstancePerDependency();
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.Ticket))
            {
                builder.RegisterType<TicketDataConnection>()
                    .Named<DataConnection>(DataConnectionHelper.ConnectionStringNames.Ticket)
                    .InstancePerDependency();
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.File))
            {
                builder.RegisterType<FileDataConnection>()
                    .Named<DataConnection>(DataConnectionHelper.ConnectionStringNames.File)
                    .InstancePerDependency();
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.Notify))
            {
                builder.RegisterType<NotifyDataConnection>()
                    .Named<DataConnection>(DataConnectionHelper.ConnectionStringNames.Notify)
                    .InstancePerDependency();
            }

            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.Qlsc))
            {
                builder.RegisterType<QlscDataConnection>()
                    .Named<DataConnection>(DataConnectionHelper.ConnectionStringNames.Qlsc)
                    .InstancePerDependency();
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.FbmOrganization))
            {
                builder.RegisterType<FbmOrganizationDataConnection>()
                    .Named<DataConnection>(DataConnectionHelper.ConnectionStringNames.FbmOrganization)
                    .InstancePerDependency();
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.FbmContract))
            {
                builder.RegisterType<FbmContractDataConnection>()
                    .Named<DataConnection>(DataConnectionHelper.ConnectionStringNames.FbmContract)
                    .InstancePerDependency();
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.FbmCrm))
            {
                builder.RegisterType<FbmCrmDataConnection>()
                    .Named<DataConnection>(DataConnectionHelper.ConnectionStringNames.FbmCrm)
                    .InstancePerDependency();
            }

            // Register resolving delegate
            builder.Register<Func<string, DataConnection>>(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                return connectionStringName => cc.ResolveNamed<DataConnection>(connectionStringName);
            });

            builder.Register<Func<string, Lazy<DataConnection>>>(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                return connectionStringName => cc.ResolveNamed<Lazy<DataConnection>>(connectionStringName);
            });
        }

        public static void RegisterRepository(this ContainerBuilder builder, IEnumerable<string> connectionStringNames)
        {
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.Master))
            {
                builder.RegisterGeneric(typeof(EntityRepository<>))
                    .Named(DataConnectionHelper.ConnectionStringNames.Master, typeof(IRepository<>))
                    .WithParameter(new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(DataConnection) && pi.Name == DataConnectionHelper.ParameterName,
                        (pi, ctx) => EngineContext.Current.Resolve<DataConnection>(DataConnectionHelper.ConnectionStringNames.Master)))
                    .InstancePerLifetimeScope();
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.Warehouse))
            {
                builder.RegisterGeneric(typeof(EntityRepository<>))
                    .Named(DataConnectionHelper.ConnectionStringNames.Warehouse, typeof(IRepository<>))
                    .WithParameter(new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(DataConnection) && pi.Name == DataConnectionHelper.ParameterName,
                        (pi, ctx) => EngineContext.Current.Resolve<DataConnection>(DataConnectionHelper.ConnectionStringNames.Warehouse)))
                    .InstancePerLifetimeScope();
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.Asset))
            {
                builder.RegisterGeneric(typeof(EntityRepository<>))
                    .Named(DataConnectionHelper.ConnectionStringNames.Asset, typeof(IRepository<>))
                    .WithParameter(new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(DataConnection) && pi.Name == DataConnectionHelper.ParameterName,
                        (pi, ctx) => EngineContext.Current.Resolve<DataConnection>(DataConnectionHelper.ConnectionStringNames.Asset)))
                    .InstancePerLifetimeScope();
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.Ticket))
            {
                builder.RegisterGeneric(typeof(EntityRepository<>))
                    .Named(DataConnectionHelper.ConnectionStringNames.Ticket, typeof(IRepository<>))
                    .WithParameter(new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(DataConnection) && pi.Name == DataConnectionHelper.ParameterName,
                        (pi, ctx) => EngineContext.Current.Resolve<DataConnection>(DataConnectionHelper.ConnectionStringNames.Ticket)))
                    .InstancePerLifetimeScope();
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.File))
            {
                builder.RegisterGeneric(typeof(EntityRepository<>))
                    .Named(DataConnectionHelper.ConnectionStringNames.File, typeof(IRepository<>))
                    .WithParameter(new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(DataConnection) && pi.Name == DataConnectionHelper.ParameterName,
                        (pi, ctx) => EngineContext.Current.Resolve<DataConnection>(DataConnectionHelper.ConnectionStringNames.File)))
                    .InstancePerLifetimeScope();
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.Notify))
            {
                builder.RegisterGeneric(typeof(EntityRepository<>))
                    .Named(DataConnectionHelper.ConnectionStringNames.Notify, typeof(IRepository<>))
                    .WithParameter(new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(DataConnection) && pi.Name == DataConnectionHelper.ParameterName,
                        (pi, ctx) => EngineContext.Current.Resolve<DataConnection>(DataConnectionHelper.ConnectionStringNames.Notify)))
                    .InstancePerLifetimeScope();
            }

            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.Qlsc))
            {
                builder.RegisterGeneric(typeof(IntLowercaseEntityRepository<>))
                    .Named(DataConnectionHelper.ConnectionStringNames.Qlsc, typeof(IIntLowercaseRepository<>))
                    .WithParameter(new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(DataConnection) && pi.Name == DataConnectionHelper.ParameterName,
                        (pi, ctx) => EngineContext.Current.Resolve<DataConnection>(DataConnectionHelper.ConnectionStringNames.Qlsc)))
                    .InstancePerLifetimeScope();
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.FbmOrganization))
            {
                builder.RegisterGeneric(typeof(IntEntityRepository<>))
                    .Named(DataConnectionHelper.ConnectionStringNames.FbmOrganization, typeof(IIntRepository<>))
                    .WithParameter(new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(DataConnection) && pi.Name == DataConnectionHelper.ParameterName,
                        (pi, ctx) => EngineContext.Current.Resolve<DataConnection>(DataConnectionHelper.ConnectionStringNames.FbmOrganization)))
                    .InstancePerLifetimeScope();
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.FbmContract))
            {
                builder.RegisterGeneric(typeof(IntEntityRepository<>))
                    .Named(DataConnectionHelper.ConnectionStringNames.FbmContract, typeof(IIntRepository<>))
                    .WithParameter(new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(DataConnection) && pi.Name == DataConnectionHelper.ParameterName,
                        (pi, ctx) => EngineContext.Current.Resolve<DataConnection>(DataConnectionHelper.ConnectionStringNames.FbmContract)))
                    .InstancePerLifetimeScope();
            }
            if (connectionStringNames.Contains(DataConnectionHelper.ConnectionStringNames.FbmCrm))
            {
                builder.RegisterGeneric(typeof(IntEntityRepository<>))
                    .Named(DataConnectionHelper.ConnectionStringNames.FbmCrm, typeof(IIntRepository<>))
                    .WithParameter(new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(DataConnection) && pi.Name == DataConnectionHelper.ParameterName,
                        (pi, ctx) => EngineContext.Current.Resolve<DataConnection>(DataConnectionHelper.ConnectionStringNames.FbmCrm)))
                    .InstancePerLifetimeScope();
            }
        }

        public static void RegisterCacheManager(this ContainerBuilder builder)
        {
            builder.RegisterType<XBaseCacheManager>()
                .As<IXBaseCacheManager>()
                .WithParameter(new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(IHybridCachingProvider) && pi.Name == CachingHelper.HybridProviderParameterName,
                    (pi, ctx) => EngineContext.Current.Resolve<IHybridProviderFactory>().GetHybridCachingProvider(CachingHelper.Configs.ProviderNames.Hybrid)))
                .SingleInstance();
        }
    }
}

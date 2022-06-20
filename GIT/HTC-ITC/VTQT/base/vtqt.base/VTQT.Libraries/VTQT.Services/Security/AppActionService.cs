using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using LinqToDB;
using VTQT.Caching;
using VTQT.Core.Data;
using VTQT.Core.Domain.Master;
using VTQT.Core.Events;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Security
{
    public partial class AppActionService : IAppActionService
    {
        #region Constants



        #endregion

        #region Fields

        private readonly IRepository<AppAction> _appActionRepository;
        private readonly IRepository<App> _appRepository;
        private readonly IXBaseCacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public AppActionService(
            IXBaseCacheManager cacheManager,
            IEventPublisher eventPublisher)
        {
            _appActionRepository = EngineContext.Current.Resolve<IRepository<AppAction>>(DataConnectionHelper.ConnectionStringNames.Master);
            _appRepository = EngineContext.Current.Resolve<IRepository<App>>(DataConnectionHelper.ConnectionStringNames.Master);
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        public virtual async Task<int> InsertAsync(AppAction entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var result = await _appActionRepository.InsertAsync(entity);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(MasterCacheKeys.AppActions.Prefix);

            // Event notification
            await _eventPublisher.EntityInsertedAsync(entity);

            return result;
        }

        public virtual async Task<int> UpdateAsync(AppAction entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var result = await _appActionRepository.UpdateAsync(entity);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(MasterCacheKeys.AppActions.Prefix);

            // Event notification
            await _eventPublisher.EntityUpdatedAsync(entity);

            return result;
        }

        public virtual async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _appActionRepository.DeleteAsync(ids);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(MasterCacheKeys.AppActions.Prefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(FrameworkCacheKeys.UserPermissionsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AppActionsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminMenusPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminBreadcrumbsPrefix);

            return result;
        }

        public virtual IList<AppAction> GetAll(bool showHidden = false)
        {
            var key = MasterCacheKeys.AppActions.AllCacheKey.FormatWith(showHidden);
            var entities = _cacheManager.GetDb(key, () =>
            {
                var query =
                    from x in _appActionRepository.Table
                    join a in _appRepository.Table on x.AppId equals a.Id into aj
                    from a in aj
                    orderby a.AppType, x.Controller, x.DisplayOrder
                    select x;

                if (!showHidden)
                    query =
                        from x in query
                        where x.Active
                        select x;

                return query.ToList();
            }, CachingDefaults.MonthCacheTime);

            return entities;
        }

        public virtual async Task<IList<AppAction>> GetByIdsAsync(IEnumerable<string> ids)
        {
            return await _appActionRepository.GetByIdsAsync(ids);
        }

        public virtual async Task<AppAction> GetByIdAsync(string id)
        {
            var query =
                from x in _appActionRepository.Table
                join a in _appRepository.Table on x.AppId equals a.Id into aj
                from a in aj
                where x.Id == id
                select new AppAction
                {
                    Id = x.Id,
                    AppId = x.AppId,
                    ParentId = x.ParentId,
                    Name = x.Name,
                    Description = x.Description,
                    Controller = x.Controller,
                    Action = x.Action,
                    Icon = x.Icon,
                    ShowOnMenu = x.ShowOnMenu,
                    Active = x.Active,
                    DisplayOrder = x.DisplayOrder,
                    App = new App
                    {
                        Id = a.Id,
                        AppType = a.AppType,
                        Name = a.Name,
                        ShortName = a.ShortName,
                        Description = a.Description,
                        Icon = a.Icon,
                        BackgroundColor = a.BackgroundColor,
                        Url = a.Url,
                        Hosts = a.Hosts,
                        SslEnabled = a.SslEnabled,
                        CdnUrl = a.CdnUrl,
                        DefaultLanguageId = a.DefaultLanguageId,
                        ShowOnMenu = a.ShowOnMenu,
                        DisplayOrder = a.DisplayOrder
                    }
                };

            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _appActionRepository.Table
                .Where(w => ids.Contains(w.Id))
                .Set(x => x.Active, active)
                .UpdateAsync();

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(MasterCacheKeys.AppActions.Prefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(FrameworkCacheKeys.UserPermissionsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AppActionsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminMenusPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminBreadcrumbsPrefix);

            return result;
        }

        public virtual AppAction GetFirst(Expression<Func<AppAction, bool>> predicate)
        {
            var query = _appActionRepository.Table
                .Where(predicate);

            query =
                from x in query
                join a in _appRepository.Table on x.AppId equals a.Id into aj
                from a in aj
                let p = _appActionRepository.Table.FirstOrDefault(w => !string.IsNullOrWhiteSpace(x.ParentId) && w.Id == x.ParentId)
                select new AppAction
                {
                    Id = x.Id,
                    AppId = x.AppId,
                    ParentId = x.ParentId,
                    Name = x.Name,
                    Description = x.Description,
                    Controller = x.Controller,
                    Action = x.Action,
                    Icon = x.Icon,
                    ShowOnMenu = x.ShowOnMenu,
                    Active = x.Active,
                    DisplayOrder = x.DisplayOrder,
                    App = new App
                    {
                        Id = a.Id,
                        AppType = a.AppType,
                        Name = a.Name,
                        ShortName = a.ShortName,
                        Description = a.Description,
                        Icon = a.Icon,
                        BackgroundColor = a.BackgroundColor,
                        Url = a.Url,
                        Hosts = a.Hosts,
                        SslEnabled = a.SslEnabled,
                        CdnUrl = a.CdnUrl,
                        DefaultLanguageId = a.DefaultLanguageId,
                        ShowOnMenu = a.ShowOnMenu,
                        DisplayOrder = a.DisplayOrder
                    },
                    Parent = p != null
                        ? new AppAction
                        {
                            Id = p.Id,
                            AppId = p.AppId,
                            ParentId = p.ParentId,
                            Name = p.Name,
                            Description = p.Description,
                            Controller = p.Controller,
                            Action = p.Action,
                            Icon = p.Icon,
                            ShowOnMenu = p.ShowOnMenu,
                            Active = p.Active,
                            DisplayOrder = p.DisplayOrder
                        }
                        : null
                };

            return query.FirstOrDefault();
        }

        public virtual AppAction GetSingle(Expression<Func<AppAction, bool>> predicate)
        {
            var query = _appActionRepository.Table
                .Where(predicate);

            query =
                from x in query
                join a in _appRepository.Table on x.AppId equals a.Id into aj
                from a in aj
                let p = _appActionRepository.Table.FirstOrDefault(w => !string.IsNullOrWhiteSpace(x.ParentId) && w.Id == x.ParentId)
                select new AppAction
                {
                    Id = x.Id,
                    AppId = x.AppId,
                    ParentId = x.ParentId,
                    Name = x.Name,
                    Description = x.Description,
                    Controller = x.Controller,
                    Action = x.Action,
                    Icon = x.Icon,
                    ShowOnMenu = x.ShowOnMenu,
                    Active = x.Active,
                    DisplayOrder = x.DisplayOrder,
                    App = new App
                    {
                        Id = a.Id,
                        AppType = a.AppType,
                        Name = a.Name,
                        ShortName = a.ShortName,
                        Description = a.Description,
                        Icon = a.Icon,
                        BackgroundColor = a.BackgroundColor,
                        Url = a.Url,
                        Hosts = a.Hosts,
                        SslEnabled = a.SslEnabled,
                        CdnUrl = a.CdnUrl,
                        DefaultLanguageId = a.DefaultLanguageId,
                        ShowOnMenu = a.ShowOnMenu,
                        DisplayOrder = a.DisplayOrder
                    },
                    Parent = p != null
                        ? new AppAction
                        {
                            Id = p.Id,
                            AppId = p.AppId,
                            ParentId = p.ParentId,
                            Name = p.Name,
                            Description = p.Description,
                            Controller = p.Controller,
                            Action = p.Action,
                            Icon = p.Icon,
                            ShowOnMenu = p.ShowOnMenu,
                            Active = p.Active,
                            DisplayOrder = p.DisplayOrder
                        }
                        : null
                };

            return query.SingleOrDefault();
        }

        public virtual IList<AppAction> GetChildrenByParentId(string parentId)
        {
            var query =
                from x in _appActionRepository.Table
                join a in _appRepository.Table on x.AppId equals a.Id into aj
                from a in aj
                where x.ParentId == parentId
                orderby x.DisplayOrder
                select new AppAction
                {
                    Id = x.Id,
                    AppId = x.AppId,
                    ParentId = x.ParentId,
                    Name = x.Name,
                    Description = x.Description,
                    Controller = x.Controller,
                    Action = x.Action,
                    Icon = x.Icon,
                    ShowOnMenu = x.ShowOnMenu,
                    Active = x.Active,
                    DisplayOrder = x.DisplayOrder,
                    App = new App
                    {
                        Id = a.Id,
                        AppType = a.AppType,
                        Name = a.Name,
                        ShortName = a.ShortName,
                        Description = a.Description,
                        Icon = a.Icon,
                        BackgroundColor = a.BackgroundColor,
                        Url = a.Url,
                        Hosts = a.Hosts,
                        SslEnabled = a.SslEnabled,
                        CdnUrl = a.CdnUrl,
                        DefaultLanguageId = a.DefaultLanguageId,
                        ShowOnMenu = a.ShowOnMenu,
                        DisplayOrder = a.DisplayOrder
                    }
                };

            return query.ToList();
        }

        public virtual async Task<int> ShowOnMenuAsync(IEnumerable<string> ids, bool showOnMenu)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _appActionRepository.Table
                .Where(w => ids.Contains(w.Id))
                .Set(x => x.ShowOnMenu, showOnMenu)
                .UpdateAsync();

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(MasterCacheKeys.AppActions.Prefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(FrameworkCacheKeys.UserPermissionsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AppActionsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminMenusPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminBreadcrumbsPrefix);

            return result;
        }

        public virtual async Task MoveActionAsync(AppAction entity, string parentId, int? displayOrder)
        {
            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = TransactionConfig.IsolationLevel },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                entity.ParentId = parentId;
                if (displayOrder.HasValue)
                    entity.DisplayOrder = displayOrder.Value;

                await _appActionRepository.UpdateAsync(entity, s => new { s.ParentId, s.DisplayOrder });

                var siblings = _appActionRepository.Table.Where(w => w.ParentId == entity.ParentId).OrderBy(o => o.DisplayOrder);
                var idx = 1;
                foreach (var action in siblings)
                {
                    action.DisplayOrder = idx;

                    await _appActionRepository.UpdateAsync(entity, s => new { s.DisplayOrder });

                    idx++;
                }

                transaction.Complete();

                await _cacheManager.HybridProvider.RemoveByPrefixAsync(MasterCacheKeys.AppActions.Prefix);
                await _cacheManager.HybridProvider.RemoveByPrefixAsync(FrameworkCacheKeys.UserPermissionsPrefix);
                await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AppActionsPrefix);
                await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminMenusPrefix);
                await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminBreadcrumbsPrefix);
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using LinqToDB;
using VTQT.Caching;
using VTQT.Core.Data;
using VTQT.Core.Domain.Master;
using VTQT.Core.Domain.Security;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Security
{
    // Sync Cache with FrameworkCacheEventConsumer
    public partial class PermissionService : IPermissionService
    {
        #region Fields

        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<AppAction> _appActionRepository;
        private readonly IRepository<AppActionRole> _appActionRoleRepository;
        private readonly IRepository<AppActionUserInclusion> _appActionUserInclusionRepository;
        private readonly IRepository<AppActionUserExclusion> _appActionUserExclusionRepository;
        private readonly IRepository<App> _appRepository;
        private readonly MasterDataConnection _masterDataConnection;
        private readonly IXBaseCacheManager _cacheManager;

        #endregion

        #region Ctor

        public PermissionService(
            MasterDataConnection masterDataConnection,
            IXBaseCacheManager cacheManager)
        {
            _roleRepository = EngineContext.Current.Resolve<IRepository<Role>>(DataConnectionHelper.ConnectionStringNames.Master);
            _userRoleRepository = EngineContext.Current.Resolve<IRepository<UserRole>>(DataConnectionHelper.ConnectionStringNames.Master);
            _appActionRepository = EngineContext.Current.Resolve<IRepository<AppAction>>(DataConnectionHelper.ConnectionStringNames.Master);
            _appActionRoleRepository = EngineContext.Current.Resolve<IRepository<AppActionRole>>(DataConnectionHelper.ConnectionStringNames.Master);
            _appActionUserInclusionRepository = EngineContext.Current.Resolve<IRepository<AppActionUserInclusion>>(DataConnectionHelper.ConnectionStringNames.Master);
            _appActionUserExclusionRepository = EngineContext.Current.Resolve<IRepository<AppActionUserExclusion>>(DataConnectionHelper.ConnectionStringNames.Master);
            _appRepository = EngineContext.Current.Resolve<IRepository<App>>(DataConnectionHelper.ConnectionStringNames.Master);
            _masterDataConnection = masterDataConnection;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        #region Role

        public virtual async Task UpdateRolePermissionsAsync(string roleId, IEnumerable<string> appActionIds, string appId)
        {
            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = TransactionConfig.IsolationLevel },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var oldAppActionRoles =
                    from x in _appActionRoleRepository.Table
                    join aa in _appActionRepository.Table on x.AppActionId equals aa.Id into aaj
                    from aa in aaj
                    where aa.AppId == appId && x.RoleId == roleId
                    select x;

                await oldAppActionRoles.DeleteAsync();

                var newAppActionRoles = new List<AppActionRole>();
                foreach (var appActionId in appActionIds)
                {
                    newAppActionRoles.Add(new AppActionRole
                    {
                        RoleId = roleId,
                        AppActionId = appActionId
                    });
                }

                await _appActionRoleRepository.InsertAsync(newAppActionRoles);

                transaction.Complete();

                await _cacheManager.HybridProvider.RemoveByPrefixAsync(FrameworkCacheKeys.UserPermissionsPrefix);
            }
        }

        public virtual async Task RemoveAllRolePermissionsAsync(string roleId, string appId)
        {
            var oldAppActionRoles =
                from x in _appActionRoleRepository.Table
                join aa in _appActionRepository.Table on x.AppActionId equals aa.Id into aaj
                from aa in aaj
                where aa.AppId == appId && x.RoleId == roleId
                select x;

            await oldAppActionRoles.DeleteAsync();

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(FrameworkCacheKeys.UserPermissionsPrefix);
        }

        public virtual async Task<IList<AppAction>> GetRolePermissionsAsync(string roleId)
        {
            var query =
                from x in _appActionRoleRepository.Table
                join aa in _appActionRepository.Table on x.AppActionId equals aa.Id into aaj
                from aa in aaj
                join a in _appRepository.Table on aa.AppId equals a.Id into aj
                from a in aj
                where x.RoleId == roleId
                orderby a.AppType, aa.Controller, aa.DisplayOrder
                select aa;

            return await query.ToListAsync();
        }

        public virtual async Task<IList<Role>> GetAuthorizedRolesAsync(string appActionId)
        {
            var query =
                from x in _appActionRoleRepository.Table
                join r in _roleRepository.Table on x.RoleId equals r.Id into rj
                from r in rj
                where x.AppActionId == appActionId
                orderby r.DisplayOrder, r.Name
                select r;

            return await query.ToListAsync();
        }

        #endregion

        #region User

        public virtual async Task UpdateUserPermissionsAsync(string userId, IEnumerable<string> appActionIds, string appId)
        {
            var userRoleAppActions = await
                (from x in _appActionRoleRepository.Table
                 join ur in _userRoleRepository.Table on x.RoleId equals ur.RoleId into urj
                 from ur in urj
                 join aa in _appActionRepository.Table on x.AppActionId equals aa.Id into aaj
                 from aa in aaj
                 where ur.UserId == userId && aa.AppId == appId
                 select aa).ToListAsync();
            var userRoleAppActionIds = userRoleAppActions.Select(s => s.Id);

            var includedUserAppActions = await
                (from x in _appActionUserInclusionRepository.Table
                 join aa in _appActionRepository.Table on x.IncludeAppActionId equals aa.Id into aaj
                 from aa in aaj
                 select aa).ToListAsync();
            var includedUserAppActionIds = includedUserAppActions.Select(s => s.Id);
            var excludedUserAppActions = await
                (from x in _appActionUserExclusionRepository.Table
                 join aa in _appActionRepository.Table on x.ExcludeAppActionId equals aa.Id into aaj
                 from aa in aaj
                 select aa).ToListAsync();
            var excludedUserAppActionIds = excludedUserAppActions.Select(s => s.Id);

            var includeAppActionIds = appActionIds.Except(userRoleAppActionIds);
            var excludeAppActionIds = userRoleAppActionIds.Except(appActionIds);

            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = TransactionConfig.IsolationLevel },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appActionUserInclusionRepository.DeleteAsync(w =>
                    w.IncludeUserId == userId && includedUserAppActionIds.Contains(w.IncludeAppActionId));
                await _appActionUserExclusionRepository.DeleteAsync(w =>
                    w.ExcludeUserId == userId && excludedUserAppActionIds.Contains(w.ExcludeAppActionId));

                var includeAppActions = new List<AppActionUserInclusion>();
                includeAppActionIds.Each(x => includeAppActions.Add(new AppActionUserInclusion
                {
                    IncludeUserId = userId,
                    IncludeAppActionId = x
                }));
                var excludeAppActions = new List<AppActionUserExclusion>();
                excludeAppActionIds.Each(x => excludeAppActions.Add(new AppActionUserExclusion
                {
                    ExcludeUserId = userId,
                    ExcludeAppActionId = x
                }));

                await _appActionUserInclusionRepository.InsertAsync(includeAppActions);
                await _appActionUserExclusionRepository.InsertAsync(excludeAppActions);

                transaction.Complete();

                await _cacheManager.HybridProvider.RemoveByPrefixAsync(FrameworkCacheKeys.UserPermissionsCacheKey.FormatWith(userId));
            }
        }

        public virtual async Task RemoveAllUserPermissionsAsync(string userId, string appId)
        {
            var includedUserAppActions = await
                (from x in _appActionUserInclusionRepository.Table
                 join aa in _appActionRepository.Table on x.IncludeAppActionId equals aa.Id into aaj
                 from aa in aaj
                 select aa).ToListAsync();
            var includedUserAppActionIds = includedUserAppActions.Select(s => s.Id);
            var excludedUserAppActions = await
                (from x in _appActionUserExclusionRepository.Table
                 join aa in _appActionRepository.Table on x.ExcludeAppActionId equals aa.Id into aaj
                 from aa in aaj
                 select aa).ToListAsync();
            var excludedUserAppActionIds = excludedUserAppActions.Select(s => s.Id);

            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = TransactionConfig.IsolationLevel },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appActionUserInclusionRepository.DeleteAsync(w =>
                    w.IncludeUserId == userId && includedUserAppActionIds.Contains(w.IncludeAppActionId));
                await _appActionUserExclusionRepository.DeleteAsync(w =>
                    w.ExcludeUserId == userId && excludedUserAppActionIds.Contains(w.ExcludeAppActionId));

                transaction.Complete();

                await _cacheManager.HybridProvider.RemoveByPrefixAsync(FrameworkCacheKeys.UserPermissionsCacheKey.FormatWith(userId));
            }
        }

        public virtual IList<AppAction> GetAllUserPermissions(string userId)
        {
            var userRoleIds =
                (from x in _userRoleRepository.Table
                where x.UserId == userId
                select x.RoleId).ToList();

            var excludeAppActionIds =
                (from x in _appActionUserExclusionRepository.Table
                where x.ExcludeUserId == userId
                select x.ExcludeAppActionId).ToList();

            var query =
                from x in _appActionRepository.Table
                join ar in _appActionRoleRepository.Table on x.Id equals ar.AppActionId into arj
                from ar in arj
                join iu in _appActionUserInclusionRepository.Table on x.Id equals iu.IncludeAppActionId into iuj
                from iu in iuj.DefaultIfEmpty()
                join a in _appRepository.Table on x.AppId equals a.Id into aj
                from a in aj
                where
                    (userRoleIds.Contains(ar.RoleId) || iu.IncludeUserId == userId)
                    //&& excludeAppActionIds.All(excludeAppActionId => excludeAppActionId != x.Id)
                    && (!excludeAppActionIds.Any() || (excludeAppActionIds.Any() && !excludeAppActionIds.Contains(x.Id)))
                    && x.Active
                orderby a.AppType, x.Controller, x.DisplayOrder
                select x;

            return query.ToList();
        }

        public virtual async Task<IList<AppAction>> GetUserPermissionsAsync(string userId, string appId)
        {
            var userRoleIds = await
                (from x in _userRoleRepository.Table
                 where x.UserId == userId
                 select x.RoleId).ToListAsync();

            var excludeAppActionIds = await
                (from x in _appActionUserExclusionRepository.Table
                 join aa in _appActionRepository.Table on x.ExcludeAppActionId equals aa.Id into aaj
                 from aa in aaj
                 where x.ExcludeUserId == userId && aa.AppId == appId
                 select x.ExcludeAppActionId).ToListAsync();

            var query =
                from x in _appActionRepository.Table
                join ar in _appActionRoleRepository.Table on x.Id equals ar.AppActionId into arj
                from ar in arj
                join iu in _appActionUserInclusionRepository.Table on x.Id equals iu.IncludeAppActionId into iuj
                from iu in iuj
                join a in _appRepository.Table on x.AppId equals a.Id into aj
                from a in aj
                where
                    x.AppId == appId
                    && (userRoleIds.Contains(ar.RoleId) || iu.IncludeUserId == userId)
                    && !excludeAppActionIds.Contains(x.Id)
                    && x.Active
                orderby a.AppType, x.Controller, x.DisplayOrder
                select x;

            return await query.ToListAsync();
        }

        #endregion

        public virtual bool Authorize(AppAction appAction, IEnumerable<AppActionSvcEntity> permissions)
        {
            return permissions.Any(a =>
                !a.AppId.IsEmpty() && !a.Controller.IsEmpty() && !a.Action.IsEmpty()
                && a.AppId.Equals(appAction.AppId, StringComparison.OrdinalIgnoreCase)
                && a.Controller.Equals(appAction.Controller, StringComparison.OrdinalIgnoreCase)
                && a.Action.Equals(appAction.Action, StringComparison.OrdinalIgnoreCase)
            );
        }

        #endregion
    }
}

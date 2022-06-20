using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Core.Domain;
using VTQT.Core.Domain.Master;
using VTQT.Core.Events;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Security
{
    public partial class RoleService : IRoleService
    {
        #region Constants



        #endregion

        #region Fields

        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IXBaseCacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public RoleService(
            IXBaseCacheManager cacheManager,
            IEventPublisher eventPublisher)
        {
            _roleRepository = EngineContext.Current.Resolve<IRepository<Role>>(DataConnectionHelper.ConnectionStringNames.Master);
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        public virtual async Task<int> InsertAsync(Role entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var result = await _roleRepository.InsertAsync(entity);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(MasterCacheKeys.Roles.Prefix);

            // Event notification
            await _eventPublisher.EntityInsertedAsync(entity);

            return result;
        }

        public virtual async Task<int> UpdateAsync(Role entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var result = await _roleRepository.UpdateAsync(entity);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(MasterCacheKeys.Roles.Prefix);

            // Event notification
            await _eventPublisher.EntityUpdatedAsync(entity);

            return result;
        }

        public virtual async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _roleRepository.DeleteAsync(ids);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(MasterCacheKeys.Roles.Prefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(FrameworkCacheKeys.UserPermissionsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminMenusPrefix);

            return result;
        }

        public virtual IList<Role> GetAll(bool showHidden = false)
        {
            var key = MasterCacheKeys.Roles.AllCacheKey.FormatWith(showHidden);
            var entities = _cacheManager.GetDb(key, () =>
            {
                var query = from p in _roleRepository.Table select p;
                if (!showHidden)
                    query =
                        from p in query
                        where p.Active
                        select p;

                query =
                    from p in query
                    orderby p.Name
                    select p;

                return query.ToList();
            }, CachingDefaults.MonthCacheTime);

            return entities;
        }

        public virtual IPagedList<Role> Get(RoleSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _roleRepository.Table select p;

            if (ctx.Keywords.HasValue())
            {
                query = query.LeftJoin(_localizedPropertyRepository.Table,
                        (e, l) => e.Id == l.EntityId,
                        (e, l) => new { e, l })
                    .Where(
                        el =>
                            el.e.Name.Contains(ctx.Keywords) ||
                            el.e.DisplayName.Contains(ctx.Keywords) ||
                            (el.l.LanguageId == ctx.LanguageId &&
                             el.l.LocaleKeyGroup == nameof(Role) &&
                             el.l.LocaleKey == nameof(Role.DisplayName) &&
                             el.l.LocaleValue.Contains(ctx.Keywords)))
                    .Select(el => el.e).Distinct();
            }
            if (ctx.Status == (int)ActiveStatus.Activated)
            {
                query =
                    from p in query
                    where p.Active
                    select p;
            }
            if (ctx.Status == (int)ActiveStatus.Deactivated)
            {
                query =
                    from p in query
                    where !p.Active
                    select p;
            }

            query =
                from p in query
                orderby p.Name
                select p;

            return new PagedList<Role>(query, ctx.PageIndex, ctx.PageSize);
        }

        public virtual async Task<Role> GetByIdAsync(string id)
        {
            if (id.IsEmpty())
                throw new ArgumentException(nameof(id));

            return await _roleRepository.GetByIdAsync(id);
        }

        public virtual async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _roleRepository.Table
                .Where(w => ids.Contains(w.Id))
                .Set(x => x.Active, active)
                .UpdateAsync();

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(MasterCacheKeys.Roles.Prefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(FrameworkCacheKeys.UserPermissionsPrefix);
            await _cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminMenusPrefix);

            return result;
        }

        public virtual async Task<bool> ExistsAsync(string code)
        {
            return await _roleRepository.Table
                .AnyAsync(
                    a =>
                        !string.IsNullOrEmpty(a.Name)
                        && a.Name.Equals(code));
        }

        public virtual async Task<bool> ExistsAsync(string oldCode, string newCode)
        {
            return await _roleRepository.Table
                .AnyAsync(
                    a =>
                        !string.IsNullOrEmpty(a.Name)
                        && a.Name.Equals(newCode)
                        && !a.Name.Equals(oldCode));
        }

        #endregion
    }
}

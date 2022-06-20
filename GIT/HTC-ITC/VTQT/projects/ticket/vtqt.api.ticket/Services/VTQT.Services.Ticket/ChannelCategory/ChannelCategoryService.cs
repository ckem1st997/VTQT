using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Domain;
using VTQT.Core.Domain.Master;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Ticket
{
    public class ChannelCategoryService : IChannelCategoryService
    {
        #region Fields
        private readonly IRepository<ChannelArea> _channelAreaServiceRepository;
        private readonly IRepository<ChannelCategory> _channelCategoryServiceRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;

        #endregion

        #region Ctor

        public ChannelCategoryService()
        {
            _channelAreaServiceRepository = EngineContext.Current.Resolve<IRepository<ChannelArea>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _channelCategoryServiceRepository = EngineContext.Current.Resolve<IRepository<ChannelCategory>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
        }

        #endregion

        #region Methods

        public async Task<int> InsertAsync(ChannelCategory entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _channelCategoryServiceRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(ChannelCategory entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _channelCategoryServiceRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _channelCategoryServiceRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<ChannelCategory> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _channelCategoryServiceRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<ChannelCategory> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = await _channelCategoryServiceRepository.Table.FirstOrDefaultAsync(x => x.Code == code);

            return result;
        }

        public async Task<bool> ExistedAsync(string code)
        {
            return await _channelCategoryServiceRepository.Table
                .AnyAsync(x => x.Code != null &&
                          x.Code.Equals(code));
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _channelCategoryServiceRepository.Table
                         .Where(w => ids.Contains(w.Id))
                         .Set(x => x.Inactive, !active)
                         .UpdateAsync();

            return result;
        }

        public virtual IPagedList<ChannelCategory> Get(ChannelCategorySearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _channelCategoryServiceRepository.Table
                        join u in _channelAreaServiceRepository.Table on i.ChannelAreaId equals u.Id into ui
                        from iu in ui.DefaultIfEmpty()
                        select new ChannelCategory
                        {
                            Id = i.Id,
                            Code = i.Code,
                            Name = i.Name,
                            ChannelAreaId = iu.Name,
                            Inactive = i.Inactive,
                            Color = i.Color,
                            ParentId=i.ParentId,
                            Path = i.Path
                        };

            if (ctx.Keywords.HasValue())
            {
                query = query.LeftJoin(_localizedPropertyRepository.DataConnection.GetTable<LocalizedProperty>().DatabaseName(DbHelper.DbNames.Master),
                        (e, l) => e.Id == l.EntityId,
                        (e, l) => new { e, l })
                    .Where(
                        el =>
                            el.e.Code.Contains(ctx.Keywords) ||
                            el.e.Name.Contains(ctx.Keywords) ||
                            (el.l.LanguageId == ctx.LanguageId &&
                             el.l.LocaleKeyGroup == nameof(Link) &&
                             el.l.LocaleKey == nameof(Link.Name) &&
                             el.l.LocaleValue.Contains(ctx.Keywords)))
                    .Select(el => el.e).Distinct();
            }
            if (ctx.Status == (int)ActiveStatus.Activated)
            {
                query =
                    from p in query
                    where !p.Inactive
                    select p;
            }
            if (ctx.Status == (int)ActiveStatus.Deactivated)
            {
                query =
                    from p in query
                    where p.Inactive
                    select p;
            }

            query =
                from p in query
                orderby p.Code
                select p;

            return new PagedList<ChannelCategory>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<ChannelCategory> GetAll(bool showHidden = false)
        {
            var query = from p in _channelCategoryServiceRepository.Table select p;
            if (!showHidden)
            {
                query = from p in query where !p.Inactive select p;
            }
            query = from p in query orderby p.Code select p;
            return query.ToList();
        }

        public virtual async Task<bool> ExistsAsync(string code)
        {
            return await _channelCategoryServiceRepository.Table
                .AnyAsync(
                    a =>
                        !string.IsNullOrEmpty(a.Code)
                        && a.Code.Equals(code));
        }

        public virtual async Task<bool> ExistsAsync(string oldCode, string newCode)
        {
            return await _channelCategoryServiceRepository.Table
                .AnyAsync(
                    a =>
                        !string.IsNullOrEmpty(a.Code)
                        && a.Code.Equals(newCode)
                        && !a.Code.Equals(oldCode));
        }

        public IList<SelectListItem> GetMvcListItems(bool showHidden)
        {
            var results = new List<SelectListItem>();

            var query = from p in _channelCategoryServiceRepository.Table select p;
            if (!showHidden)
            {
                query = from p in query
                    where !p.Inactive
                    select p;
            }

            query = from p in query
                orderby p.Code
                select p;

            if (query?.ToList()?.Count > 0)
            {
                query.ToList().ForEach(x =>
                {
                    var m = new SelectListItem
                    {
                        Value = x.Id,
                        Text = $"[{x.Code}] {x.Name}"
                    };
                    results.Add(m);
                });
            }

            return results;
        }

        #endregion

        #region List

        

        #endregion
    }
}
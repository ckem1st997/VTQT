using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VTQT.Core.Domain.Master;

namespace VTQT.Services.Security
{
    public partial interface IAppActionService
    {
        Task<int> InsertAsync(AppAction entity);

        Task<int> UpdateAsync(AppAction entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        IList<AppAction> GetAll(bool showHidden = false);

        Task<IList<AppAction>> GetByIdsAsync(IEnumerable<string> ids);

        Task<AppAction> GetByIdAsync(string id);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        AppAction GetFirst(Expression<Func<AppAction, bool>> predicate);

        AppAction GetSingle(Expression<Func<AppAction, bool>> predicate);

        IList<AppAction> GetChildrenByParentId(string parentId);

        Task<int> ShowOnMenuAsync(IEnumerable<string> ids, bool showOnMenu);

        Task MoveActionAsync(AppAction entity, string parentId, int? displayOrder);
    }
}

using System;
using System.Collections.Generic;
using VTQT.Data;
using VTQT.Services.Security;
using VTQT.SharedMvc.Master.Models;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Core.Infrastructure;

namespace VTQT.Services.Master.UserRole
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IKeycloakService _keycloakService;
        private readonly IRepository<Core.Domain.Master.UserRole> _userRoleRepository;
        private readonly IXBaseCacheManager _cacheManager;

        public UserRoleService(IKeycloakService keycloakService, IXBaseCacheManager cacheManager)
        {
            _keycloakService = keycloakService;
            _userRoleRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Master.UserRole>>(DataConnectionHelper.ConnectionStringNames.Master);
            _cacheManager = cacheManager;
        }

        public IEnumerable<UserModel> GetListManageUserByRole()
        {
            var userList = new List<UserModel>();
            foreach (var VARIABLE in _keycloakService.GetAllUsers())
            {
                userList.Add(new UserModel()
                {
                    Id = VARIABLE.Id,
                    UserName = VARIABLE.UserName,
                    Email = VARIABLE.Email,
                    FirstName = VARIABLE.FirstName,
                    LastName = VARIABLE.LastName,
                    Active = VARIABLE.Active,
                });
            }

            return userList;
        }

        public List<Core.Domain.Master.UserRole> GetListUserByUserId(string userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            var list = _userRoleRepository.Table.Where(x => x.UserId.Equals(userId)).Select(x => x).ToList();
            return list;
        }

        public bool GetUser(string userId, string roleId)
        {
            if (userId == null) 
                throw new ArgumentNullException(nameof(userId));
            if (roleId == null) 
                throw new ArgumentNullException(nameof(roleId));
            var check=from i in _userRoleRepository.Table
                      where i.UserId.Equals(userId) && i.RoleId.Equals(roleId)
                      select i;
            return check.FirstOrDefault() != null;
        }


        public virtual async Task<int> DeletesAsync(IEnumerable<Core.Domain.Master.UserRole> entitys)
        {
            if (entitys == null)
                throw new ArgumentNullException(nameof(entitys));
            int result = await this._userRoleRepository.DeleteAsync(entitys);
            await DeleteCache();
            return result;
        }
        
        public virtual async Task<long> InsertAsync(IEnumerable<Core.Domain.Master.UserRole> entitys)
        {
            if (entitys == null)
                throw new ArgumentNullException(nameof(entitys));
            var result = await this._userRoleRepository.InsertAsync(entitys);
            await DeleteCache();
            return result;
        }

        async Task DeleteCache()
        {
            await this._cacheManager.HybridProvider.RemoveByPrefixAsync(FrameworkCacheKeys.UserPermissionsPrefix);
            await this._cacheManager.HybridProvider.RemoveByPrefixAsync(ModelCacheKeys.AdminMenusPrefix);
        }
    }
}
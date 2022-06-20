using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.SharedMvc.Master.Models;

namespace VTQT.Services.Master.UserRole
{
    public interface IUserRoleService
    {
        public IEnumerable<UserModel> GetListManageUserByRole();
        public List<Core.Domain.Master.UserRole> GetListUserByUserId(string userId);
        public bool GetUser(string userId, string roleId);
        Task<int> DeletesAsync(IEnumerable<Core.Domain.Master.UserRole> entitys);
        Task<long> InsertAsync(IEnumerable<Core.Domain.Master.UserRole> entitys);
    }
}
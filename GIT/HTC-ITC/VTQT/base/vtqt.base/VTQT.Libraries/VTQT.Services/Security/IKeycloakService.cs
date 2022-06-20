using System.Collections.Generic;
using VTQT.Core;
using VTQT.Core.Domain.Security;

namespace VTQT.Services.Security
{
    public interface IKeycloakService
    {
        IPagedList<User> GetUsers(UserSearchContext ctx, string realm = null);

        User GetUserById(string id, string realm = null);

        IList<User> GetAllUsers(bool showHidden = false, string realm = null);

        TokenResponse RequestClientCredentialsToken(string realm, string clientId, string clientSecret, string scope = null);

        TokenResponse RequestPasswordToken(string realm, string userName, string password, string scope = null);

        TokenResponse RequestRefreshToken(string realm, string clientId, string clientSecret, string refreshToken);
    }
}

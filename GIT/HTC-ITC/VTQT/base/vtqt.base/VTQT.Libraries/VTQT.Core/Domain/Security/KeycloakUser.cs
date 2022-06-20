using System;

namespace VTQT.Core.Domain.Security
{
    public class KeycloakUser
    {
        public string id { get; set; }
        public long createdTimestamp { get; set; }
        public string username { get; set; }
        public bool enabled { get; set; }
        public bool emailVerified { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
    }
}

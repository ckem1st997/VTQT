using System;

namespace VTQT.Core.Domain.Security
{
    [Serializable]
    public class User : BaseEntity
    {
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool Active { get; set; }
    }
}

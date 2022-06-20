using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VTQT.Core.Domain.Security
{
    // NotMapped: để tránh lỗi "Invalid column name 'Discriminator'" khi query paging ToList
    [NotMapped]
    [Serializable]
    public class AppActionSvcEntity : BaseEntity
    {
        public string AppId { get; set; } // varchar(36)
        public string ParentId { get; set; } // varchar(36)
        public string Name { get; set; } // varchar(255)
        public string Description { get; set; } // longtext
        public string Controller { get; set; } // varchar(255)
        public string Action { get; set; } // varchar(255)
        public string Icon { get; set; } // varchar(1000)
        public bool ShowOnMenu { get; set; } // tinyint(1)
        public bool Active { get; set; } // tinyint(1)
        public int DisplayOrder { get; set; } // int
	}
}

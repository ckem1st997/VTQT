using System;
using VTQT.Web.Framework.Modelling;
using System.Collections;
using System.Collections.Generic;

namespace VTQT.Web.Master.Models
{
    public class UserRoleModel 
    {
        public static UserRoleModel CreateInstance()
        {
            return new UserRoleModel();
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string ListRole { get; set; }
        
        public object SelectList { get; set; }
    }
}
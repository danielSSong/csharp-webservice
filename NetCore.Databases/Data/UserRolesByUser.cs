//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NetCore.Databases.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserRolesByUser
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public System.DateTime OwnedUtcDate { get; set; }
    
        public virtual User User { get; set; }
        public virtual UserRole UserRole { get; set; }
    }
}

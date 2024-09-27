using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class MUserPermission
    {
        public int UserPermissionId { get; set; }
        public int UserId { get; set; }
        public bool? Permission1 { get; set; }
        public bool? Permission2 { get; set; }
    }
}

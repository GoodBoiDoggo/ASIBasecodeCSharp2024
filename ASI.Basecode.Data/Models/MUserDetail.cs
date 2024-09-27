using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class MUserDetail
    {
        public int UserDetailId { get; set; }
        public int UserId { get; set; }
        public string Detail1 { get; set; }
        public string Detail2 { get; set; }
    }
}

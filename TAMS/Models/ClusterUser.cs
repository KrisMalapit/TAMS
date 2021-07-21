using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TAMS.Models
{
    public class ClusterUser
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
      
        public int UserId { get; set; }
        public string Status { get; set; } = "Active";
        public int ClusterId { get; set; }
    }
}

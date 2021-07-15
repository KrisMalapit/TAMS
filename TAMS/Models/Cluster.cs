using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TAMS.Models
{
    public class Cluster
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Departments { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public virtual User Users { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TAMS.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "VARCHAR(50)")]

        public string Username { get; set; }
        public int RoleId { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Status { get; set; }
        public virtual Role Roles { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TAMS.Models
{
    public class AttendanceLog
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Remarks { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:s}", ApplyFormatInEditMode = true)]
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now.Date;
        public DateTime UpdatedAt { get; set; }
        public virtual Employee Employees { get; set; }
        [Column(TypeName = "decimal(18,1)")]
        public decimal Temperature1 { get; set; }
        [Column(TypeName = "decimal(18,1)")]
        public decimal Temperature2 { get; set; }
        public string EntryType { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TAMS.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string EmployeeNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Designation { get; set; }
        public int DepartmentId { get; set; }
        public int CompanyId { get; set; }
        public int LevelId { get; set; }
        public string Status { get; set; }
        public string LocalNo { get; set; }
        public int EmployeeTypeId { get; set; }
        public virtual Department Department { get; set; }
        public virtual Company Company { get; set; }
        public virtual Level Levels { get; set; }
        public virtual EmployeeType EmployeeTypes { get; set; }
        public string BadgeNo { get; set; }
        public string Imagefilename { get; set; }
    }
}

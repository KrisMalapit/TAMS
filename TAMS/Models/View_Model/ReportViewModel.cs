﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TAMS.Models.View_Model
{
    public class ReportViewModel
    {
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public int[] LevelId { get; set; }
        public int[] EmployeeName { get; set; }
        public int EmployeeType { get; set; }
        public string Report { get; set; }
        public string rptType { get; set; }
        public string deptAccess { get; set; }
    }
}

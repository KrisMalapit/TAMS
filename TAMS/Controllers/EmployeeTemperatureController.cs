using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TAMS.Models;

namespace TAMS.Controllers
{
    public class EmployeeTemperatureController : Controller
    {
        private readonly TAMSContext _context;
        private Claim claimUser;

        public EmployeeTemperatureController(TAMSContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> AuditTrail() {
            return View();
        }


        // GET: EmployeeTemperature
        public async Task<IActionResult> Index()
        {
            var employeeList = _context.Employees.Where(a => a.Status == "Active").Select(b => new
            {
                b.Id,
                Name = b.LastName + ", " + b.FirstName,
            });
            ViewData["Employees"] = new SelectList(employeeList.OrderBy(a => a.Name), "Id", "Name");
            return View();
        }
      
        public IActionResult getData(string start,string end)
        {
            claimUser = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            var userid = claimUser.Value;
            string userrole = User.Claims.FirstOrDefault(c => c.Type == "RoleName").Value;
            string deptAccess = "";
            
            //test only
           DateTime startDate =  DateTime.ParseExact(start, "MM/dd/yyyy", CultureInfo.InvariantCulture);
           DateTime endDate = DateTime.ParseExact(end, "MM/dd/yyyy", CultureInfo.InvariantCulture);


            DateTime dt = new DateTime(1, 1, 1);
            string status = "";
            string message = "";
            string[]stat = new string[2];
            stat[0] = "Active";
            stat[1] = "Manual";
            var v =

                _context.AttendanceLogs
                .Where(a => stat.Contains(a.Status))
                .Where(a=>a.CreatedDate >= startDate && a.CreatedDate <= endDate)
                .Select(a => new {
                    a.Id,
                    a.Employees.EmployeeNo,
                    EmployeeName = a.Employees.LastName + ", " + a.Employees.FirstName,
                    a.Temperature1,
                    a.Temperature2,
                    a.CreatedDate,
                    DepartmentName = a.Employees.Department.Name,
                    a.Employees.DepartmentId
                });

            
            status = "success";
          

            var al = v.ToList();
            if (userrole != "EHSAdmin" && userrole != "Admin")
            {
                //kcm
                //deptAccess = _context.Clusters.FirstOrDefault(a => a.UserId == Convert.ToInt32(userid)).Departments;
                int[] deptid;
                int x = 0;
                var cu = _context.ClusterUsers.Where(a => a.Status == "Active").Where(a => a.UserId == Convert.ToInt32(userid));
                deptid = new int[cu.Count()];
                foreach (var item in cu)
                {
                    deptid[x] = item.DepartmentId;
                    x++;
                }
                //deptAccess = _context.ClusterUsers.Where(a=>a.Status == "Active")
                //    .FirstOrDefault(a => a.UserId == Convert.ToInt32(userid)).Departments;

                //int[] deptId = deptAccess.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                
                v = v.Where(a => deptid.Contains(a.DepartmentId));
            }

            var al2 = v.ToList();

            var model = new
            {
                status
                ,
                data = v.ToList()
            };








            return Json(model);
        }
        public IActionResult getDataAudit(string start, string end)
        {
            string status = "";
          
            var v =

                _context.Logs
                
        
              
                .Select(a => new {

                    a.Id,

                    a.CreatedDate
                                     ,
                    a.Descriptions
                                     ,
                    a.UserId
                                     ,
                    a.Action
                                      
                    



                });


            status = "success";






            var model = new
            {
                status
                ,
                data = v.ToList()
            };


            return Json(model);
        }
        public JsonResult saveTemperature(int id,string type,decimal temp) {
            string message = "";
            string status = "";
           

            try
            {
                var att = _context.AttendanceLogs.Find(id);

                var emp = _context.Employees.Find(att.EmployeeId);
                string empno = emp.EmployeeNo;
                string empname = emp.LastName + ", " + emp.FirstName;
                string attdate = att.CreatedDate.Date.ToString("MM/dd/yyyy");

                if (type == "temp1")
                {
                    att.Temperature1 = temp;
                }
                else
                {
                    att.Temperature2 = temp;
                }
                _context.Entry(att).State = EntityState.Modified;
                _context.SaveChanges();

                status = "success";

                Log log = new Log
                {
                    Module = "Employee Temperature",
                    Descriptions = "Employee No : " + empno + " Employee Name : " + empname + " Attendance Date: " + attdate + " Field: " + type + " Value :" + temp,
                    Action = "Change",
                    Status = "success",
                    UserId = User.Claims.FirstOrDefault(c => c.Type == "UserName").Value
                };

                _context.Add(log);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                status = "fail";
                message =e.Message;
                throw;
            }

            var model = new
            {
                status,
                message
            };


            


            return Json(model);
        }
        public JsonResult saveEmployee(int empid)
        {
            string message = "";
            string status = "";
            string[] stat = new string[2];
            stat[0] = "Active";
            stat[1] = "Manual";

            try
            {
                var emp = _context.AttendanceLogs.Where(a => a.CreatedDate == DateTime.Now.Date).Where(a => a.EmployeeId == empid).Where(a=> stat.Contains(a.Status)).Count();
                if (emp == 0)
                {
                    DateTime dt = new DateTime(0001, 01, 01);
                    AttendanceLog attendanceLog = new AttendanceLog();
                    attendanceLog.EmployeeId = empid;
                    attendanceLog.CreatedDate = DateTime.Now.Date;
                    attendanceLog.TimeIn = dt;
                    attendanceLog.TimeOut = dt;
                    attendanceLog.Status = "Manual";
                    attendanceLog.Type = "Regular";
                    attendanceLog.Remarks = "Manual input from Temperature Module";
                    attendanceLog.EntryType = "Manual";
                    _context.AttendanceLogs.Add(attendanceLog);
                    _context.SaveChanges();

                    status = "success";
                    message = "Data added";

                    Log log = new Log
                    {
                        Module = "AttendanceLog",
                        Descriptions = "Add Daily AttendanceLogs Id : " + attendanceLog.Id,
                        Action = "Add",
                        Status = "success",
                        UserId = User.Claims.FirstOrDefault(c => c.Type == "UserName").Value
                    };

                    _context.Add(log);
                    _context.SaveChanges();


                }
                else
                {
                    status = "success";
                    message = "Data already existing";
                }
               
               
            }
            catch (Exception e)
            {
                status = "fail";
                message = e.Message;
                
            }

            var model = new
            {
                status,
                message
            };





            return Json(model);
        }
      

    }
}

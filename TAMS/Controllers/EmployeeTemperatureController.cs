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
            
            return View();
        }
      
        public IActionResult getData(string start,string end)
        {
            claimUser = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            var userid = claimUser.Value;
            string userrole = User.Claims.FirstOrDefault(c => c.Type == "RoleName").Value;
            string deptAccess = "";
            

           DateTime startDate =  DateTime.ParseExact(start, "MM/dd/yyyy", CultureInfo.InvariantCulture);
           DateTime endDate = DateTime.ParseExact(end, "MM/dd/yyyy", CultureInfo.InvariantCulture);


            DateTime dt = new DateTime(1, 1, 1);
            string status = "";
            var v =

                _context.AttendanceLogs
                .Where(a => a.Status == "Active")
                .Where(a=>a.CreatedDate >= startDate && a.CreatedDate <= endDate)
               
                .Select(a => new {

                    a.Id,
                    
                    a.Employees.EmployeeNo
                                     ,
                    EmployeeName = a.Employees.LastName + ", " + a.Employees.FirstName
                                     ,
                    a.Temperature1
                                     ,
                    a.Temperature2
                                      ,
                    a.CreatedDate,
                    DepartmentName = a.Employees.Department.Name,
                    a.Employees.DepartmentId



                });

            
            status = "success";


            if (userrole != "EHSAdmin" && userrole != "Admin")
            {

                deptAccess = _context.Clusters.FirstOrDefault(a => a.UserId == Convert.ToInt32(userid)).Departments;

                int[] deptId = deptAccess.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                
                v = v.Where(a => deptId.Contains(a.DepartmentId));
            }



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
      
    }
}

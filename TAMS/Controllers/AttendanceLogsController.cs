using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TAMS.Models;

namespace TAMS.Controllers
{
    public class AttendanceLogsController : Controller
    {
        private readonly TAMSContext _context;
        private Claim claimUserName;

        public AttendanceLogsController(TAMSContext context)
        {
           
            _context = context;
           

        }
        private void ResetContextState() => _context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);
       

        // GET: AttendanceLogs
        public ActionResult Index()
        {
            


            return View();
        }

        // GET: AttendanceLogs/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AttendanceLogs/Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult lastTimeInOut(int empid)
        {

            string status = "";
            string message = "";
            string timeIn = "";
            string timeOut = "";
            try
            {

                var attlogs = _context.AttendanceLogs.Where(a => a.Status == "Active")
                           .Where(a => a.EmployeeId == empid)
                           .OrderByDescending(a => a.Id)
                           .FirstOrDefault();
                timeIn  = attlogs.TimeIn.ToString("MM/dd/yyyy HH:mm:ss");
                timeOut = attlogs.TimeOut.ToString("MM/dd/yyyy HH:mm:ss");
                if (timeOut == "01/01/0001 00:00:00")
                {
                    timeOut = "";
                }
                status = "success";

            }
            catch (Exception e)
            {
                status = "fail";
                message = e.Message;
            }
            var res = new
            {
                timeIn
                ,
                timeOut
                
                ,
                message,

                status
            };
            return Json(res);
        }


        [HttpPost]
        public ActionResult saveScreening(ScreeningToolData sd)
        {
            claimUserName = User.Claims.FirstOrDefault(c => c.Type == "UserName");
            string status = "";
            string message = "";

            try
            {
                sd.CreatedDate = DateTime.Now;
                _context.ScreeningToolData.Add(sd);
                _context.SaveChanges();
                status = "success";


            }
            catch (Exception e)
            {

                status = "fail";
                message = e.InnerException.Message;
            }

            Log log = new Log
            {
                Descriptions = "Add data to Screenlogdata ScreenLogID - " + sd.Id,
                Action = "Add",
                Status = status,
                UserId = claimUserName.Value
            };

            _context.Add(log);
            _context.SaveChanges();




            var res = new
            {
                message,
                status
                  
            };

            return Json(res);

        }
            

        [HttpPost]
        public ActionResult badgeData(string badgeNo,string barcode)
        {

            string message = "";
            string status = "";
            string empno = "";
            string empname = "";
            string dept = "";
            string imageFilename = "";
            string employeeno = "";
            int score = 0;
            string remarks = "";
            string strDate = "";
            int quarStat = 0;
            DateTime dt = DateTime.Now;
            DateTime dt2;

            if (!string.IsNullOrEmpty(barcode))
            {
                var param = barcode.Split('-');
                score = Convert.ToInt32(param[2].ToString());
                employeeno = param[3];
                quarStat = Convert.ToInt32(param[4].ToString());


                if (quarStat == 1)
                {
                    remarks = "FAILED - EMPLOYEE UNDER QUARANTINE";
                }
                else
                {
                    if (score < 2)
                    {
                        remarks = "PASSED";
                    }
                    else
                    {
                        remarks = "FAILED";
                    }

                    strDate = param[1].ToString();

                    int M = Convert.ToInt32(strDate.Substring(0, 2));
                    int D = Convert.ToInt32(strDate.Substring(2, 2));
                    int Y = Convert.ToInt32(strDate.Substring(4, 4));
                    int H = Convert.ToInt32(strDate.Substring(8, 2));
                    int m = Convert.ToInt32(strDate.Substring(10, 2));
                    int s = Convert.ToInt32(strDate.Substring(12, 2));

                    dt = new DateTime(Y, M, D, H, m, s);
                    dt2 = dt.AddDays(1);
                    DateTime curDate = DateTime.Now;
                    if (dt2 < curDate)
                    {
                        remarks = "FAILED - OVERDUE";
                    }


                    bool verifiedAlready = _context.ScreeningToolData.Where(a => a.BarCode == barcode).Count() > 0;

                    if (verifiedAlready)
                    {
                        int rem = _context.ScreeningToolData.Where(a => a.BarCode == barcode).FirstOrDefault().Action;
                        if (rem == 1)
                        {
                            remarks = "CLAIMED - ACCEPTED";
                        }
                        else
                        {
                            remarks = "CLAIMED - DENIED";
                        }

                    }
                }
                


               
                

            }
            
            
            int empid = 0;
            try
            {

                var emp = _context.Employees.Include(a => a.Department).Select(a=> new { 
                    a.Id,
                    a.BadgeNo,
                    a.EmployeeNo,
                    a.LastName,
                    a.FirstName,
                    a.Department.Name,
                    a.Imagefilename
                });

                if (string.IsNullOrEmpty(employeeno))
                {
                   emp = emp.Where(a => a.BadgeNo == badgeNo);
                }
                else
                {
                   emp = emp.Where(a => a.EmployeeNo == employeeno);
                }
       
                if (emp!=null)
                {
                    empid = emp.FirstOrDefault().Id;
                    empno = emp.FirstOrDefault().EmployeeNo;
                    empname = emp.FirstOrDefault().LastName + ", " + emp.FirstOrDefault().FirstName; 
                    dept = emp.FirstOrDefault().Name;
                    imageFilename = emp.FirstOrDefault().Imagefilename;

                    if (true)
                    {}

                    status = "success";
                }
                else
                {
                    status = "fail";
                    message = "No record";
                }
               
            }
            catch (Exception e)
            {
                status = "fail";
                message = e.Message;
            }

            var res = new {
                message
                ,empid
                ,empno
                ,empname
                ,dept
                ,imageFilename
                ,status
                ,remarks
                ,dt
            };

            return Json(res);
        }
        // POST: AttendanceLogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AttendanceLogs/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AttendanceLogs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AttendanceLogs/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AttendanceLogs/Delete/5
        [HttpPost]
 
        public ActionResult DeleteLog(int id)
        {
            claimUserName = User.Claims.FirstOrDefault(c => c.Type == "UserName");

            string message = "";
            string status = "";
            try
            {
                var attLogs = _context.AttendanceLogs.Find(id);
                attLogs.Status = "Deleted_" + DateTime.Now ;
                _context.Update(attLogs);
                _context.SaveChanges();

                Log log = new Log
                {
                    Descriptions = "Delete AttendanceLogs - " + id,
                    Action = "Delete",
                    Status = "success",
                    UserId = claimUserName.Value
                };

                _context.Add(log);
                _context.SaveChanges();

                message = "Item Deleted";
                status = "success";

            }
            catch (Exception e)
            {
                ResetContextState();
                Log log = new Log
                {
                    Descriptions = "Delete AttendanceLogs - " + id + " " +  e.InnerException.Message,
                    Action = "Delete",
                    Status = "fail",
                    UserId = claimUserName.Value
                };
                _context.Add(log);
                _context.SaveChanges();

                message = e.InnerException.Message;
                status = "fail";
            }
            var res = new
            {
                message,
                status
            };
            return Json(res);
        }
        [HttpPost]

        public ActionResult timeOut(int id)
        {
            claimUserName = User.Claims.FirstOrDefault(c => c.Type == "UserName");

            string message = "";
            string status = "";
            try
            {
                var attLogs = _context.AttendanceLogs.Find(id);
                attLogs.TimeOut = DateTime.Now;
                _context.Update(attLogs);
                _context.SaveChanges();

                Log log = new Log
                {
                    Descriptions = "Time Out AttendanceLogs - ID " + id,
                    Action = "TimeOut",
                    Status = "success",
                    UserId = claimUserName.Value
                };

                _context.Add(log);
                _context.SaveChanges();

                message = "Time-Out Set";
                status = "success";

            }
            catch (Exception e)
            {
                ResetContextState();
                Log log = new Log
                {
                    Descriptions = "Time Out AttendanceLogs - ID " + id + " " + e.InnerException.Message,
                    Action = "TimeOut",
                    Status = "fail",
                    UserId = claimUserName.Value
                };
                _context.Add(log);
                _context.SaveChanges();

                message = e.InnerException.Message;
                status = "fail";
            }
            var res = new
            {
                message,
                status
            };
            return Json(res);
        }
        public IActionResult timeInput(string EntryType,int [] EmpList,DateTime DateTrans,string Remarks,string logtype) {

          claimUserName = User.Claims.FirstOrDefault(c => c.Type == "UserName");
           



            int EmpID = 0;
            string message = "";
            string status = "";

            int cnt = EmpList.Count();
            try
            {
                if (EntryType == "IN")
                {
                    try
                    {
                        foreach (var item in EmpList)
                        {
                            try
                            {
                                EmpID = item;
                                DateTime dt = new DateTime(0001, 01, 01);
                                AttendanceLog attendanceLog = new AttendanceLog();
                                attendanceLog.EmployeeId = EmpID;
                                attendanceLog.CreatedDate = DateTime.Now.Date;
                                attendanceLog.TimeIn = DateTrans;
                                attendanceLog.TimeOut = dt;
                                attendanceLog.Status = "Active";
                                attendanceLog.Type = "Regular";
                                attendanceLog.Remarks = Remarks;
                                attendanceLog.EntryType = logtype;
                                _context.AttendanceLogs.Add(attendanceLog);
                                _context.SaveChanges();

                                Log log = new Log
                                {
                                    Descriptions = "Add Daily AttendanceLogs TIME-IN - " + attendanceLog.Id + "for EmpId - " + EmpID,
                                    Action = "Add",
                                    Status = "success",
                                    UserId = claimUserName.Value
                            };

                                _context.Add(log);
                                _context.SaveChanges();

                                message = "Time-IN set";
                                status = "success";

                            }
                            catch (Exception e)
                            {
                                ResetContextState();
                                Log log = new Log
                                {
                                    Descriptions = "Add Daily AttendanceLogs TIME-IN " + e.InnerException.Message + " for EmpId - " + EmpID,
                                    Action = "Add",
                                    Status = "fail",
                                    UserId = claimUserName.Value
                                };

                                _context.Add(log);
                                _context.SaveChanges();

                                message = "1 or more employee already has TIME-IN";
                                status = "fail";
                            }

                        }

                    }
                    catch (Exception e)
                    {
                        ResetContextState();

                        Log log = new Log
                        {
                            Descriptions = "Add Daily AttendanceLogs TIME-IN " + e.InnerException.Message + " for EmpId - " + EmpID,
                            Action = "Add",
                            Status = "fail",
                            UserId = claimUserName.Value
                        };
                        _context.Add(log);
                        _context.SaveChanges();

                        message = e.InnerException.Message;
                        status = "fail";
                    }

                }
                else
                {
                    //DateTime dt = DateTime.Now.Date;
                    DateTime dt = new DateTime(0001, 01, 01);
                    foreach (var item in EmpList)
                    {
                        EmpID = item;

                        var attlogs = _context.AttendanceLogs.Where(a => a.Status == "Active")
                             //.Where(a => a.CreatedDate == dt)
                             //.Where(a => a.TimeOut == dt)
                             .Where(a => a.EmployeeId == EmpID)
                             .OrderByDescending(a=>a.Id)
                             .FirstOrDefault();

                        if (attlogs == null)
                        {
                            message = "1 or more employee has no TIME-IN";
                            status = "fail";

                            Log log = new Log
                            {
                                Descriptions = "Add Daily AttendanceLogs TIME-OUT - " + message + " for EmpId - " + EmpID,
                                Action = "Add",
                                Status = "fail",
                                UserId = claimUserName.Value
                            };
                            _context.Add(log);
                            _context.SaveChanges();

                        }
                        else
                        {
                            if (EntryType == "OUT")
                            {
                                attlogs.TimeOut = DateTrans;
                                attlogs.Remarks = attlogs.Remarks + " " + Remarks;
                                _context.Update(attlogs);
                                _context.SaveChanges();
                                message = "Time-OUT set";
                                status = "success";

                                Log log = new Log
                                {
                                    Descriptions = "Add Daily AttendanceLogs TIME-OUT ID: " + attlogs.Id + " for EmpId - " + EmpID,
                                    Action = "Add",
                                    Status = "success",
                                    UserId = claimUserName.Value
                                };
                                _context.Add(log);
                                _context.SaveChanges();
                            }
                            else
                            {
                                string newType = "";

                                if (attlogs.Type == "OB")
                                {
                                    newType = "Regular";
                                }
                                else
                                {
                                    newType = "OB";
                                }


                                attlogs.Type = newType;
                                _context.Update(attlogs);
                                _context.SaveChanges();
                                message = "New TYPE set";
                                status = "success";

                                Log log = new Log
                                {
                                    Descriptions = "Update Daily AttendanceLogs ID: " + attlogs.Id + ". Changes : set new type " + newType,
                                    Action = "Edit",
                                    Status = "success",
                                    UserId = claimUserName.Value
                                };
                                _context.Add(log);
                                _context.SaveChanges();
                            }

                        }


                    }
                   
                }
                
              
            }
            catch (Exception e)
            {
                ResetContextState();
                Log log = new Log
                {
                    Descriptions = "Add Daily AttendanceLogs for EmpId - " + EmpID,
                    Action = "Add",
                    Status = "success",
                    UserId = claimUserName.Value
                };
                _context.Add(log);
                _context.SaveChanges();

                message = e.InnerException.Message;
                status = "fail";
                
            }
            

            var res = new
            {
                message,status
            };
            return Json(res);

        }
        

        public IActionResult getData(string type)
        {
            DateTime dt = new DateTime(1, 1, 1);
            string status = "";
            var v =

                _context.AttendanceLogs.Where(a=>a.Status == "Active").Select(a => new {

                    a.EmployeeId,
                    a.Employees.EmployeeNo
                                   ,
                    EmployeeName = a.Employees.LastName + ", " +  a.Employees.FirstName
                                   ,
                    a.TimeIn
                                   ,
                    a.TimeOut
                                    ,
                    a.Remarks,
                    CompanyName = a.Employees.Company.Name,
                    a.Type,
                    a.Id
               
                    ,a.Employees.Designation
                    ,a.CreatedDate
                    ,
                    employeeType = a.Employees.EmployeeTypes.Name


                });

            if (!string.IsNullOrEmpty(type))
            {
               v = v.Where(a=>a.CreatedDate == DateTime.Now.Date || (a.TimeOut == dt && a.employeeType == "Swiping"));
            }
            status = "success";




          

            var model = new
            {
                status
                ,
                data = v.ToList(),
                totalIn = v.Count(),
                totalOut = v.Where(a => a.TimeOut != dt).Count(),
                totalOB = v.Where(a => a.Type=="OB").Count()
            };


            





            return Json(model);
        }
        public IActionResult getDataDetails(string stat,string type)
        {
            type = "filtered";
            DateTime dt = new DateTime(0001, 01, 01);
            string status = "";
            var v =

                _context.AttendanceLogs.Where(a => a.Status == "Active").Select(a => new {


                    a.Employees.EmployeeNo
                                     ,
                    EmployeeName = a.Employees.LastName + ", " + a.Employees.FirstName
                                     ,
                    a.TimeIn
                                     ,
                    a.TimeOut
                                      ,
                    a.Remarks,
                    CompanyName = a.Employees.Company.Name,
                    a.Type,
                    a.Id
                      ,
                    a.EmployeeId
                      ,
                    a.CreatedDate
                      ,
                    a.Employees.Designation
                     ,
                    employeeType = a.Employees.EmployeeTypes.Name


                });

            if (!string.IsNullOrEmpty(type))
            {
                v = v.Where(a => a.CreatedDate == DateTime.Now.Date || a.TimeOut == dt && a.employeeType == "Swiping");
            }

            if (stat != "TIMEIN")
            {
                if (stat == "TIMEOUT")
                {
                    v = v.Where(a => a.TimeOut != dt);
                }
                if (stat == "VICINITY")
                {
                    v = v.Where(a => a.TimeOut == dt);
                }
                if (stat == "OB")
                {
                    v = v.Where(a => a.Type == "OB");
                }
            }

            status = "success";

            var model = new
            {
                status
                ,
                data = v.ToList()
               
            };








            return Json(model);
        }
        [HttpPost]

        public ActionResult updateDailyLogs(int id,DateTime timein,DateTime timeout,string remarks)
        {
            claimUserName = User.Claims.FirstOrDefault(c => c.Type == "UserName");
            DateTime prevTimeIn;
            DateTime preTimeOut;
            string prevRemarks;

            string message = "";
            string status = "";
            try
            {
                var attLogs = _context.AttendanceLogs.Find(id);

                prevTimeIn = attLogs.TimeIn;
                preTimeOut = attLogs.TimeOut;
                prevRemarks = attLogs.Remarks;

                attLogs.TimeIn = timein;
                attLogs.TimeOut = timeout;
                attLogs.Remarks = remarks;
                _context.Update(attLogs);
                _context.SaveChanges();

                Log log = new Log
                {
                    Descriptions = "Edit Daily AttendanceLogs ID :  " + id + " OrigTimeIN :" + prevTimeIn + " NewTimeIN : " + timein 
                    + " OrigTimeOUT :" + preTimeOut + " NewTimeOUT : " + timeout + " OrigRemarks :" + prevRemarks + " NewRemarks : " + remarks,

                    Action = "Edit",
                    Status = "success",
                    UserId = claimUserName.Value
                };
                _context.Add(log);
                _context.SaveChanges();


                message = "Data updated";
                status = "success";

            }
            catch (Exception e)
            {
                ResetContextState();
                Log log = new Log
                {
                    Descriptions = "Edit Daily AttendanceLogs ID :  " + id + ". Message " + e.InnerException.Message,
                    Action = "Edit",
                    Status = "success",
                    UserId = claimUserName.Value
                };
                _context.Add(log);
                _context.SaveChanges();

                message = e.InnerException.Message;
                status = "fail";
            }
            var res = new
            {
                message,
                status
            };
            return Json(res);
        }
    }
}
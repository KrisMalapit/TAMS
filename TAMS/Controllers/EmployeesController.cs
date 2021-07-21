using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using TAMS.Models;
using TAMS.Models.View_Model;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography;
using System.Text;

namespace TAMS.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly TAMSContext _context;
        private readonly IHostingEnvironment _appEnvironment;

        public EmployeesController(TAMSContext context, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var tAMSContext = _context.Employees.Include(e => e.Company).Include(e => e.Department);
            return View(await tAMSContext.ToListAsync());
        }
        public IActionResult LocalNo()
        {
 
            return View();
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Company)
                .Include(e => e.Department)
                .Include(e => e.Levels)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies.Where(a=>a.Status=="Active"), "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(_context.Departments.Where(a => a.Status == "Active"), "Id", "Name");
            ViewData["LevelId"] = new SelectList(_context.Levels.Where(a => a.Status == "Active"), "Id", "Name");
            ViewData["EmployeeTypeId"] = new SelectList(_context.EmployeeTypes, "Id", "Name");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeNo,FirstName,LastName,Designation,DepartmentId,CompanyId,LevelId,EmployeeTypeId,LocalNo,BadgeNo")] Employee employee)
        {


            var files = HttpContext.Request.Form.Files;
            foreach (var Image in files)
            {
                if (Image != null && Image.Length > 0)
                {
                    var file = Image;
                    var uploads = Path.Combine(_appEnvironment.WebRootPath, "assets\\employees");
                    if (file.Length > 0)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + this.GetUniqueKey() + Path.GetExtension(file.FileName);
                        using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                            employee.Imagefilename = fileName;
                        }

                    }
                }
            }


            if (ModelState.IsValid)
            {
                employee.Status = "Active";
                _context.Add(employee);
                await _context.SaveChangesAsync();

                Log log = new Log
                {
                    Descriptions = "Add Employees - " + employee.Id,
                    Action = "Add",
                    Status = "success",
                    UserId = User.Identity.Name
                };

                _context.Add(log);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies.Where(a => a.Status == "Active"), "Id", "Name", employee.CompanyId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments.Where(a => a.Status == "Active"), "Id", "Name", employee.DepartmentId);
            ViewData["LevelId"] = new SelectList(_context.Levels.Where(a => a.Status == "Active"), "Id", "Name", employee.LevelId);
            ViewData["EmployeeTypeId"] = new SelectList(_context.EmployeeTypes, "Id", "Name", employee.EmployeeTypeId);
            return View(employee);
        }
        private string GetUniqueKey()
        {
            int maxSize = 4;

            //char[] chars = new char[62];
            char[] chars = new char[36];
            string a;
            //a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            a = "abcdefghijklmnopqrstuvwxyz1234567890";


            chars = a.ToCharArray();
            int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length - 1)]);
            }
            return result.ToString();
        }
        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies.Where(a => a.Status == "Active"), "Id", "Name", employee.CompanyId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments.Where(a => a.Status == "Active"), "Id", "Name", employee.DepartmentId);
            ViewData["LevelId"] = new SelectList(_context.Levels.Where(a => a.Status == "Active"), "Id", "Name", employee.LevelId);
            ViewData["EmployeeTypeId"] = new SelectList(_context.EmployeeTypes, "Id", "Name", employee.EmployeeTypeId);
            ViewBag.ImageFilename = employee.Imagefilename;


            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeNo,FirstName,LastName,Designation,DepartmentId,CompanyId,LevelId,Status,EmployeeTypeId,LocalNo,BadgeNo,Imagefilename")] Employee employee)
        {
       

            if (id != employee.Id)
            {
                return NotFound();
            }

            var files = HttpContext.Request.Form.Files;
            foreach (var Image in files)
            {
                if (Image != null && Image.Length > 0)
                {
                    var file = Image;
                    var uploads = Path.Combine(_appEnvironment.WebRootPath, "assets\\employees");
                    if (file.Length > 0)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + this.GetUniqueKey() + Path.GetExtension(file.FileName);
                        using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                            employee.Imagefilename = fileName;
                        }

                    }
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    employee.Status = "Active";
                    _context.Update(employee);
                    await _context.SaveChangesAsync();

                    Log log = new Log
                    {
                        Descriptions = "Edit Employees - " + employee.Id,
                        Action = "Edit",
                        Status = "success",
                        UserId = User.Identity.Name
                    };
                    _context.Add(log);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    Log log = new Log
                    {
                        Descriptions = "Edit Employees - " + employee.Id,
                        Action = "Edit",
                        Status = "success",
                        UserId = User.Identity.Name
                    };
                    _context.Add(log);
                    await _context.SaveChangesAsync();
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies.Where(a => a.Status == "Active"), "Id", "Name", employee.CompanyId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments.Where(a => a.Status == "Active"), "Id", "Name", employee.DepartmentId);
            ViewData["LevelId"] = new SelectList(_context.Levels.Where(a => a.Status == "Active"), "Id", "Name", employee.LevelId);
            ViewData["EmployeeTypeId"] = new SelectList(_context.EmployeeTypes, "Id", "Name", employee.EmployeeTypeId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Company)
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            //_context.Employees.Remove(employee);
            employee.Status = "Deleted";
            await _context.SaveChangesAsync();

            Log log = new Log
            {
                Descriptions = "Delete Employees - " + employee.Id,
                Action = "Delete",
                Status = "success",
                UserId = User.Identity.Name
            };
            _context.Add(log);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
        public IActionResult getData()
        {
            string status = "";
            var v =
                _context.Employees.Where(a => a.Status == "Active").Select(a => new {
                    a.EmployeeNo
                                   ,
                    a.FirstName
                                   ,
                    a.LastName
                                   ,
                    a.Designation
                                   ,
                    DepartmentName = a.Department.Name
                                   ,
                    CompanyName = a.Company.Name,
                    a.Id,
                    a.LocalNo
                    ,EmployeeName  = a.LastName + ", " + a.FirstName


                });
                status = "success";






            var model = new
            {
                status
                ,data = v.ToList()
            };
            return Json(model);
        }
        public JsonResult SearchEmployee_Json(string q)
        {
            IEnumerable<EmployeeViewModel> model = null;


                model = _context.Employees.Where(e => e.Status == "Active").Select(b => new EmployeeViewModel
                {
                    id = b.Id,
                    text = b.LastName + ", " + b.FirstName,
                });
            


            if (!string.IsNullOrEmpty(q))
            {
                model = model.Where(b => b.text.ToUpper().Contains(q.ToUpper()));
            }
           

            var modelEmp = new
            {
                total_count = model.Count(),
                incomplete_results = false,
                items = model.ToList(),
            };
            return Json(modelEmp);
        }
        public JsonResult GetEmployeeInfo (int id)
        {
            string status = "";
            string message = "";

            try
            {
                var emp = _context.Employees.Include(a=>a.Department).Where(a=>a.Id == id);
               
                status = "success";
                var model = new
                {
                    status,
                    message = emp.FirstOrDefault().Department.Name,
                    data = emp,

                };
                return Json(model);
            }
            catch (Exception e)
            {
                status = "fail";
                message = e.Message;
                var model = new
                {
                    status,
                    message

                };
                return Json(model);
            }


            
        }

    }
}

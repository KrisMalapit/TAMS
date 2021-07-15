using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TAMS.Models;
using TAMS.Models.View_Model;

namespace TAMS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly TAMSContext _context;
        private Claim claimUser;

        public HomeController(TAMSContext context)
        {
            _context = context;
        }
       
        public async Task<IActionResult> Index()
        {
            string rolename = User.Claims.FirstOrDefault(c => c.Type == "RoleName").Value;
            if (rolename== "Checker" || rolename == "EHSAdmin")
            {
                return RedirectToAction("Index", "EmployeeTemperature");
            }
            else
            {
                var employeeList = _context.Employees.Where(a => a.Status == "Active").Select(b => new
                {
                    b.Id,
                    Name = b.LastName + ", " + b.FirstName,
                });
                var levelList = _context.Levels.Where(a => a.Status == "Active").Select(b => new
                {
                    b.Id,
                    b.Name,
                });
                ViewData["Employees"] = new SelectList(employeeList.OrderBy(a => a.Name), "Id", "Name");
                ViewData["Levels"] = new SelectList(levelList.OrderBy(a => a.Name), "Id", "Name");
                return View();
            }

            



        }
        public IActionResult test()
        {
            
            return View("Test");



        }


    }
}

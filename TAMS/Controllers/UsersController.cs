using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TAMS.Models;
using TAMS.Models.View_Model;

namespace TAMS.Controllers
{
    public class UsersController : Controller
    {
        private void ResetContextState() => _context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);
        private readonly TAMSContext _context;

        public UsersController(TAMSContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
           
            return View();
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

           


            ViewData["Status"] = user.Status == "1" ? "Enabled" : "Disabled";

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Set<Role>(), "Id", "Name");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel userView)
        {
         
            if (ModelState.IsValid)
            {
                try
                {
                    User user = new User();
                    user.Password = GetSHA1HashData(userView.Password);
                    user.RoleId = userView.RoleId;
                    user.Username = userView.Username;
                    user.Status = "1";
                    _context.Add(user);
                    await _context.SaveChangesAsync();

                   
                    Log log = new Log
                    {
                        Descriptions = "New User - " + user.Id,
                        Action = "Add",
                        Status = "success",
                        UserId = User.Identity.Name
                    };
                    _context.Add(log);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    ResetContextState();
                    Log log = new Log
                    {
                        Descriptions = "New User - " + e.InnerException.Message,
                        Action = "Add",
                        Status = "fail",
                        UserId = User.Identity.Name
                    };
                    _context.Add(log);
                    await _context.SaveChangesAsync();
                    ModelState.AddModelError("", e.InnerException.Message);
                }
               
            }



           


            ViewData["RoleId"] = new SelectList(_context.Set<Role>(), "Id", "Name", userView.RoleId);
            return View(userView);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var itemStatus = new List<SelectListItem>
            {
                new SelectListItem {Text = "Disabled", Value = "0"},
                new SelectListItem {Text = "Enabled", Value = "1"}
            };

            ViewData["RoleId"] = new SelectList(_context.Set<Role>(), "Id", "Name", user.RoleId);
            ViewData["Status"] = new SelectList(itemStatus, "Value", "Text", user.Status);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,RoleId,Status")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var users = _context.Users.Where(a=>a.Id == user.Id).FirstOrDefault();
                    if (users != null)
                    {
                        _context.Entry(users).State = EntityState.Detached;
                        user.Password = users.Password;
                    }
                   
                   
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    Log log = new Log
                    {
                        Descriptions = "Edit User - " + user.Id,
                        Action = "Edit",
                        Status = "success",
                        UserId = User.Identity.Name
                    };
                    _context.Add(log);
                    await _context.SaveChangesAsync();


                }
                catch (DbUpdateConcurrencyException)
                {
                    ResetContextState();
                    Log log = new Log
                    {
                        Descriptions = "Edit User - " + user.Id,
                        Action = "Edit",
                        Status = "fail",
                        UserId = User.Identity.Name
                    };
                    _context.Add(log);
                    await _context.SaveChangesAsync();

                    if (!UserExists(user.Id))
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


            var itemStatus = new List<SelectListItem>
            {
                new SelectListItem {Text = "Disabled", Value = "0"},
                new SelectListItem {Text = "Enabled", Value = "1"}
            };
                    

            ViewData["RoleId"] = new SelectList(_context.Set<Role>(), "Id", "Name", user.RoleId);
            ViewData["Status"] = new SelectList(itemStatus, "Value", "Text", user.Status);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["Status"] = user.Status == "1" ? "Enabled" : "Disabled";
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            //_context.Users.Remove(user);
            user.Status = "Deleted";
            await _context.SaveChangesAsync();


            Log log = new Log
            {
                Descriptions = "Delete User - " + id,
                Action = "Delete",
                Status = "success",
                UserId = User.Identity.Name
            };

            _context.Add(log);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
        public IActionResult getData()
        {
            string status = "";
            var v =

                _context.Users.Where(a=>a.Status != "Deleted").Select(a => new {


                    a.Username
                    ,
                    rolename = a.Roles.Name
                    ,
                    status = a.Status == "1" ? "Enabled" : "Disabled" ,

                    a.Id
                   



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
        private string GetSHA1HashData(string data)
        {
            //create new instance of md5
            SHA1 sha1 = SHA1.Create();

            //convert the input text to array of bytes
            byte[] hashData = sha1.ComputeHash(Encoding.Default.GetBytes(data));

            //create new instance of StringBuilder to save hashed data
            StringBuilder returnValue = new StringBuilder();

            //loop for each byte and add it to StringBuilder
            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());
            }

            // return hexadecimal string
            return returnValue.ToString();
        }
    }
}

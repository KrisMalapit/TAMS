using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAMS.Models;
using TAMS.Models.View_Model;

namespace TAMS.Controllers
{
    public class AccountsController : Controller
    {
        private readonly TAMSContext _context;

        public AccountsController(TAMSContext context)
        {
            _context = context;
        }
        //Get
        //Account/Login
        [AllowAnonymous]

        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }


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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Login", "Accounts");
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = new User() { Username = model.Username, Password = model.Password };

            user = GetUserDetails(user);
            //string s = user.Roles.Name;

            if (user != null)
            {
                var principal = CreatePrincipal(user);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                var log = new Log();

                log.Module = "LOG-IN";
                log.Descriptions = "Username: " + model.Username + " Status : Success";
                log.Action = "Log-In";
                log.Status = "success";
                log.UserId = model.Username;

                _context.Add(log);
                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                var log = new Log();

                log.Module = "LOG-IN";
                log.Descriptions = "Username: " + model.Username + " Status : Failed. Invalid login attempt";
                log.Action = "Log-In";
                log.Status = "failed";
                log.UserId = model.Username;
                
                _context.Add(log);
                _context.SaveChanges();
                return View(model);
            }

        }
        public User GetUserDetails(User user)
        {
             var users = _context.Users.Include(e => e.Roles).Where(u => u.Status == "1")
                .Where(u => u.Username.ToLower() == user.Username.ToLower() &&
                u.Password == GetSHA1HashData(user.Password))
            .FirstOrDefault();


            return users;
            //return users.Where(u=>u.Status == "1")
            //    .Where(u => u.Username.ToLower() == user.Username.ToLower() &&
            //    u.Password == GetSHA1HashData(user.Password))
            //.FirstOrDefault();
        }
        private ClaimsPrincipal CreatePrincipal(User user)
        {
            var claims = new List<Claim>
                {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("UserName", user.Username),
                    new Claim("RoleName", user.Roles.Name)
                };
                var principal = new ClaimsPrincipal();
                principal.AddIdentity(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
                return principal;
        }
    }
}
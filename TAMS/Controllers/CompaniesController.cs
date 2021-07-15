using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TAMS.Models;

namespace TAMS.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly TAMSContext _context;

        public CompaniesController(TAMSContext context)
        {
            _context = context;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Companies.Where(a=>a.Status=="Active").ToListAsync());
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Name")] Company company)
        {
            if (ModelState.IsValid)
            {
                company.Status = "Active";
                _context.Add(company);
                await _context.SaveChangesAsync();

                Log log = new Log
                {
                    Descriptions = "New Company - " + company.Id,
                    Action = "Add",
                    Status = "success",
                    UserId = User.Identity.Name
                };

                _context.Add(log);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Name")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    company.Status = "Active";
                    _context.Update(company);
                    await _context.SaveChangesAsync();

                    Log log = new Log
                    {
                        Descriptions = "Edit Company - " + company.Id,
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
                        Descriptions = "Edit Company - " + company.Id,
                        Action = "Edit",
                        Status = "fail",
                        UserId = User.Identity.Name
                    };
                    _context.Add(log);
                    await _context.SaveChangesAsync();

                    if (!CompanyExists(company.Id))
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
            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            //_context.Companies.Remove(company);
            company.Status = "Deleted";
            _context.Update(company);


            await _context.SaveChangesAsync();


            Log log = new Log
            {
                Descriptions = "Delete Company - " + id,
                Action = "Delete",
                Status = "success",
                UserId = User.Identity.Name
            };
            _context.Add(log);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
    }
}

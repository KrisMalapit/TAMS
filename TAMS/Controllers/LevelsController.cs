using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAMS.Models;

namespace TAMS.Controllers
{
    public class LevelsController : Controller
    {
        private readonly TAMSContext _context;
        public LevelsController(TAMSContext context)
        {
            _context = context;
        }
        // GET: Levels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Levels.Where(a => a.Status == "Active").ToListAsync());
        }

        // GET: Levels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Level = await _context.Levels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Level == null)
            {
                return NotFound();
            }

            return View(Level);
        }

        // GET: Levels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Levels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Name")] Level level)
        {
            if (ModelState.IsValid)
            {
                level.Status = "Active";
                _context.Add(level);
                await _context.SaveChangesAsync();

                Log log = new Log
                {
                    Descriptions = "Add Levels - " + level.Id,
                    Action = "Add",
                    Status = "success",
                    UserId = User.Identity.Name
                };

                _context.Add(log);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(level);
        }

        // GET: Levels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Level = await _context.Levels.FindAsync(id);
            if (Level == null)
            {
                return NotFound();
            }
            return View(Level);
        }

        // POST: Levels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Name")] Level level)
        {
            if (id != level.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    level.Status = "Active";
                    _context.Update(level);
                    await _context.SaveChangesAsync();
                    Log log = new Log
                    {
                        Descriptions = "Edit Levels - " + level.Id,
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
                        Descriptions = "Edit Levels - " + level.Id,
                        Action = "Edit",
                        Status = "fail",
                        UserId = User.Identity.Name
                    };

                    if (!LevelExists(level.Id))
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
            return View(level);
        }

        // GET: Levels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Level = await _context.Levels
                .FirstOrDefaultAsync(m => m.Id == id);


            if (Level == null)
            {
                return NotFound();
            }

            return View(Level);
        }

        // POST: Levels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Level = await _context.Levels.FindAsync(id);
            //_context.Levels.Remove(Level);
            Level.Status = "Deleted";
            _context.Update(Level);

            await _context.SaveChangesAsync();

            Log log = new Log
            {
                Descriptions = "Delete Levels - " + id,
                Action = "Delete",
                Status = "success",
                UserId = User.Identity.Name
            };
            _context.Add(log);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool LevelExists(int id)
        {
            return _context.Levels.Any(e => e.Id == id);
        }
    }
}
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
    public class ClustersController : Controller
    {
        private readonly TAMSContext _context;

        public ClustersController(TAMSContext context)
        {
            _context = context;
        }

        // GET: Clusters
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: Clusters/Details/5
        [HttpGet]
        public IActionResult getData(string type)
        {
            DateTime dt = new DateTime(1, 1, 1);
            string status = "";
            var v =

                _context.Clusters
                .Where(a => a.Status == "Active")
                .Select(a => new
                {
                    a.Id
                    ,
                    a.Code,

                    a.Name,



                    a.Departments,

                    a.Status,
                    //a.Users.Username
                    


                });

            status = "success";
            var model = new
            {
                status
                ,
                data = v.ToList(),


            };
            return Json(model);
        }



        // GET: Clusters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Cluster = await _context.Clusters

                .FirstOrDefaultAsync(m => m.Id == id);
            if (Cluster == null)
            {
                return NotFound();
            }

            return View(Cluster);
        }

        // GET: Clusters/Create
        public IActionResult Create()
        {
            var deptList = _context.Departments.Where(a => a.Status == "Active").Select(b => new
            {
                b.Id,
                b.Name,
            });

            ViewData["DepartmentList"] = new SelectList(deptList.OrderBy(a => a.Name), "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users.OrderBy(a => a.Username), "Id", "Username");

            return View();
        }

        // POST: Clusters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public IActionResult CreateEdit(Cluster Cluster)
        {
            string status = "";
            string message = "";
            Cluster.Status = "Active";
            try
            {
                if (Cluster.Id == 0)
                {
                    _context.Add(Cluster);
                    _context.SaveChanges();
                    status = "success";

                    Log log = new Log();
                    log.Action = "Add";
                    log.Descriptions = "Add Cluster details. Cluster Id : " + Cluster.Id;
                    log.Status = status;
                    _context.Logs.Add(log);
                    _context.SaveChanges();

                }
                else
                {

                    _context.Update(Cluster);
                    _context.SaveChanges();
                    status = "success";

                    Log log = new Log();
                    log.Action = "Modify";
                    log.Descriptions = "Modify Cluster details. Cluster Id : " + Cluster.Id;
                    log.Status = status;
                    _context.Logs.Add(log);
                    _context.SaveChanges();

                }


                int[] deptId = Cluster.Departments.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                int[] userId = Cluster.UserId.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                var cu = _context.ClusterUsers.Where(a => a.ClusterId == Cluster.Id);
                if (cu != null)
                {
                    cu.ToList().ForEach(a => a.Status = "Deleted");
                }
                _context.SaveChanges();
                foreach (var dept in deptId)
                {
                    foreach (var user in userId)
                    {
                        int cntUser = _context.ClusterUsers.Where(a => a.DepartmentId == dept)
                            .Where(a => a.UserId == user)
                            .Where(a=>a.Status == "Active")
                            .Count();

                        if (cntUser == 0)
                        {
                            var cUser = new ClusterUser()
                            {
                                DepartmentId = dept,
                                UserId = user,
                                ClusterId = Cluster.Id
                            };

                            _context.Add(cUser);
                        }

                    };
                    
                };
                _context.SaveChanges();



            }
            catch (Exception e)
            {
                status = "fail";
                message = e.InnerException.Message.ToString();

            }




            var model = new
            {
                status,
                message

            };

            return Json(model);
        }

        // GET: Clusters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cluster = await _context.Clusters.FindAsync(id);
            if (cluster == null)
            {
                return NotFound();
            }

            var deptList = _context.Departments.Where(a => a.Status == "Active").Select(b => new
            {
                b.Id,
                b.Name,
            });
            var userList = _context.Users.Where(a => a.Status == "1").Select(b => new
            {
                b.Id,
                b.Username,
            });
            
            ViewData["DepartmentList"] = new SelectList(deptList.OrderBy(a => a.Name), "Id", "Name");
            ViewData["DepartmentsId"] = cluster.Departments;
            ViewData["UserList"] = new SelectList(userList.OrderBy(a => a.Username), "Id", "Username");
            ViewData["UserId"] = cluster.UserId;
            return View(cluster);
        }

        // POST: Clusters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Name,CompanyId,ClusterHeads,Status")] Cluster Cluster)
        {
            if (id != Cluster.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Cluster);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClusterExists(Cluster.Id))
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

            return View(Cluster);
        }

        // GET: Clusters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Cluster = await _context.Clusters

                .FirstOrDefaultAsync(m => m.Id == id);
            if (Cluster == null)
            {
                return NotFound();
            }

            return View(Cluster);
        }

        // POST: Clusters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Cluster = await _context.Clusters.FindAsync(id);


            //_context.Clusters.Remove(Cluster);
            Cluster.Status = "Deleted";

            await _context.SaveChangesAsync();
            Log log = new Log
            {
                Descriptions = "Delete Cluster - " + Cluster.Id,
                Action = "Delete",
                Status = "success",
                UserId = User.Identity.Name
            };
            _context.Add(log);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClusterExists(int id)
        {
            return _context.Clusters.Any(e => e.Id == id);
        }
    }
}

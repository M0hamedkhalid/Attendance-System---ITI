#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Attendance_System___ITI.Data;
using Attendance_System___ITI.Models;
using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;
using System.Data;
using Microsoft.AspNetCore.Identity;

namespace Attendance_System___ITI.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;

        public InstructorsController( ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager
           )
        {
            _context = context;
            this.userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
        }

        // GET: Instructors
        [Authorize(Roles = "instractor,admin")]
        public async Task<IActionResult> Index(int DeptID , string searchName)
        {
            var instructorsList = new List<Instructor>();

            if (DeptID == 0)
            {
                instructorsList  = await _context.Instructors.ToListAsync();
            }
            else
            {
                instructorsList = await _context.Instructors.Where(ins => ins.DeptID == DeptID).ToListAsync();
            }
            if (searchName != null)
            {
                ViewData["CurrentFilter"] = searchName;
                instructorsList = instructorsList.Where(ins => ins.Name.StartsWith(searchName)).ToList();
            }

            //var applicationDbContext = _context.Instructors.Include(i => i.Credential).Include(i => i.Department);
            ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Name");
            return View(instructorsList);
        }

        // GET: Instructors/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.Credential)
                .Include(i => i.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // GET: Instructors/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["Id"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Id");
            return View();
        }

        // POST: Instructors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create( Instructor instructor)
        {

            var user = CreateUser();

            await _userStore.SetUserNameAsync(user, instructor.Credential.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, instructor.Credential.Email, CancellationToken.None);
            var result = await userManager.CreateAsync(user);
            
            if (result.Succeeded)
            {
                instructor.Id = await userManager.GetUserIdAsync(user);
                ModelState.Remove("Id");


                if (ModelState.IsValid)
                {
                    _context.Add(instructor);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
               
            }


            ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Id", "Name");
            return View(instructor);
        }

        // GET: Instructors/Edit/5
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null)
            {
                return NotFound();
            }
            //ViewData["Id"] = new SelectList(_context.Users, "Id", "Id", instructor.Id);
            ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Name", instructor.DeptID);
            return View(instructor);
        }

        // POST: Instructors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> Edit(string id, Instructor instructor)
        {
            if (id != instructor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instructor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructorExists(instructor.Id))
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
            //ViewData["Id"] = new SelectList(_context.Users, "Id", "Id", instructor.Id);
            ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Name", instructor.DeptID);
            return View(instructor);
        }

        // GET: Instructors/Delete/5
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.Credential)
                .Include(i => i.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();
            var user = await _context.Users.FindAsync(id);
             _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool InstructorExists(string id)
        {
            return _context.Instructors.Any(e => e.Id == id);
        }



        public IActionResult Export()
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[] { new DataColumn("Id"), new DataColumn("Name"),
                                                     new DataColumn("Address"), new DataColumn("Department Name")
            });


            var es = from Instructor in this._context.Instructors.Include(s => s.Department) select Instructor;

            foreach (var ems in es)
            {
                dt.Rows.Add(ems.Id, ems.Name, ems.Address, ems.Department.Name);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Instructors.xlsx");
                }
            }
        }
        private ApplicationUser CreateUser()
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}

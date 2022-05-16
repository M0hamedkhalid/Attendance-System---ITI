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
using Microsoft.EntityFrameworkCore.Query;

namespace Attendance_System___ITI.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Students
        [HttpGet]
        public async Task<IActionResult> Index(int? DeptID)
        {
            if (DeptID == null)
            {
                var applicationDbContext = _context.Students.Include(s => s.Credential).Include(s => s.Department);
                ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Name");
                return View(await applicationDbContext.ToListAsync());
            }
            else
            {
                var applicationDbContext1 = _context.Students.Include(s => s.Credential).Include(s => s.Department).Where(a => a.DeptID == DeptID);
                ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Name");
                return View(await applicationDbContext1.ToListAsync());
            }
        }
        [HttpPost]
        public async Task<IActionResult> Index(string searchtext)
        {
            ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Name");

            //var std = _context.Students.Where(a => a.Name.Contains(searchtext));
            //ViewBag.StdList = await std.ToListAsync();
            List<Student> students;
            ViewData["CurrentFilter"] = searchtext;
           


            if (!String.IsNullOrEmpty(searchtext))
            {
                students = await _context.Students.Where(a => a.Name.Contains(searchtext)).ToListAsync();
            }
            else
            {
                students = await _context.Students.ToListAsync();
            }

            return View( students);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Credential)
                .Include(s => s.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["Id"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Id");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,GraduationYear,GraduationGrade,Mobile,Faculty,University,Address,DeptID")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id"] = new SelectList(_context.Users, "Id", "Id", student.Id);
            ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Id", student.DeptID);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["Id"] = new SelectList(_context.Users, "Id", "Id", student.Id);
            ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Id", student.DeptID);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,GraduationYear,GraduationGrade,Mobile,Faculty,University,Address,DeptID")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
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
            ViewData["Id"] = new SelectList(_context.Users, "Id", "Id", student.Id);
            ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Id", student.DeptID);
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Credential)
                .Include(s => s.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var student = await _context.Students.FindAsync(id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(string id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}

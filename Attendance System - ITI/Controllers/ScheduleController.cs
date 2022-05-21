using Attendance_System___ITI.Data;
using Attendance_System___ITI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Attendance_System___ITI.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScheduleController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult FindAllLectures()
        {
            var lect = _context.Lectures.Include(a => a.Department).Select(l => new
            {
                id = l.Id,
                title = l.Name,
                start = l.Date+l.StartTime,
                end = l.Date+l.EndTime,
                department = l.Department.Name,
                allDay = false



            }).ToList();
            return new JsonResult(lect);
        }

        [Authorize(Roles = "instractor,admin")]

        public IActionResult Create()
        {
            ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "instractor,admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Date,StartTime,EndTime,DeptID")] Lecture lec)
        {
          
                _context.Add(lec);
                await _context.SaveChangesAsync();
                ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Name", lec.DeptID);
                return RedirectToAction("Create");
  

        }
 
        public async Task<IActionResult> List()
        {
            var app = _context.Lectures.Include(s => s.Department);
            ViewData["DeptID"] = new SelectList(_context.Lectures, "Id", "Name");
            return View(await app.ToListAsync());
        }



        public async Task<IActionResult> Delete(int id)
        {
            var lec = await _context.Lectures.FindAsync(id);
            _context.Lectures.Remove(lec);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));


        }



    }
}

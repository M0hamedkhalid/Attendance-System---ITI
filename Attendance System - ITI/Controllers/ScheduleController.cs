using Attendance_System___ITI.Data;
using Attendance_System___ITI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Attendance_System___ITI.Controllers
{
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


        public IActionResult Create()
        {
            ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Date,StartTime,EndTime,DeptID")] Lecture lec)
        {
          
                _context.Add(lec);
                await _context.SaveChangesAsync();
                ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Name", lec.DeptID);
                return RedirectToAction("Create");
  

        }


    }
}

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
namespace Attendance_System___ITI.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Students.Where(S=>S.StudentStatus==0);
            ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Name");
            var attended = _context.Attendances.Include(a=>a.Student).Where(A => A.LeaveTime == null&&A.Date==DateTime.Now.Date&&A.Student.StudentStatus==1);

            ViewBag.Attended =await attended.ToListAsync();
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> Attend(string id)
        {

          var st=  _context.Students.FirstOrDefault(t => t.Id == id);
              st.StudentStatus = 1;
           await _context.SaveChangesAsync();
            var std = _context.Attendances.FirstOrDefault(S => (S.StudentId == id && S.Date == DateTime.Now.Date));
            if (std == null)
            {
                Attendance attendStd = new Attendance();
                attendStd.StudentId = id;
                attendStd.Date = DateTime.Now.Date;
                attendStd.ArriveTime = DateTime.Now.TimeOfDay;
                _context.Attendances.Add(attendStd);
            }
            else
            {
                std.ArriveTime = DateTime.Now.TimeOfDay;
                std.LeaveTime = null;   
               
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Leave(string id)
        {
            var st = _context.Students.FirstOrDefault(t => t.Id == id);
            st.StudentStatus = 0;
            await _context.SaveChangesAsync();
            var LST = _context.Attendances.FirstOrDefault(t => (t.StudentId == id && t.Date == DateTime.Now.Date));
            LST.LeaveTime= DateTime.Now.TimeOfDay;
            await _context.SaveChangesAsync();


            return RedirectToAction("Index");

        }
    }
}

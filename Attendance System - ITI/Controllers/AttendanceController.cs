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
    [Authorize(Roles = "instractor,admin")]
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        [BindProperty(SupportsGet =true)]
        public string DepartmentID { get; set; }

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int DepartmentId, string Flag)
        {
            ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Name");
            if (DepartmentId != 0)
            {
                AttendanceViewModel viewModel = new AttendanceViewModel();
                viewModel.DepartmentId = DepartmentId;
                int deptId = int.Parse(DepartmentID);
                var applicationDbContext = _context.Students.Where(S => S.StudentStatus == 0 && S.DeptID == deptId).Include(S => S.Department);
                var attended = _context.Attendances.Include(a => a.Student).Where(A => A.LeaveTime == null && A.Date == DateTime.Now.Date && A.Student.StudentStatus == 1);

                ViewBag.Attended = await attended.ToListAsync();
                viewModel.Students = await applicationDbContext.ToListAsync();
                viewModel.Flag = Flag?? "attend";
                return View(viewModel);
            }
            return View();
            
        }
        public async Task<IActionResult> Attend(string id , int DepartmentId )
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
            return Redirect($"/Attendance/?DepartmentId={DepartmentID}&Flag=attend");

        }

        public async Task<IActionResult> Leave(string id, int DepartmentId)
        {
            var st = _context.Students.FirstOrDefault(t => t.Id == id);
            st.StudentStatus = 0;
            await _context.SaveChangesAsync();
            var LST = _context.Attendances.FirstOrDefault(t => (t.StudentId == id && t.Date == DateTime.Now.Date));
            LST.LeaveTime= DateTime.Now.TimeOfDay;
            await _context.SaveChangesAsync();


            return Redirect($"/Attendance/?DepartmentId={DepartmentID}&Flag=leave");

        }
    }
}

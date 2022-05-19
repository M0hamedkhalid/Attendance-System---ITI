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
using System.Net.Mail;
using System.Net;
using System.Data;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using Attendance_System___ITI.Areas.Identity.Pages.Account;
using Microsoft.EntityFrameworkCore.Query;
using System.Data;
using System.Reflection;
using ClosedXML.Excel;

namespace Attendance_System___ITI.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<StudentRegisterModel> _logger;

        public StudentsController(ApplicationDbContext context,UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<StudentRegisterModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
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

            return View(students);
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
            ViewBag.Warnings = student.Warning;

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
            ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student st)
        {


            var user = CreateUser();

            await _userStore.SetUserNameAsync(user,st.Credential.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user,st.Credential.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user);


            _logger.LogInformation("Accout created to  Student by instractor and student must register");
            var userId = await _userManager.GetUserIdAsync(user);

            await _userManager.AddToRoleAsync(user, "student");


            Student std = new Student();
            std.Id = userId;
            std.Name = st.Name;
            std.Address = st.Address;
            std.DeptID = st.DeptID;
            std.Faculty = st.Faculty;
            std.University = st.University;
            std.GraduationGrade = st.GraduationGrade;
            std.GraduationYear = st.GraduationYear;
            std.Mobile = st.Mobile;
            std.StudentStatus = 0;
            _context.Add(std);
            await _context.SaveChangesAsync();



            return RedirectToAction(nameof(Index));
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
            ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Name");
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
            
            ViewData["Id"] = new SelectList(_context.Users, "Id", "Id", student.Id);
            ViewData["DeptID"] = new SelectList(_context.Departments, "Id", "Name");
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
        public async Task<IActionResult> AddWarning(string Id)
        {
            
            var std = await _context.Students.Include(s => s.Credential).FirstOrDefaultAsync(s=> s.Id == Id);
            string stdEmail = std.Credential.Email;
            string sender= HttpContext.User.Identity.Name;
            string email = $"Dear {std.Name}\n" +
                $",\n As your instructor, I am writing because I am concerned about your current Attendance & grade in the course, and I want to help you get back on track to successfully complete the course.There are many resources for academic help \n" +
                $"\n Regards,\n" +
                $" {sender}";
            if (std != null)
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();

                message.From = new MailAddress("itiegypt2022@gmail.com");
                message.To.Add(new MailAddress("mkhaledmohamed07@gmail.com"));
                message.Subject = "Warning";
                message.IsBodyHtml = false; //to make message body as html  
                message.Body = email;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("itiegypt2022@gmail.com", "Iti20222021");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                std.Warning = std.Warning+1;
                await _context.SaveChangesAsync();
            }
            return Redirect($"/Students/Details/{Id}");
            
        }

        public async Task<IActionResult> Expel(string Id)
        {
            var std = await _context.Students.FirstOrDefaultAsync(s => s.Id == Id);
             _context.Students.Remove(std);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Export()
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[] { new DataColumn("Id"), new DataColumn("Name"),
                                                     new DataColumn("GraduationYear"), new DataColumn("GraduationGrade"),
                                                     new DataColumn("Mob1ile"), new DataColumn("Faculty"),
                                                     new DataColumn("University"), new DataColumn("Address"),
                                                     new DataColumn("Department"),
                                                     new DataColumn("Warning")
            });


            var es = from Student in this._context.Students.Include(s=>s.Department) select Student;

            foreach (var ems in es)
            {
                dt.Rows.Add(ems.Id, ems.Name,ems.GraduationYear,ems.GraduationGrade,ems.Mobile,
                            ems.Faculty,ems.University,ems.Address,ems.Department.Name,
                            ems.Warning);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students.xlsx");
                }
            }
        }

        private ApplicationUser CreateUser()
        {
            return Activator.CreateInstance<ApplicationUser>();
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}

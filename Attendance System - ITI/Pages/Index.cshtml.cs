using Attendance_System___ITI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Attendance_System___ITI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;


        public IndexModel(ILogger<IndexModel> logger ,ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
            var result = _context.Announcements.OrderByDescending(d=>d.Date).Take(6).ToList();
            ViewData["Anno"] = result;
        }


    }
}
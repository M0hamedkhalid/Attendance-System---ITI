using Attendance_System___ITI.Data;
using Attendance_System___ITI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Attendance_System___ITI.Pages
{
    public class ChatRoomModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public ChatRoomModel(ApplicationDbContext dbContext, UserManager<ApplicationUser> _userManager)
        {
            _dbContext = dbContext;
            userManager = _userManager;
        }
        public List<Message>? LongMessagesList { get; set; }

        public List<Message>? ShortMessagesList { get; set; }
        public string TheSender { get; set; }
        public async Task OnGet()
        {
            var user = userManager.GetUserAsync(HttpContext.User).Result;
            if (user != null)
            {
                await _dbContext.Entry(user).Reference(u => u.Student).LoadAsync();
                await _dbContext.Entry(user).Reference(u => u.Instructor).LoadAsync();
                TheSender = user.Instructor?.Name ?? user.Student?.Name;

            }

            ShortMessagesList = _dbContext.Messages.OrderByDescending(msg => msg.Id).Take(10).ToList();
            LongMessagesList = _dbContext.Messages.OrderByDescending(msg => msg.Id).Take(50).ToList();

        }
    }
}

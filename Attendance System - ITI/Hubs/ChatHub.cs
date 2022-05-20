using Attendance_System___ITI.Data;
using Attendance_System___ITI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Attendance_System___ITI.Hubs
{
    public class ChatHub:Hub
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public ChatHub(ApplicationDbContext dbContext , UserManager<ApplicationUser> _userManager)
        {
            _dbContext = dbContext;
            userManager = _userManager;
        }
        public async Task sendmessage(string msg)
        {
            var user = await userManager.GetUserAsync(Context.User);
            _dbContext.Entry(user).Reference(u => u.Student).Load();
            _dbContext.Entry(user).Reference(u => u.Instructor).Load();

            var sender = user.Instructor?.Name ?? user.Student?.Name;
            //string sender =   Context.User.Identity.Name;
          
            var fullDate = DateTime.Now;
          await  Clients.All.SendAsync("addmsg", msg, sender, fullDate.ToString("MMMM dd"), fullDate.ToString("h:mm tt"));
           
           await Clients.Caller.SendAsync("iSentMessage",msg, sender, fullDate.ToString("MMMM dd"), fullDate.ToString("h:mm tt"));
           await Clients.Others.SendAsync("someoneSentMessage",msg, sender, fullDate.ToString("MMMM dd"), fullDate.ToString("h:mm tt"));
            Message dbMsg = new Message() { Text = msg, Date = fullDate.ToString("MMMM dd"), Sender = sender, ArriveTime = fullDate.ToString("h:mm tt") };
            await _dbContext.Messages.AddAsync(dbMsg);
            _dbContext.SaveChanges();
        }
    }
}

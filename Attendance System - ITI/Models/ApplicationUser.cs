using Microsoft.AspNetCore.Identity;

namespace Attendance_System___ITI.Models
{
    public class ApplicationUser:IdentityUser
    {
        public Student? Student { get; set; }
        public Instructor? Instructor { get; set; }
    }
}

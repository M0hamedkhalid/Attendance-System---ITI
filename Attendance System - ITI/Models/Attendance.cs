using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance_System___ITI.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        [ForeignKey("Student")]
        public string StudentId { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? Date { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan? ArriveTime { get; set; }
        [Column(TypeName = "time")]
        public TimeSpan? LeaveTime { get; set; }

        public Student Student { get; set; }
    }
}

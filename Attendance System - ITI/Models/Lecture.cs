using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance_System___ITI.Models
{
    public class Lecture
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        public string Name { get; set; }


        [Column(TypeName = "datetime2")]
        public DateTime Date { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan StartTime { get; set; }
        [Column(TypeName = "time")]
        public TimeSpan EndTime { get; set; }
        [ForeignKey("Department")]
        public int? DeptID { get; set; }

        public Department? Department { get; set; }
    }
}


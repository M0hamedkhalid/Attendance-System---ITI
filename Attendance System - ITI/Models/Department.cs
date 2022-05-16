using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance_System___ITI.Models
{
    public class Department
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("Manger")]
        public string? MangerId { get; set; }

        public ICollection<Student> Students { get; set; }
        public ICollection<Instructor> Instructors { get; set; }

        public ICollection<Lecture> Lectures { get; set; }
        public Instructor? Manger { get; set; }

        public Department()
        {
            Students = new HashSet<Student>();
            Instructors = new HashSet<Instructor>();
            Lectures=new HashSet<Lecture>();
        }
    }
}

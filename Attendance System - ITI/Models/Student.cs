using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance_System___ITI.Models
{
    public class Student
    {
        [Key, ForeignKey("Credential")]
        public string Id { get; set; }
        public string Name { get; set; }
        public int GraduationYear { get; set; }
        public string? GraduationGrade { get; set; }
        public string? Mobile { get; set; }
        public string? Faculty { get; set; }
        public string? University { get; set; }
        public string? Address { get; set; }
        [ForeignKey("Department")]
        public int? DeptID { get; set; }
        public int StudentStatus { get; set; }
        public int Warning { get; set; }


        public ApplicationUser Credential { get; set; }
        public Department? Department { get; set; }
        public ICollection<Attendance> Attendances { get; set; }


        public Student()
        {
            Attendances = new HashSet<Attendance>();
        }
    }
}

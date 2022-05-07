using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance_System___ITI.Models
{
    public class Instructor
    {
        [Key, ForeignKey("Credential")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        [ForeignKey("Department")]
        public int? DeptID { get; set; }
        public ApplicationUser Credential { get; set; }
        public Department? Department { get; set; }
        public Department? ManagedDepartment  { get; set; }

    }
}

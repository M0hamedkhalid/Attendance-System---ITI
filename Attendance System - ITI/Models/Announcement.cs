using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance_System___ITI.Models
{
    public class Announcement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        public string Descreption { get; set; }

        public String Date { get; set; }   



    }
}


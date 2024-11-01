using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace RestApiCRUD.Models
{
    public class Warrior
    {
        [Key]
        public int WarriorId { get; set; }
        public string FirstName { get; set; }
        public string NickName { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public HashSet<Enrollment> Enrollments { get; set; }
    }
}

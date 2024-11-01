using System.ComponentModel.DataAnnotations;

namespace RestApiCRUD.Models
{
    public class WarriorDto
    {
        [Required(ErrorMessage = "Обязательно для заполнения")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Обязательно для заполнения")]
        public string NickName { get; set; }
        [Required(ErrorMessage = "Обязательно для заполнения")]
        public DateTime EnrollmentDate { get; set; }
    }
}

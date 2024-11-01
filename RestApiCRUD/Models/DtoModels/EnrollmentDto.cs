using System.ComponentModel.DataAnnotations;

namespace RestApiCRUD.Models
{
    public class EnrollmentDto
    {
        [Required(ErrorMessage = "Обязательно для заполнения")]
        public int QuestId { get; set; }
        [Required(ErrorMessage = "Обязательно для заполнения")]
        public int WarriorId { get; set; }
        [Required(ErrorMessage = "Обязательно для заполнения")]
        public Level Level { get; set; }
    }
}

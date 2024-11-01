using System.ComponentModel.DataAnnotations;

namespace RestApiCRUD.Models
{
    public class QuestDto
    {
        [Required(ErrorMessage = "Обязательно для заполнения")]
        public int QuestDtoId { get; set; }
        [Required(ErrorMessage = "Обязательно для заполнения")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Обязательно для заполнения")]
        public int Reward { get; set; }
    }
}

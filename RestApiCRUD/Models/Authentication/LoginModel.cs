using System.ComponentModel.DataAnnotations;

namespace RestApiCRUD.Models.Authentication
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Обязательно для заполнения")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Обязательно для заполнения")]
        public string Password { get; set; }
    }
}

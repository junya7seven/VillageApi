/*using Microsoft.AspNetCore.Mvc;
using RestApiCRUD.Models.Authentication;
namespace RestApiCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AuthorizationController : ControllerBase
    {
        private readonly JwtService _jwtService;
        
        public AuthorizationController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }
        /// <summary>
        ///  Получение JWT токена
        /// </summary>
        /// <param name="login">Авторизация</param>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение получения токена</response>
        /// <response code="401">Не авторизован</response>

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel login)
        {
            if (login.Name == "admin" && login.Password == "root")
            {
                return Ok(new { token = _jwtService.GenerateToken(login) });
            }
            return Unauthorized();
        }
    }
}
*/
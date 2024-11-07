using Microsoft.AspNetCore.Mvc;


namespace RestApiCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AuthorizationController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly IConfiguration _configuration;
        public AuthorizationController(JwtService jwtService, IConfiguration configuration)
        {
            _jwtService = jwtService;
            _configuration = configuration;
        }
        /// <summary>
        ///  Получение JWT токена
        /// </summary>
        /// <param name="login">Авторизация</param>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение получения токена</response>
        /// <response code="401">Не авторизован</response>

        [HttpPost("{login}/{password}")]
        public async Task<IActionResult> Login(string login, string password)
        {
            var configuredLogin = _configuration["AccessData:login"];
            var configuredPassword = _configuration["AccessData:password"];
            if (login == configuredLogin && password == configuredPassword)
            {
                return Ok(new { token = _jwtService.GenerateToken(login, password) });
            }
            return Unauthorized();
        }
    }
}

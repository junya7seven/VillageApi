using Application.Interfaces.JwtInterface;
using Entities.Models.JwtModels;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Application.AuthModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace RestApiCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AuthorizationController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly VillageContext _context;

        public AuthorizationController(IJwtService jwtService, IConfiguration configuration,
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, 
            RoleManager<IdentityRole> roleManager, VillageContext context)
        {
            _jwtService = jwtService;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }
        /// <summary>
        ///  Получение JWT токена
        /// </summary>
        /// <param name="login">Авторизация</param>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение получения токена</response>
        /// <response code="401">Не авторизован</response>

        /*[HttpPost("{login}/{password}")]
        public async Task<IActionResult> Login(string login, string password)
        {
            var configuredLogin = _configuration["AccessData:login"];
            var configuredPassword = _configuration["AccessData:password"];
            if (login == configuredLogin && password == configuredPassword)
            {
                return Ok(new { token = _jwtService.GenerateToken(login, password) });
            }
            return Unauthorized();
        }*/


        [HttpPost("register")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new ApplicationUser
            {

                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded || !await CreateRole(user.Email)) return BadRequest(result.Errors);

            return Ok("User registered successfully");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if(user == null || !await _userManager.CheckPasswordAsync(user,model.Password))
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            };
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var jwtToken = await _jwtService.GenerateTokenAsync(claims);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var tokenEntry = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.Now.AddDays(7),
                CreatedDate = DateTime.Now,
                IsRevoked = false,
            };
            _context.RefreshTokens.Add(tokenEntry);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                Token = jwtToken,
                RefreshToken = refreshToken
            });

        }
        private async Task<bool> CreateRole(string email, string role = "user")
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
            var result = await _userManager.AddToRoleAsync(user, role);
            if(!result.Succeeded) return false;
            return true;
        }
    }
}

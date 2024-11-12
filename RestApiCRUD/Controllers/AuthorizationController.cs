using Application.Interfaces.JwtInterface;
using Entities.Models.JwtModels;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Application.AuthModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;


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

        private readonly VillageContext _context;

        public AuthorizationController(IJwtService jwtService, IConfiguration configuration,
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, VillageContext context)
        {
            _jwtService = jwtService;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }


        [HttpPost("Test")]
        [Authorize]
        public async Task<IActionResult> TestMethod()
        {
            var user = await _userManager.FindByNameAsync("body");
            var result = await _userManager.DeleteAsync(user);
            
            return Ok(result);
        }


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
            if (!result.Succeeded /*|| !await CreateRole(user.Email)*/) return BadRequest(result.Errors);

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


            var jwtToken = await _jwtService.GenerateTokenAsync(user);
            //var jwtToken = await GenerateJwtToken(user);
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
                jwtToken
            });

        }
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var principal = _jwtService.GetClaimsPrincipal(request.AccessToken);
            if (principal == null)
            {
                return Unauthorized("Invalid access token.");
            }
            foreach (var claim in principal.Claims)
            {
                Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
            }
            var userNameClaim = principal.FindFirst(ClaimTypes.Name);
            var userName = userNameClaim.Value;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("User name is missing in the token");
            }
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Unauthorized("User not found");
            }

            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == request.RefreshToken && t.UserId == user.Id);

            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiryDate <= DateTime.Now)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }
           
            var newAccessToken = await _jwtService.GenerateTokenAsync(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            refreshToken.Token = newRefreshToken;
            refreshToken.ExpiryDate = DateTime.Now.AddDays(7);
            refreshToken.CreatedDate = DateTime.Now;
            refreshToken.IsRevoked = false;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("GetRole")]
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> GetRole(string email, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
                return BadRequest("User is not exists");
            if (!await CreateRole(email, role))
                return BadRequest();
            return Ok();
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

        /*public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            // Получаем ключ шифрования из appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Добавляем стандартные claims
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email)
        };

            // Добавляем claims на основе ролей пользователя
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Настройка токена
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }*/



        /*public string GenerateRefreshToken()
        {
            var rndBytes = new byte[64];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(rndBytes);
            }
            return Convert.ToBase64String(rndBytes);
        }*/

        /* public ClaimsPrincipal GetClaimsPrincipal(string token)
         {
             var tokenHandler = new JwtSecurityTokenHandler();

             try
             {
                 var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
                 if (jwtToken == null)
                     return null;

                 var validationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = false,
                     ValidIssuer = _configuration["JwtSettings:Issuer"],
                     ValidAudience = _configuration["JwtSettings:Audience"],
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"])) // Ключ для проверки подписи
                 };

                 var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                 return principal;
             }
             catch
             {
                 return null;
             }
         }*/
    }
}

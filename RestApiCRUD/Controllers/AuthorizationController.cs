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
            UserManager<ApplicationUser> userManager,VillageContext context,
            RoleManager<IdentityRole> roleManager)
        {
            _jwtService = jwtService;
            _configuration = configuration;
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }


        [HttpPost("Test")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> TestMethod()
        {
            User.IsInRole("d2a604ba-40be-4c46-b43e-6d7d67e70aef");

            return Ok("Okay okay");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status code</returns>
        [HttpPost("register")]
        [Authorize(AuthenticationSchemes = "Bearer",Roles = "admin")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if(!ModelState.IsValid && _userManager.FindByEmailAsync(model.Email).Result == null) 
                return BadRequest("Invalid data or email is exist");
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded || !await CreateRole(user.Email)) return BadRequest(result.Errors);

            return Ok();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>accessToken, refreshToken</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
                return Unauthorized();
            if (await _userManager.IsLockedOutAsync(user))
                return Unauthorized($"your account has been blocked {user.LockoutEnd?.LocalDateTime}");
            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                await _userManager.AccessFailedAsync(user);
                if (await _userManager.IsLockedOutAsync(user))
                    return Unauthorized($"your account has been blocked {user.LockoutEnd?.LocalDateTime}");
                return Unauthorized("Invalid password");
            }
            await _userManager.ResetAccessFailedCountAsync(user);
            var jwtToken = await _jwtService.GenerateTokenAsync(user);
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
                accessToken = jwtToken,
                refreshToken = refreshToken
            });
        }
        /// <summary>
        /// Блокировка пользователя
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost("BlockUser/{userName}/{time}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin")]
        public async Task<IActionResult> BlockUser(string userName, int time = 10)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return BadRequest("User not found");
            var role = await _userManager.GetRolesAsync(user);
            if (role.Contains("admin")) return BadRequest("You can't block admin");
            user.LockoutEnd = DateTimeOffset.Now.AddHours(time);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest("user not blocked");
            await RevokeAllTokensUser(user.Id);
            return Ok();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>accessToken, refreshToken</returns>
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var principal = _jwtService.GetClaimsPrincipal(request.AccessToken);
            if (principal == null) return Unauthorized("Invalid access token.");
            var userName = principal.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName))  return Unauthorized("User name is missing in the token");

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return Unauthorized("User not found");

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="role"></param>
        /// <returns>Статус код выдачи роли</returns>
        [HttpPost("AssignRole/{email}/{role}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin")]

        public async Task<IActionResult> AssingRole(string email, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return BadRequest("User is not exists");
            if (!await CreateRole(email, role)) return BadRequest();
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
            if (!result.Succeeded) return false;
            return true;
        }

        [HttpDelete("RevokeAll/{userId}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin")]
        public async Task<IActionResult> RevokeAllTokensUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest("User is not exists");
            
            var userTokens = await _context.RefreshTokens.Where(t => t.UserId == userId).ToListAsync();
            if (!userTokens.Any()) return Ok();
            _context.RefreshTokens.RemoveRange(userTokens);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("RevokeAll")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin")]
        public async Task<IActionResult> RevokeAllTokens()
        {
            var userTokens = await _context.RefreshTokens.ToListAsync();
            if(userTokens.Any()) return Ok();
            _context.RefreshTokens.RemoveRange(userTokens);
            await _context.SaveChangesAsync();
            return Ok();
        }


    }
}

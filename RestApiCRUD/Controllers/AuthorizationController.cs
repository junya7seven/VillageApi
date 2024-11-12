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
            return Ok("Okay okay");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
                LastName = model.LastName
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded || !await CreateRole(user.Email)) return BadRequest(result.Errors);

            return Ok();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password) || user.LockoutEnabled == false)
            {
                return Unauthorized();
            }
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
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost("BlockUser")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin")]
        public async Task<IActionResult> BlockUser(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return BadRequest("User not found");
            user.LockoutEnabled = false;
            return Ok();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
            var userName = principal.FindFirstValue(ClaimTypes.Name);
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost("GetRole")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin")]

        public async Task<IActionResult> AssingRole(string email, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
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
            if (!result.Succeeded) return false;
            return true;
        }


        
    }
}

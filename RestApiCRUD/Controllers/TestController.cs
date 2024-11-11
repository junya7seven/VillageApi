using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace RestApiCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TestController : ControllerBase
    {
        private readonly VillageContext _context;
        private readonly UserManager<VillageContext> _userManager;
        public TestController(VillageContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> SomeMethod(string name)
        {
            var test = await _userManager.FindByIdAsync(name);
            return Ok(test);
        }
    }
}

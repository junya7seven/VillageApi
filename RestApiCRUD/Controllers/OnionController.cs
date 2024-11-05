using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace RestApiCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class OnionController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly VillageContext _context;

        public OnionController(IServiceManager serviceManager,VillageContext context)
        {
            _context = context;
            _serviceManager = serviceManager;
        }

        [HttpDelete("ownerId")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            await _context.Enrollments.FindAsync(id);
            await _serviceManager.EnrollmentService.DeleteAsync(id);
            return Ok();
        }
    }
}

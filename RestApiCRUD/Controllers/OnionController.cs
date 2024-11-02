using Application.Interfaces;
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

        public OnionController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpDelete("{ownerId:int")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            await _serviceManager.EnrollmentService.DeleteAsync(id);
            return Ok();
        }
    }
}

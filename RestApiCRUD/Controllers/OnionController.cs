using Application.Interfaces;
using Entities.Interfaces;
using Entities.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace RestApiCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class OnionController : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;

        public OnionController(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddWarrior(Warrior warrior)
        {
            await _repositoryManager.warriorRepository.Insert(warrior);
            await _repositoryManager.unitOfWork.SaveChangesAsync();
            return Ok(warrior);
        }
    }
}

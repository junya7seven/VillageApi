using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApiCRUD.Database;
using RestApiCRUD.Models;
using System.Threading;

namespace RestApiCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class WarriorController : ControllerBase
    {
        private readonly VillageContext _context;
        public WarriorController(VillageContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Получить воина по id
        /// </summary>
        /// <param name="id">id модели</param>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Данные не заполнены</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Модель не найдена</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("GetWarriorById")]
        public async Task<ActionResult<IEnumerable<Warrior>>> GetWarriorById(int id)
        {
            if (id <= 0)
                return BadRequest("Id cannotr be 0 or negative");
            try
            {
                var warrior = await _context.Warriors.Include(s => s.Enrollments).ThenInclude(e => e.Quest).AsNoTracking().
                    FirstOrDefaultAsync(m => m.WarriorId == id);
                if (warrior == null)
                    return NotFound();
                return Ok(warrior);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        /// <summary>
        /// Получение всех воинов
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("GetAllWarrior")]
        public async Task<ActionResult<IEnumerable<Warrior>>> GetAllWarior()
        {
            try
            {
                var warriors = await _context.Warriors.Include(s => s.Enrollments).ThenInclude(e => e.Quest).AsNoTracking().ToListAsync();
                if (warriors == null)
                    return NotFound();
                return Ok(warriors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Создать воина
        /// </summary>
        /// <param name="warriorDto">Модель</param>
        /// <returns></returns>
        /// <response code="201">Успешное выполнение</response>
        /// <response code="400">Данные не заполнены</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="409">Модель с таким id уже есть</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost("CreateWarrior")]
        public async Task<IActionResult> CreateWarrior([FromBody] WarriorDto warriorDto) // Using DTO to hide a property Enrollment
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var warrior = new Warrior
                {
                    FirstName = warriorDto.FirstName,
                    NickName = warriorDto.NickName,
                    EnrollmentDate = warriorDto.EnrollmentDate
                };
                await _context.AddAsync(warrior);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetWarriorById), new { id = warrior.WarriorId }, warrior);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновление воина
        /// </summary>
        /// <param name="id">id модели</param>
        /// <param name="warriorDto">модель</param>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Данные не заполнены</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Модель не найдена</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPatch("UpdateWarrior")]
        public async Task<ActionResult<Warrior>> UpdateWarrior(int id, WarriorDto warriorDto)
        {
            if (id <= 0)
                return BadRequest("Id cannot be 0 or negative");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var existModel = await _context.Warriors.FindAsync(id);
            if (existModel == null)
            {
                return NotFound($"Warrior with ID {id} does not exist.");
            }
            existModel.FirstName = warriorDto.FirstName;
            existModel.NickName = warriorDto.NickName;
            try
            {
                await _context.SaveChangesAsync();
                return Ok(existModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Удаление квеста
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Данные не заполнены</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Модель не найдена</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpDelete("DeleteWarrior/{ID}")]
        public async Task<IActionResult> DeleteWarrior(int id)
        {
            if (id <= 0)
                return BadRequest("Id cannot be 0 or negative");
            var existModel = await _context.Warriors.FindAsync(id);
            if (existModel == null)
            {
                return NotFound($"Warrior with ID {id} does not exist.");
            }
            try
            {
                var t = _context.Warriors.Remove(existModel);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

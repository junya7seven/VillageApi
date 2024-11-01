using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApiCRUD.Database;
using RestApiCRUD.Models;

namespace RestApiCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class QuestController : ControllerBase
    {
        private readonly VillageContext _context;
        public QuestController(VillageContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Получение квеста по id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("GetQuestById")]
        public async Task<ActionResult<IEnumerable<Quest>>> GetQuestById(int id)
        {
            if (id <= 0)
                return BadRequest("Id cannotr be 0 or negative");
            try
            {
                var quests = await _context.Quests.Include(s => s.Enrollments).ThenInclude(e => e.Warrior).AsNoTracking().
                    FirstOrDefaultAsync(m => m.QuestId == id);
                if (quests == null)
                    return NotFound();
                return Ok(quests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        /// <summary>
        /// Получение всех квестов
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("GetAllQuest")]
        public async Task<ActionResult<IEnumerable<Quest>>> GetAllQuest()
        {
            try
            {
                var quests = await _context.Quests.Include(s => s.Enrollments).ThenInclude(e => e.Warrior).AsNoTracking().ToListAsync();
                if (quests == null)
                    return NotFound();
                return Ok(quests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        /// <summary>
        /// Создание квеста
        /// </summary>
        /// <param name="questDto">Модель</param>
        /// <returns></returns>
        /// <response code="201">Модель создана</response>
        /// <response code="400">Данные не заполнены</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="409">Модель уже существует</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost("CreateQuest")]
        public async Task<IActionResult> CreateQuest([FromBody] QuestDto questDto) // Using DTO to hide a property Enrollment
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await _context.Quests.AnyAsync(q => q.QuestId == questDto.QuestDtoId);
            if (exists)
                return Conflict("The quest already exists");

            try
            {
                var quest = new Quest
                {
                    QuestId = questDto.QuestDtoId,
                    Description = questDto.Description,
                    Reward = questDto.Reward
                };
                await _context.AddAsync(quest);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetQuestById), new { id = quest.QuestId }, quest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        /// <summary>
        /// Обновление квеста
        /// </summary>
        /// <param name="questDto">Модель</param>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Данные не заполнены</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Модель не найдена</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPatch("QuestUpdate")]
        public async Task<ActionResult<Quest>> QuestUpdate(QuestDto questDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var existModel = await _context.Quests.FindAsync(questDto.QuestDtoId);
            if (existModel == null)
            {
                return NotFound($"Quest with ID {questDto.QuestDtoId} does not exist.");
            }

            if(existModel.Description != null) existModel.Description = questDto.Description;
            if(existModel.Reward != null) existModel.Reward = questDto.Reward;
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
        [HttpDelete("QuestDelete")]
        public async Task<IActionResult> QuestDelete(int id)
        {
            if (id <= 0) return BadRequest("Id cannot be 0 or negative");
            var existModel = await _context.Quests.FindAsync(id);
            if (existModel == null)
            {
                return NotFound($"Warrior with ID {id} does not exist.");
            }
            try
            {
                var t = _context.Quests.Remove(existModel);
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

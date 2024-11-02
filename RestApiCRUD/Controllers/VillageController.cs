/*using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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

    public class VillageController : ControllerBase
    {
        private readonly VillageContext _context;
        public VillageController(VillageContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Получить запись по id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Данные не заполнены</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Модель не найдена</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("GetEnrollmentById")]
        public async Task<ActionResult<Enrollment>> GetEnrollmentById(int id)
        {
            if (id <= 0)
                return BadRequest("Id cannot be 0 or negative");
            var existModel = await _context.Enrollments.FirstOrDefaultAsync(x => x.EnrollmentId == id);
            if (existModel == null)
                return NotFound();
            return Ok(existModel);
        }
        /// <summary>
        /// Получение всех записей
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("GetAllEnrollment")]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetAllEnrollment()
        {
            try
            {
                var enrollments = await _context.Enrollments.ToListAsync();
                if(!enrollments.Any())
                    return NotFound();
                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        /// <summary>
        /// Создать запись
        /// </summary>
        /// <param name="enrollmentDto">Модель</param>
        /// <returns></returns>
        /// <response code="201">Успешное выполнение</response>
        /// <response code="400">Данные не заполнены</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="409">Модель с таким id уже есть</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost("CreateEnrollment")]
        public async Task<IActionResult> CreateEnrollment(EnrollmentDto enrollmentDto) // Using DTO to hide a property Quest & Warrior
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!await ExistingModels(enrollmentDto.QuestId, enrollmentDto.WarriorId))
                return Conflict("Quest Id or Warrior Id is not exists");
            try
            {
                var enrollment = new Enrollment
                {
                    QuestId = enrollmentDto.QuestId,
                    WarriorId = enrollmentDto.WarriorId,
                    Level = enrollmentDto.Level
                };
                await _context.AddAsync(enrollment);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetEnrollmentById), new { id = enrollment.EnrollmentId}, enrollment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        /// <summary>
        /// Обновление записи
        /// </summary>
        /// <param name="enrollment">Модель</param>
        /// <param name="id">id модели</param>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Данные не заполнены</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Модели не найдены</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPatch("UpdateEnrollment")]
        public async Task<ActionResult<Enrollment>> UpdateEnrollment(int id, EnrollmentDto enrollmentDto)
        {
            if (id <= 0)
                return BadRequest("Id cannot be 0 or negative");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var existModel = await _context.Enrollments.FindAsync(id);
            if (existModel == null)
                return NotFound("Enrollment Id is not exists");
            if (!await ExistingModels(enrollmentDto.QuestId, enrollmentDto.WarriorId))
                return NotFound("Quest Id or Warrior Id is not exists");

            existModel.QuestId = enrollmentDto.QuestId;
            existModel.WarriorId = enrollmentDto.WarriorId;
            existModel.Level = enrollmentDto.Level;

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
        /// Удаление записи
        /// </summary>
        /// <param name="id">id модели</param>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Данные не заполнены</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Модели не найдены</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpDelete("DeleteEnrollment")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            if (id <= 0)
                return BadRequest("Id cannot be 0 or negative");
            var existModel = await _context.Enrollments.FindAsync(id);
            if (existModel == null)
                return NotFound("Enrollment Id is not exists");
            try
            {
                _context.Enrollments.Remove(existModel);
                await _context.SaveChangesAsync();
                return Ok(existModel);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");

            }
        }


        // Checking existing IDs for added Enrlloment
        private async Task<bool> ExistingModels(int questId, int warriorId) 
        {
            var quest = await _context.Quests.AnyAsync(q => q.QuestId == questId);
            var warrior = await _context.Warriors.AnyAsync(w => w.WarriorId == warriorId);
            if (quest && warrior)
                return true;
            return false;
        }
    }
}
*/
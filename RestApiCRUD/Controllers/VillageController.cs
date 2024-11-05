using Application.DTOs;
using Application.Interfaces;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RestApiCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    //[Authorize]

    public class VillageController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public VillageController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
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
        [HttpGet("GetEnrollmentById")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var enrollment = await _serviceManager.EnrollmentService.GetByIdAsync(id);
                if (enrollment == null)
                    return NotFound();
                return Ok(enrollment);
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
        [HttpGet("GetAllEnrollment")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var enrollments = await _serviceManager.EnrollmentService.GetAllAsync();
                if (enrollments == null)
                    return NotFound();
                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        /// <summary>
        /// Создание квеста
        /// </summary>
        /// <param name="enrollmentDto">Модель</param>
        /// <returns></returns>
        /// <response code="201">Модель создана</response>
        /// <response code="400">Данные не заполнены</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="409">Модель уже существует</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost("CreateEnrollment")]
        public async Task<IActionResult> Create([FromBody] EnrollmentDTO enrollmentDto) // Using DTO to hide a property Enrollment
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var exists = await _serviceManager.EnrollmentService.CreateAsync(enrollmentDto);
                if (exists == null)
                    return Conflict("The warriorId or questId not aexists");

                return Ok();
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
        [HttpPatch("EnrollmentUpdate")]
        public async Task<IActionResult> Update(int id, EnrollmentDTO enrollmentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _serviceManager.EnrollmentService.UpdateAsync(id, enrollmentDto);
            return Ok();

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
        [HttpDelete("EnrollmentDelete")]
        public async Task<IActionResult> Delete(int id)
        {
            await _serviceManager.EnrollmentService.DeleteAsync(id);
            return Ok();
        }
    }
}

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
    [Authorize]

    public class VillageController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public VillageController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        /// <summary>
        /// Получение записи по id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest("Invalid ID.");
            try
            {
                var enrollment = await _serviceManager.EnrollmentService.GetByIdAsync(id);
                return Ok(enrollment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        /// <summary>
        /// Получение всех записей
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var enrollments = await _serviceManager.EnrollmentService.GetAllAsync();
                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        /// <summary>
        /// Создание записи
        /// </summary>
        /// <param name="enrollmentDto">Модель</param>
        /// <returns></returns>
        /// <response code="201">Модель создана</response>
        /// <response code="400">Данные не заполнены</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="409">Модель уже существует</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EnrollmentDTO enrollmentDto) // Using DTO to hide a property Enrollment
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var exists = await _serviceManager.EnrollmentService.CreateAsync(enrollmentDto);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
        /// <summary>
        /// Обновление записи
        /// </summary>
        /// <param name="enrollmentDto">Модель</param>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Данные не заполнены</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Модель не найдена</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, EnrollmentDTO enrollmentDto)
        {
            if (id <= 0) return BadRequest("Invalid ID.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await _serviceManager.EnrollmentService.UpdateAsync(id, enrollmentDto);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
        /// <summary>
        /// Удаление записи
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Данные не заполнены</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Модель не найдена</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _serviceManager.EnrollmentService.DeleteAsync(id);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

﻿using Application.DTOs;
using Application.Interfaces;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RestApiCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    //[Authorize]
    public class WarriorController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public WarriorController(IServiceManager serviceManager)
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
        [HttpGet("GetWarriorById")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var warrior = await _serviceManager.WarriorService.GetByIdAsync(id);
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
        /// Получение всех квестов
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("GetAllWarriors")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var warriors = await _serviceManager.WarriorService.GetAllAsync();
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
        /// Создание квеста
        /// </summary>
        /// <param name="questDto">Модель</param>
        /// <returns></returns>
        /// <response code="201">Модель создана</response>
        /// <response code="400">Данные не заполнены</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="409">Модель уже существует</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost("CreateWarrior")]
        public async Task<IActionResult> Create([FromBody] WarriorDTO warriotDto) // Using DTO to hide a property Enrollment
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var exists = await _serviceManager.WarriorService.CreateAsync(warriotDto);
                if (exists == null)
                    return Conflict("The warrior already exists");

                return CreatedAtAction(nameof(GetById), new { id = exists.FirstName }, exists);
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
        [HttpPatch("WarriorUpdate")]
        public async Task<IActionResult> Update(int id,WarriorDTO warriorDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _serviceManager.WarriorService.UpdateAsync(id, warriorDto);
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
        [HttpDelete("WarriorDelete")]
        public async Task<IActionResult> Delete(int id)
        {
            await _serviceManager.WarriorService.DeleteAsync(id);
            return Ok();
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TastifyAPI.DTOs;
using TastifyAPI.Entities;
using TastifyAPI.Helpers;
using TastifyAPI.Services;

namespace TastifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly ScheduleService _scheduleService;
        private readonly ILogger<ScheduleController> _logger;
        private readonly IMapper _mapper;

        public ScheduleController(
            ScheduleService scheduleService, 
            ILogger<ScheduleController> logger, 
            IMapper mapper)
        {
            _scheduleService = scheduleService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all schedules.
        /// </summary>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a list of ScheduleDto.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A list of ScheduleDto.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpGet]
        public async Task<ActionResult<List<ScheduleDto>>> GetAllSchedules()
        {
            try
            {
                var schedules = await _scheduleService.GetAsync();
                var scheduleDtos = _mapper.Map<List<ScheduleDto>>(schedules);
                return Ok(scheduleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all schedules");
                return StatusCode(500, "Failed to get all schedules");
            }
        }

        /// <summary>
        /// Get a schedule by ID.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule.</param>
        /// <remarks>
        /// This endpoint requires Worker or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a ScheduleDto.
        /// If the schedule is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A ScheduleDto.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpGet("{scheduleId:length(24)}")]
        public async Task<ActionResult<ScheduleDto>> GetScheduleById(string scheduleId)
        {
            try
            {
                var schedule = await _scheduleService.GetByIdAsync(scheduleId);
                if (schedule == null)
                    return NotFound();

                var scheduleDto = _mapper.Map<ScheduleDto>(schedule);
                return Ok(scheduleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get schedule with ID {0}", scheduleId);
                return StatusCode(500, $"Failed to get schedule with ID {scheduleId}");
            }
        }

        /// <summary>
        /// Create a new schedule.
        /// </summary>
        /// <param name="scheduleDto">The schedule data.</param>
        /// <remarks>
        /// This endpoint requires Worker or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 201 Created containing the created ScheduleDto.
        /// If the ModelState is invalid, it will return a BadRequest response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// The created ScheduleDto.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpPost]
        public async Task<ActionResult<ScheduleDto>> CreateSchedule(ScheduleDto scheduleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var schedule = _mapper.Map<Schedule>(scheduleDto);
                await _scheduleService.CreateAsync(schedule);

                var createdScheduleDto = _mapper.Map<ScheduleDto>(schedule);
                return CreatedAtAction(nameof(GetScheduleById), new { scheduleId = createdScheduleDto.Id }, createdScheduleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new schedule");
                return StatusCode(500, "Failed to create new schedule");
            }
        }

        /// <summary>
        /// Delete a schedule by ID.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule to delete.</param>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK with a success message.
        /// If the schedule is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// HTTP 200 OK with a success message.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpDelete("{scheduleId:length(24)}")]
        public async Task<IActionResult> DeleteSchedule(string scheduleId)
        {
            try
            {
                var schedule = await _scheduleService.GetByIdAsync(scheduleId);
                if (schedule == null)
                    return NotFound($"Schedule with ID {scheduleId} not found");

                await _scheduleService.RemoveAsync(scheduleId);

                return Ok("Schedule was deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete schedule with ID {0}", scheduleId);
                return StatusCode(500, "Failed to delete schedule");
            }
        }

        /// <summary>
        /// Update an existing schedule.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule to update.</param>
        /// <param name="scheduleDto">The updated schedule data.</param>
        /// <remarks>
        /// This endpoint requires Worker or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a success message.
        /// If the schedule is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// HTTP 200 OK with a success message.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpPut("{scheduleId:length(24)}")]
        public async Task<IActionResult> UpdateSchedule(string scheduleId, ScheduleDto scheduleDto)
        {
            try
            {
                var existingSchedule = await _scheduleService.GetByIdAsync(scheduleId);
                if (existingSchedule == null)
                    return NotFound();

                scheduleDto.Id = scheduleId;
                _mapper.Map(scheduleDto, existingSchedule);

                await _scheduleService.UpdateAsync(scheduleId, existingSchedule);

                return Ok("Schedule was updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update schedule with ID {0}", scheduleId);
                return StatusCode(500, $"Failed to update schedule with ID {scheduleId}");
            }
        }

        /// <summary>
        /// Get schedule by staff ID.
        /// </summary>
        /// <param name="staffId">The ID of the staff.</param>
        /// <remarks>
        /// This endpoint requires Worker or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a ScheduleDto.
        /// If the schedule is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A ScheduleDto.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpGet("staff/{staffId:length(24)}")]
        public async Task<ActionResult<List<ScheduleDto>>> GetScheduleByStaff(string staffId)
        {
            try
            {
                var schedules = await _scheduleService.GetByStaffAsync(staffId);
                if (schedules == null || schedules.Count == 0)
                    return NotFound();

                var scheduleDtos = _mapper.Map<List<ScheduleDto>>(schedules);
                return Ok(scheduleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get schedule with ID {0}", staffId);
                return StatusCode(500, $"Failed to get schedule with ID {staffId}");
            }
        }
    }
}

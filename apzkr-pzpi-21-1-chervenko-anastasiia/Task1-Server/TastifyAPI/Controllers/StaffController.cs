using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TastifyAPI.DTOs;
using TastifyAPI.Entities;
using TastifyAPI.Services;
using TastifyAPI.Helpers;
using TastifyAPI.DTOs.Features_DTOs;
using TastifyAPI.Services.JwtTokenService;

namespace TastifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly StaffService _staffService;
        private readonly ILogger<StaffController> _logger;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Staff> _passwordHasher;
        private readonly JwtService _jwtService;

        public StaffController(
            StaffService staffService,
            ILogger<StaffController> logger,
            IMapper mapper,
            IPasswordHasher<Staff> passwordHasher,
            IConfiguration config)
        {
            _staffService = staffService;
            _logger = logger;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _jwtService = new JwtService(config);
        }

        /// <summary>
        /// Get all staff members.
        /// </summary>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a list of StaffDto.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A list of StaffDto.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpGet]
        public async Task<ActionResult<List<StaffDto>>> GetAllStaff()
        {
            try
            {
                var staffList = await _staffService.GetAsync();
                var staffDtoList = _mapper.Map<List<StaffDto>>(staffList);
                return Ok(staffDtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all staff");
                return StatusCode(500, "Failed to get all staff");
            }
        }

        /// <summary>
        /// Get a staff member by ID.
        /// </summary>
        /// <param name="staffId">The ID of the staff member.</param>
        /// <remarks>
        /// This endpoint requires Worker or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a StaffDto.
        /// If the staff member is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A StaffDto.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpGet("{staffId:length(24)}")]
        public async Task<ActionResult<StaffDto>> GetStaffById(string staffId)
        {
            try
            {
                var staff = await _staffService.GetByIdAsync(staffId);
                if (staff == null)
                    return NotFound();

                var staffDto = _mapper.Map<StaffDto>(staff);
                return Ok(staffDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get staff with ID {0}", staffId);
                return StatusCode(500, $"Failed to get staff with ID {staffId}");
            }
        }

        /// <summary>
        /// Get weekly working hours of staff members.
        /// </summary>
        /// <param name="date">The date for which to get the weekly working hours.</param>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a list of StaffReportDto.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A list of StaffReportDto.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpGet("weekly-working-hours")]
        public async Task<ActionResult<List<StaffReportDto>>> GetWeeklyWorkingHours(DateTime date)
        {
            try
            {
                var weeklyWorkingHours = await _staffService.GetWeeklyWorkingHoursAsync(date);

                return Ok(weeklyWorkingHours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get weekly working hours");
                return StatusCode(500, "Failed to get weekly working hours");
            }
        }

        /// <summary>
        /// Register a new staff member.
        /// </summary>
        /// <param name="staffRegistrationDto">The staff registration data.</param>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a JWT token.
        /// If the ModelState is invalid or staff with the same login already exists, it will return a BadRequest response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A JWT token.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpPost("register")]
        public async Task<IActionResult> StaffRegister(StaffRegistrationDto staffRegistrationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (await _staffService.AnyAsync(s => s.Login == staffRegistrationDto.Login))
                    return BadRequest("Staff with such login already exists");

                var newStaff = _mapper.Map<Staff>(staffRegistrationDto);
                newStaff.Password = _passwordHasher.HashPassword(newStaff, staffRegistrationDto.Password);

                await _staffService.CreateAsync(newStaff);

                var token = _jwtService.GenerateStaffToken(newStaff);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during staff registration");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Log in a staff member.
        /// </summary>
        /// <param name="staffLoginDto">The staff login data.</param>
        /// <remarks>
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a JWT token.
        /// If the ModelState is invalid, staff does not exist, or the password is incorrect, it will return a BadRequest response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A JWT token.
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> StaffLogin(StaffLoginDto staffLoginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var staff = await _staffService.GetByLoginAsync(staffLoginDto.Login);

                if (staff == null)
                {
                    return BadRequest("Staff with such login does not exist");
                }

                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(staff, staff.Password, staffLoginDto.Password);

                if (passwordVerificationResult != PasswordVerificationResult.Success)
                {
                    return BadRequest("Invalid login or password");
                }

                var token = _jwtService.GenerateStaffToken(staff);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Update staff member information by ID.
        /// </summary>
        /// <param name="staffId">The ID of the staff member to update.</param>
        /// <param name="staffDto">The updated staff data.</param>
        /// <remarks>
        /// This endpoint requires Worker or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a success message.
        /// If the staff member is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A success message.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpPut("{staffId:length(24)}")]
        public async Task<IActionResult> UpdateStaff(string staffId, StaffDto staffDto)
        {
            try
            {
                var existingStaff = await _staffService.GetByIdAsync(staffId);
                if (existingStaff == null)
                    return NotFound();

                staffDto.Id = staffId;
                _mapper.Map(staffDto, existingStaff);

                await _staffService.UpdateAsync(staffId, existingStaff);

                return Ok("Staff info was updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update staff with ID {0}", staffId);
                return StatusCode(500, $"Failed to update staff with ID {staffId}");
            }
        }

        /// <summary>
        /// Delete a staff member by ID.
        /// </summary>
        /// <param name="staffId">The ID of the staff member to delete.</param>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a success message.
        /// If the staff member is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A success message.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpDelete("{staffId:length(24)}")]
        public async Task<IActionResult> DeleteStaff(string staffId)
        {
            try
            {
                var staff = await _staffService.GetByIdAsync(staffId);
                if (staff == null)
                    return NotFound();

                await _staffService.RemoveAsync(staffId);

                return Ok("Staff was deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete staff with ID {0}", staffId);
                return StatusCode(500, $"Failed to delete staff with ID {staffId}");
            }
        }
    }
}

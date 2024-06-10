using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TastifyAPI.DTOs;
using TastifyAPI.DTOs.Features_DTOs;
using TastifyAPI.Entities;
using TastifyAPI.Helpers;
using TastifyAPI.Services;
using TastifyAPI.Services.JwtTokenService;

namespace TastifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuestController : ControllerBase
    {
        private readonly GuestService _guestService;
        private readonly JwtService _jwtService;
        private readonly ILogger<GuestController> _logger;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Guest> _passwordHasher;

        public GuestController(
            GuestService guestService,
            ILogger<GuestController> logger,
            IPasswordHasher<Guest> passwordHasher,
            IMapper mapper,
            IConfiguration config)
        {
            _guestService = guestService;
            _logger = logger;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _jwtService = new JwtService(config);
        }

        /// <summary>
        /// Get all guests.
        /// </summary>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a list of GuestDto.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with error message.
        /// </remarks>
        /// <returns>
        /// A list of GuestDto.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpGet]
        public async Task<ActionResult<List<GuestDto>>> GetAllGuests()
        {
            try
            {
                var Guests = await _guestService.GetAsync();
                var GuestDtos = _mapper.Map<List<GuestDto>>(Guests);
                return Ok(GuestDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all Guests");
                return StatusCode(500, "Failed to get all Guests");
            }
        }

        /// <summary>
        /// Get guest by ID.
        /// </summary>
        /// <param name="guestId">The ID of the guest.</param>
        /// <remarks>
        /// This endpoint requires Guest or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a GuestDto.
        /// If the guest is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A GuestDto.
        /// </returns>
        [Authorize(Roles = Roles.Guest + "," + Roles.Administrator)]
        [HttpGet("{guestId:length(24)}")]
        public async Task<ActionResult<GuestDto>> GetGuestById(string guestId)
        {
            try
            {
                var Guest = await _guestService.GetByIdAsync(guestId);
                if (Guest == null)
                    return NotFound();

                var GuestDto = _mapper.Map<GuestDto>(Guest);
                return Ok(GuestDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Guest with ID {0}", guestId);
                return StatusCode(500, $"Failed to get Guest with ID {guestId}");
            }
        }

        /// <summary>
        /// Get all guests sorted by bonus and name.
        /// </summary>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a list of GuestDto.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with error message.
        /// </remarks>
        /// <returns>
        /// A list of GuestDto sorted by name and bonus.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpGet("sorted-by-name-and-bonus")]
        public async Task<ActionResult<List<GuestDto>>> GetGuestsSortedByNameAndBonus()
        {
            try
            {
                var guests = await _guestService.GetSortedByBonusAndNameAsync();
                var guestDtos = _mapper.Map<List<GuestDto>>(guests);
                return Ok(guestDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get guests sorted by name and bonus");
                return StatusCode(500, "Failed to get guests sorted by name and bonus");
            }
        }

        /// <summary>
        /// Make a coupon for a guest based on bonus points.
        /// </summary>
        /// <param name="bonus">The bonus points.</param>
        /// <remarks>
        /// This endpoint requires Guest role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a CouponDto.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with error message.
        /// </remarks>
        /// <returns>
        /// A CouponDto whith sale and bonuses remaining.
        /// </returns>
        [Authorize(Roles = Roles.Guest)]
        [HttpPost("make-coupon")]
        public async Task<ActionResult<CouponDto>> MakeCoupon(int bonus)
        {
            try
            {
                var (discount, remainingBonus) = await _guestService.CalculateCouponAsync(bonus);
                var couponDto = new CouponDto { Discount = discount, Bonus = remainingBonus };
                return Ok(couponDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to make coupon for bonus {0}", bonus);
                return StatusCode(500, $"Failed to make coupon for bonus {bonus}");
            }
        }

        /// <summary>
        /// Register a new guest.
        /// </summary>
        /// <param name="guestRegistrationDto">The guest registration data.</param>
        /// <remarks>
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a JWT token.
        /// If the guest already exists, it will return a BadRequest response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with error message.
        /// </remarks>
        /// <returns>
        /// A JWT token.
        /// </returns>
        [HttpPost("register")]
        public async Task<ActionResult> Register(GuestRegistrationDto guestRegistrationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (await _guestService.AnyAsync(g => g.Email == guestRegistrationDto.Email))
                    return BadRequest("Guest with such email already exists");

                var newGuest = _mapper.Map<Guest>(guestRegistrationDto);

                newGuest.Bonus = 0;
                newGuest.Password = _passwordHasher.HashPassword(newGuest, guestRegistrationDto.Password);

                await _guestService.CreateAsync(newGuest);

                var token = _jwtService.GenerateGuestToken(newGuest);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during guest registration");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Login as a guest.
        /// </summary>
        /// <param name="guestLoginDto">The guest login data.</param>
        /// <remarks>
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a JWT token.
        /// If the login is invalid, it will return a BadRequest response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with error message.
        /// </remarks>
        /// <returns>
        /// A JWT token.
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(GuestLoginDto guestLoginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var guest = await _guestService.GetByLoginAsync(guestLoginDto.Login);

                if (guest == null)
                    return BadRequest("Guest with such login does not exists");

                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(guest, 
                    guest.Password, guestLoginDto.Password);

                if (passwordVerificationResult != PasswordVerificationResult.Success)
                {
                    return BadRequest("Invalid login or password");
                }

                var token = _jwtService.GenerateGuestToken(guest);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Update a guest's information.
        /// </summary>
        /// <param name="guestId">The ID of the guest.</param>
        /// <param name="guestDto">The updated guest data.</param>
        /// <remarks>
        /// This endpoint requires Guest or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a success message.
        /// If the guest is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with error message.
        /// </remarks>
        /// <returns>
        /// HTTP 200 OK with success message.
        /// </returns>
        [Authorize(Roles = Roles.Guest + "," + Roles.Administrator)]
        [HttpPut("{guestId:length(24)}")]
        public async Task<IActionResult> UpdateGuest(string guestId, GuestDto guestDto)
        {
            try
            {
                var existingGuest = await _guestService.GetByIdAsync(guestId);
                if (existingGuest == null)
                    return NotFound();

                guestDto.Id = guestId;
                _mapper.Map(guestDto, existingGuest);

                guestDto.Password = _passwordHasher.HashPassword(existingGuest, guestDto.Password);

                await _guestService.UpdateAsync(guestId, existingGuest);

                return Ok("Guest updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update Guest with ID {0}", guestId);
                return StatusCode(500, $"Failed to update Guest with ID {guestId}");
            }
        }
        /// <summary>
        /// Delete a guest.
        /// </summary>
        /// <param name="guestId">The ID of the guest.</param>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK with a success message.
        /// If the guest is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with error message.
        /// </remarks>
        /// <returns>
        /// HTTP 200 OK with a success message.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpDelete("{guestId:length(24)}")]
        public async Task<IActionResult> DeleteGuest(string guestId)
        {
            try
            {
                var Guest = await _guestService.GetByIdAsync(guestId);
                if (Guest == null)
                    return NotFound();

                await _guestService.RemoveAsync(guestId);

                return Ok("Guest deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete Guest with ID {0}", guestId);
                return StatusCode(500, $"Failed to delete Guest with ID {guestId}");
            }
        }
    }
}

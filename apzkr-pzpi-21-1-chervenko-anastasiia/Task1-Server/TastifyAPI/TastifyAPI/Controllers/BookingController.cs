using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TastifyAPI.DTOs;
using TastifyAPI.Entities;
using TastifyAPI.Services;
using TastifyAPI.Helpers;

namespace TastifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;
        private readonly ILogger<BookingController> _logger;
        private readonly IMapper _mapper;

        public BookingController(
            BookingService bookingService, 
            ILogger<BookingController> logger, 
            IMapper mapper)
        {
            _bookingService = bookingService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get a list of bookings.
        /// </summary>
        /// <remarks>
        /// This endpoint requires Worker or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a list of BookingDto.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with error message.
        /// </remarks>
        /// <returns>
        /// A list of BookingDto.
        /// </returns>   
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpGet]
        public async Task<ActionResult<List<BookingDto>>> GetAllBookings()
        {
            try
            {
                var bookings = await _bookingService.GetAsync();
                var bookingDtos = _mapper.Map<List<BookingDto>>(bookings);
                return Ok(bookingDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all bookings");
                return StatusCode(500, "Failed to get all bookings");
            }
        }

        /// <summary>
        /// Get a booking by its ID.
        /// </summary>
        /// <param name="bookingId">The ID of the booking.</param>
        /// <remarks>
        /// This endpoint requires Worker, Guest, or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a BookingDto.
        /// If the booking is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A bookingDto.
        /// </returns>    
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator + "," + Roles.Guest)]
        [HttpGet("{bookingId:length(24)}")]
        public async Task<ActionResult<BookingDto>> GetBookingById(string bookingId)
        {
            try
            {
                var booking = await _bookingService.GetByIdAsync(bookingId);
                if (booking == null)
                    return NotFound();
 
                var bookingDto = _mapper.Map<BookingDto>(booking);
                return Ok(bookingDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get booking with ID {0}", bookingId);
                return StatusCode(500, $"Failed to get booking with ID {bookingId}");
            }
        }

        /// <summary>
        /// Get all bookings of the guest by his ID.
        /// </summary>
        /// <param name="guestId">The ID of the guest.</param>
        /// <remarks>
        /// This endpoint requires Worker, Guest or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a list of BookingDto.
        /// If the bookings are not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with error message.
        /// </remarks>
        /// <returns>
        /// A list of BookingDto for specified guest.
        /// </returns>     
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator + "," + Roles.Guest)]
        [HttpGet("guest-bookings/{guestId:length(24)}")]
        public async Task<ActionResult<List<BookingDto>>> GetAllGuestBookings(string guestId)
        {
            try
            {
                var bookings = await _bookingService.GetAllBookingsAsync(guestId);
                var bookingDtos = _mapper.Map<List<BookingDto>>(bookings);
                return Ok(bookingDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all bookings for guest with ID {0}", guestId);
                return StatusCode(500, $"Failed to get all bookings for guest with ID {guestId}");
            }
        }

        /// <summary>
        /// Get all bookings by specified date.
        /// </summary>
        /// <param name="date">The date of bookings.</param>
        /// <remarks>
        /// This endpoint requires Worker, Guest or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a list of BookingDto.
        /// If the bookings are not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with error message.
        /// </remarks>
        /// <returns>
        /// A List of BookingDto for the specified date.
        /// </returns> 
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator + "," + Roles.Guest)]
        [HttpGet("bookins-by-date")]
        public async Task<ActionResult<BookingDto>> GetBookingsByDate(DateTime date)
        {
            try
            {
                var booking = await _bookingService.GetByDateAsync(date);
                if (booking == null)
                    return NotFound($"Booking for date {date} not found");

                var bookingDto = _mapper.Map<BookingDto>(booking);
                return Ok(bookingDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get bookings for date {0}", date);
                return StatusCode(500, $"Failed to get bookings for date {date}");
            }
        }

        /// <summary>
        /// Get all bookings by specified date sorted in descending order by time.
        /// </summary>
        /// <param name="date">The date of bookings.</param>
        /// <remarks>
        /// This endpoint requires Worker or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a list of BookingDto.
        /// If the bookings are not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with error message.
        /// </remarks>
        /// <returns>
        /// A list of BookingDto sorted in descending order by time.
        /// </returns> 
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpGet("sorted-bookings-by-date")]
        public async Task<ActionResult<List<BookingDto>>> GetSortedBookingsByDate(DateTime date)
        {
            try
            {
                var sortedBookings = await _bookingService.GetSortedByDateAsync(date);
                var bookingDtos = _mapper.Map<List<BookingDto>>(sortedBookings);
                return Ok(bookingDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get sorted bookings by date");
                return StatusCode(500, "Failed to get sorted bookings by date");
            }
        }

        /// <summary>
        /// Create new booking.
        /// </summary>
        /// <param name="bookingDto">The DTO of bookings.</param>
        /// <remarks>
        /// This endpoint requires Worker, Guest or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a BookingDto.
        /// If the bookings are not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with error message.
        /// </remarks>
        /// <returns>
        /// A BookingDto of new booking.
        /// </returns> 
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator + "," + Roles.Guest)]
        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking(BookingDto bookingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var booking = _mapper.Map<Booking>(bookingDto);

                await _bookingService.CreateAsync(booking);

                var createdBookingDto = _mapper.Map<BookingDto>(booking);

                return Ok("Booking created successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new booking");
                return StatusCode(500, "Failed to create new booking");
            }
        }

        /// <summary>
        /// Update info about exiting booking.
        /// </summary>
        /// <param name="bookingDto">The DTO of bookings.</param>
        /// <remarks>
        /// This endpoint requires Worker, Guest or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK with ssuccess message.
        /// If the bookings are not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with error message.
        /// </remarks>
        /// <returns>
        /// HTTP 200 OK with ssuccess message.
        /// </returns> 
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator + "," + Roles.Guest)]
        [HttpPut("{bookingId:length(24)}")]
        public async Task<IActionResult> UpdateBooking(string bookingId, BookingDto bookingDto)
        {
            try
            {
                var existingBooking = await _bookingService.GetByIdAsync(bookingId);
                if (existingBooking == null)
                    return NotFound();

                bookingDto.Id = bookingId;
                _mapper.Map(bookingDto, existingBooking);

                await _bookingService.UpdateAsync(bookingId, existingBooking);

                return Ok("Booking updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update booking with ID {0}", bookingId);
                return StatusCode(500, $"Failed to update booking with ID {bookingId}");
            }
        }

        /// <summary>
        /// Delete exiting booking.
        /// </summary>
        /// <param name="bookingId">The booking ID.</param>
        /// <remarks>
        /// This endpoint requires Worker, Guest or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK with ssuccess message.
        /// If the bookings are not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with error message.
        /// </remarks>
        /// <returns>
        /// HTTP 200 OK with ssuccess message.
        /// </returns> 
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator + "," + Roles.Guest)]
        [HttpDelete("{bookingId:length(24)}")]
        public async Task<IActionResult> DeleteBooking(string bookingId)
        {
            try
            {
                var booking = await _bookingService.GetByIdAsync(bookingId);
                if (booking == null)
                    return NotFound($"Booking with ID {bookingId} not found");

                await _bookingService.DeleteAsync(bookingId);

                return Ok("Booking deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete booking with ID {0}", bookingId);
                return StatusCode(500, "Failed to delete booking");
            }
        }  
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TastifyAPI.DTOs;
using TastifyAPI.Entities;
using TastifyAPI.Helpers;
using TastifyAPI.Services;

namespace TastifyAPI.Controllers
{
    [ApiController]
    [Authorize(Roles = Roles.Administrator)]
    [Route("api/[controller]")]
    public class RestaurantsController : ControllerBase
    {
        private readonly RestaurantService _restaurantsService;
        private readonly ILogger<RestaurantsController> _logger;
        private readonly IMapper _mapper;

        public RestaurantsController(
            RestaurantService restaurantsService, 
            ILogger<RestaurantsController> logger, 
            IMapper mapper)
        {
            _restaurantsService = restaurantsService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all restaurants.
        /// </summary>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a list of RestaurantDto.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A list of RestaurantDto.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<List<RestaurantDto>>> GetAllRestaurants()
        {
            try
            {
                var restaurants = await _restaurantsService.GetAsync();
                var restaurantDtos = _mapper.Map<List<RestaurantDto>>(restaurants);
                return Ok(restaurantDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all restaurants");
                return StatusCode(500, "Failed to get all restaurants");
            }
        }

        /// <summary>
        /// Get a restaurant by ID.
        /// </summary>
        /// <param name="restaurantId">The ID of the restaurant.</param>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a RestaurantDto.
        /// If the restaurant is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A RestaurantDto.
        /// </returns>
        [HttpGet("{restaurantId:length(24)}")]
        public async Task<ActionResult<RestaurantDto>> GetRestaurantById(string restaurantId)
        {
            try
            {
                var restaurant = await _restaurantsService.GetByIdAsync(restaurantId);
                if (restaurant == null)
                    return NotFound();

                var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);
                return Ok(restaurantDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get restaurant with ID {0}", restaurantId);
                return StatusCode(500, $"Failed to get restaurant with ID {restaurantId}");
            }
        }

        /// <summary>
        /// Create a new restaurant.
        /// </summary>
        /// <param name="restaurantDto">The restaurant data.</param>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 201 Created containing the created RestaurantDto.
        /// If the ModelState is invalid, it will return a BadRequest response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// The created RestaurantDto.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<RestaurantDto>> CreateRestaurant(RestaurantDto restaurantDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var restaurant = _mapper.Map<Restaurant>(restaurantDto);
                await _restaurantsService.CreateAsync(restaurant);

                var createdRestaurantDto = _mapper.Map<RestaurantDto>(restaurant);
                return CreatedAtAction(nameof(GetRestaurantById), 
                    new { restaurantId = createdRestaurantDto.Id }, createdRestaurantDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new restaurant");
                return StatusCode(500, "Failed to create new restaurant");
            }
        }

        /// <summary>
        /// Update an existing restaurant.
        /// </summary>
        /// <param name="restaurantId">The ID of the restaurant to update.</param>
        /// <param name="restaurantDto">The updated restaurant data.</param>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a success message.
        /// If the restaurant is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// HTTP 200 OK with success message.
        /// </returns>
        [HttpPut("{restaurantId:length(24)}")]
        public async Task<IActionResult> UpdateRestaurant(string restaurantId, RestaurantDto restaurantDto)
        {
            try
            {
                var existingRestaurant = await _restaurantsService.GetByIdAsync(restaurantId);
                if (existingRestaurant == null)
                    return NotFound();

                restaurantDto.Id = restaurantId;
                _mapper.Map(restaurantDto, existingRestaurant);

                await _restaurantsService.UpdateAsync(restaurantId, existingRestaurant);

                return Ok("Restaurant was updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update restaurant with ID {0}", restaurantId);
                return StatusCode(500, $"Failed to update restaurant with ID {restaurantId}");
            }
        }

        /// <summary>
        /// Delete a restaurant.
        /// </summary>
        /// <param name="restaurantId">The ID of the restaurant to delete.</param>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK with a success message.
        /// If the restaurant is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// HTTP 200 OK with a success message.
        /// </returns>
        [HttpDelete("{restaurantId:length(24)}")]
        public async Task<IActionResult> DeleteRestaurant(string restaurantId)
        {
            try
            {
                var restaurant = await _restaurantsService.GetByIdAsync(restaurantId);
                if (restaurant == null)
                    return NotFound();

                await _restaurantsService.RemoveAsync(restaurantId);

                return Ok("Restaurant was deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete restaurant with ID {0}", restaurantId);
                return StatusCode(500, $"Failed to delete restaurant with ID {restaurantId}");
            }
        }
        
    }
}

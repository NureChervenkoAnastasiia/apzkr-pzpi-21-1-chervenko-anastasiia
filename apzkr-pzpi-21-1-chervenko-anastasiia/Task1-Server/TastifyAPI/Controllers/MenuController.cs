using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TastifyAPI.DTOs;
using TastifyAPI.DTOs.Features_DTOs;
using TastifyAPI.Entities;
using TastifyAPI.Helpers;
using TastifyAPI.IServices;
using TastifyAPI.Services;

namespace TastifyAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly MenuService _menuService;
        private readonly ILogger<MenuController> _logger;
        private readonly IMapper _mapper;

        public MenuController(
            MenuService menuService, 
            ILogger<MenuController> logger, 
            IMapper mapper)
        {
            _menuService = menuService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all dishes.
        /// </summary>
        /// <remarks>
        /// This endpoint requires Worker, Guest, or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a list of MenuDto.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A list of MenuDto.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator + "," + Roles.Guest)]
        [HttpGet]
        public async Task<ActionResult<List<MenuDto>>> GetAllDishes()
        {
            try
            {
                var menuItems = await _menuService.GetAsync();
                var menuDtos = _mapper.Map<List<MenuDto>>(menuItems);
                return Ok(menuDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all Menus");
                return StatusCode(500, "Failed to get all Menus");
            }
        }

        /// <summary>
        /// Get dish by ID.
        /// </summary>
        /// <param name="menuId">The ID of the dish.</param>
        /// <remarks>
        /// This endpoint requires Worker, Guest, or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a MenuDto.
        /// If the dish is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A MenuDto.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator + "," + Roles.Guest)]
        [HttpGet("{menuId:length(24)}")]
        public async Task<ActionResult<MenuDto>> GetDishById(string menuId)
        {
            try
            {
                var menu = await _menuService.GetByIdAsync(menuId);
                if (menu == null)
                    return NotFound();

                var menuDto = _mapper.Map<MenuDto>(menu);
                return Ok(menuDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Menu with ID {0}", menuId);
                return StatusCode(500, $"Failed to get Menu with ID {menuId}");
            }
        }

        /// <summary>
        /// Get the menu for a specific restaurant by its ID.
        /// </summary>
        /// <param name="restaurantId">The ID of the restaurant.</param>
        /// <remarks>
        /// This endpoint requires Worker, Guest, or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a list of MenuDto.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A list of MenuDto representing the menu for the restaurant.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator + "," + Roles.Guest)]
        [HttpGet("restaurant/{restaurantId}/menu")]
        public async Task<ActionResult<List<MenuDto>>> GetRestaurantMenu(string restaurantId)
        {
            return await GetMenuByType(restaurantId, null);
        }

        /// <summary>
        /// Get the first dishes for a specific restaurant.
        /// </summary>
        /// <param name="restaurantId">The ID of the restaurant.</param>
        /// <remarks>
        /// This endpoint requires Worker, Guest, or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a list of MenuDto.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A list of MenuDto representing the first dishes for the restaurant.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator + "," + Roles.Guest)]
        [HttpGet("restaurant/{restaurantId}/first-dishes")]
        public async Task<ActionResult<List<MenuDto>>> GetFirstDishesForRestaurant(string restaurantId)
        {
            return await GetMenuByType(restaurantId, "FirstDish");
        }

        /// <summary>
        /// Get the second dishes for a specific restaurant.
        /// </summary>
        /// <param name="restaurantId">The ID of the restaurant.</param>
        /// <remarks>
        /// This endpoint requires Worker, Guest, or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a list of MenuDto.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A list of MenuDto representing the second dishes for the restaurant.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator + "," + Roles.Guest)]
        [HttpGet("restaurant/{restaurantId}/second-dishes")]
        public async Task<ActionResult<List<MenuDto>>> GetSecondDishesForRestaurant(string restaurantId)
        {
            return await GetMenuByType(restaurantId, "SecondDish");
        }

        /// <summary>
        /// Get the drinks for a specific restaurant.
        /// </summary>
        /// <param name="restaurantId">The ID of the restaurant.</param>
        /// <remarks>
        /// This endpoint requires Worker, Guest, or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a list of MenuDto.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A list of MenuDto representing the drinks for the restaurant.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator + "," + Roles.Guest)]
        [HttpGet("restaurant/{restaurantId}/drinks")]
        public async Task<ActionResult<List<MenuDto>>> GetDrinksForRestaurant(string restaurantId)
        {
            return await GetMenuByType(restaurantId, "Drink");
        }

        /// <summary>
        /// Get menu items by restaurant ID and type.
        /// </summary>
        /// <param name="restaurantId">The ID of the restaurant.</param>
        /// <param name="type">The type of menu items (optional).</param>
        /// <remarks>
        /// This endpoint requires Worker, Guest, or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a list of MenuDto.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A list of MenuDto representing the menu items for the restaurant.
        /// </returns>
        private async Task<ActionResult<List<MenuDto>>> GetMenuByType(string restaurantId, string? type)
        {
            try
            {
                var menuItems = type switch
                {
                    null => await _menuService.GetRestaurantMenuAsync(restaurantId),
                    "FirstDish" => await _menuService.GetFirstDishesForRestaurantAsync(restaurantId),
                    "SecondDish" => await _menuService.GetSecondDishesForRestaurantAsync(restaurantId),
                    "Drink" => await _menuService.GetDrinksForRestaurantAsync(restaurantId),
                    _ => throw new ArgumentException("Invalid menu type")
                };

                var menuDtos = _mapper.Map<List<MenuDto>>(menuItems);
                return Ok(menuDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get menu position by type {0}", type);
                return StatusCode(500, $"Failed to get menu position by type {type}");
            }
        }

        /// <summary>
        /// Get a list of dishes and statistics on their orders in a restaurant.
        /// </summary>
        /// <param name="restaurantId">The ID of the restaurant.</param>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a list of DishPopularityDto.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A list of DishPopularityDto with dishes names and orders amount.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpGet("restaurant/{restaurantId}/dishes-rating")]
        public async Task<ActionResult<List<DishPopularityDto>>> GetMostPopularDishes(string restaurantId)
        {
            try
            {
                var popularDishes = await _menuService.GetMostPopularDishesAsync(restaurantId);
                return Ok(popularDishes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all Second Dishes for Restaurant {0}", restaurantId);
                return StatusCode(500, $"Failed to get all Second Dishes for Restaurant {restaurantId}");
            }
        }

        /// <summary>
        /// Create a new dish.
        /// </summary>
        /// <param name="menuDto">The dish data to create.</param>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 201 Created containing the created dish's MenuDto.
        /// If the dish data is invalid, it will return a BadRequest response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// The created dish's MenuDto.
        /// </returns>
        //[Authorize(Roles = Roles.Administrator)]
        [HttpPost]
        public async Task<ActionResult<MenuDto>> CreateDish(MenuDto menuDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var menu = _mapper.Map<Menu>(menuDto);
                await _menuService.CreateAsync(menu);

                var createdMenuDto = _mapper.Map<MenuDto>(menu);
                return CreatedAtAction(nameof(GetDishById), new { menuId = createdMenuDto.Id }, createdMenuDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new Menu");
                return StatusCode(500, "Failed to create new Menu");
            }
        }

        /// <summary>
        /// Update a dish by ID.
        /// </summary>
        /// <param name="menuId">The ID of the dish to update.</param>
        /// <param name="menuDto">The updated dish data.</param>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a success message.
        /// If the dish is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// HTTP 200 OK with a success message.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpPut("{menuId:length(24)}")]
        public async Task<IActionResult> UpdateDish(string menuId, MenuDto menuDto)
        {
            try
            {
                var existingMenu = await _menuService.GetByIdAsync(menuId);
                if (existingMenu == null)
                    return NotFound();

                menuDto.Id = menuId;
                _mapper.Map(menuDto, existingMenu);

                await _menuService.UpdateAsync(menuId, existingMenu);

                return Ok("Dish or drink was updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update Menu with ID {0}", menuId);
                return StatusCode(500, $"Failed to update Menu with ID {menuId}");
            }
        }

        /// <summary>
        /// Delete a dish by ID.
        /// </summary>
        /// <param name="menuId">The ID of the dish to delete.</param>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK with a success message.
        /// If the dish is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// HTTP 200 OK with a success message.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpDelete("{menuId:length(24)}")]
        public async Task<IActionResult> DeleteDish(string menuId)
        {
            try
            {
                var menu = await _menuService.GetByIdAsync(menuId);
                if (menu == null)
                    return NotFound();

                await _menuService.RemoveAsync(menuId);

                return Ok("Dish or drink was deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete Menu with ID {0}", menuId);
                return StatusCode(500, $"Failed to delete Menu with ID {menuId}");
            }
        }
    }
}

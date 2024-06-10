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
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly ILogger<OrderController> _logger;
        private readonly IMapper _mapper;

        public OrderController(
            OrderService ordersService, 
            ILogger<OrderController> logger, 
            IMapper mapper)
        {
            _orderService = ordersService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all orders.
        /// </summary>
        /// <remarks>
        /// This endpoint requires Worker or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a list of OrderDto.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A list of OrderDto.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAsync();
                var orderDtos = _mapper.Map<List<OrderDto>>(orders);
                return Ok(orderDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all orders");
                return StatusCode(500, "Failed to get all orders");
            }
        }

        /// <summary>
        /// Get an order by ID.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <remarks>
        /// This endpoint requires Worker, Guest, or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing an OrderDto.
        /// If the order is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// An OrderDto.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator + "," + Roles.Guest)]
        [HttpGet("{orderId:length(24)}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(string orderId)
        {
            try
            {
                var order = await _orderService.GetByIdAsync(orderId);
                if (order == null)
                    return NotFound();

                var orderDto = _mapper.Map<OrderDto>(order);
                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get order with ID {0}", orderId);
                return StatusCode(500, $"Failed to get order with ID {orderId}");
            }
        }

        /// <summary>
        /// Create a new order.
        /// </summary>
        /// <param name="orderDto">The order data.</param>
        /// <remarks>
        /// This endpoint requires Worker, Guest, or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 201 Created containing the created OrderDto.
        /// If the ModelState is invalid, it will return a BadRequest response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// The created OrderDto.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator + "," + Roles.Guest)]
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(OrderDto orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var order = _mapper.Map<Order>(orderDto);
                await _orderService.CreateAsync(order);

                var createdOrderDto = _mapper.Map<OrderDto>(order);
                return CreatedAtAction(nameof(GetOrderById), 
                    new { orderId = createdOrderDto.Id }, createdOrderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new order");
                return StatusCode(500, "Failed to create new order");
            }
        }

        /// <summary>
        /// Update an existing order.
        /// </summary>
        /// <param name="orderId">The ID of the order to update.</param>
        /// <param name="orderDto">The updated order data.</param>
        /// <remarks>
        /// This endpoint requires Worker or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a success message.
        /// If the order is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// HTTP 200 OK with success message.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpPut("{orderId:length(24)}")]
        public async Task<IActionResult> UpdateOrder(string orderId, OrderDto orderDto)
        {
            try
            {
                var existingOrder = await _orderService.GetByIdAsync(orderId);
                if (existingOrder == null)
                    return NotFound();

                orderDto.Id = orderId;
                _mapper.Map(orderDto, existingOrder);

                await _orderService.UpdateAsync(orderId, existingOrder);

                return Ok("Order was updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update order with ID {0}", orderId);
                return StatusCode(500, $"Failed to update order with ID {orderId}");
            }
        }

        /// <summary>
        /// Delete an order.
        /// </summary>
        /// <param name="orderId">The ID of the order to delete.</param>
        /// <remarks>
        /// This endpoint requires Worker or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK with a success message.
        /// If the order is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// HTTP 200 OK with a success message.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpDelete("{orderId:length(24)}")]
        public async Task<IActionResult> DeleteOrder(string orderId)
        {
            try
            {
                var order = await _orderService.GetByIdAsync(orderId);
                if (order == null)
                    return NotFound($"Order with ID {orderId} not found");

                await _orderService.RemoveAsync(orderId);

                return Ok("Order was deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete order with ID {0}", orderId);
                return StatusCode(500, "Failed to delete order");
            }
        }
    }
}

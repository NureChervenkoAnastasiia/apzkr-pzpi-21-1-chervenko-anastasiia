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
    public class TableController : ControllerBase
    {
        private readonly TableService _tableService;
        private readonly ILogger<TableController> _logger;
        private readonly IMapper _mapper;

        public TableController(
            TableService tableService, 
            ILogger<TableController> logger, 
            IMapper mapper)
        {
            _tableService = tableService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all tables.
        /// </summary>
        /// <remarks>
        /// This endpoint requires Worker or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a list of TableDto.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A list of TableDto.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpGet]
        public async Task<ActionResult<List<TableDto>>> GetAllTables()
        {
            try
            {
                var tables = await _tableService.GetAsync();
                var tableDtos = _mapper.Map<List<TableDto>>(tables);
                return Ok(tableDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all tables");
                return StatusCode(500, "Failed to get all tables");
            }
        }

        /// <summary>
        /// Get a table by ID.
        /// </summary>
        /// <param name="tableId">The ID of the table.</param>
        /// <remarks>
        /// This endpoint requires Worker or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a TableDto.
        /// If the table is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A TableDto.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpGet("{tableId:length(24)}")]
        public async Task<ActionResult<TableDto>> GetTableById(string tableId)
        {
            try
            {
                var table = await _tableService.GetByIdAsync(tableId);
                if (table == null)
                    return NotFound();

                var tableDto = _mapper.Map<TableDto>(table);
                return Ok(tableDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get table with ID {0}", tableId);
                return StatusCode(500, $"Failed to get table with ID {tableId}");
            }
        }

        /// <summary>
        /// Create a new table.
        /// </summary>
        /// <param name="tableDto">The table data to create.</param>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the ModelState is invalid, it will return a BadRequest response.
        /// If the operation is successful, it will return an ActionResult with HTTP 201 Created containing the created TableDto.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// The created TableDto.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpPost]
        public async Task<ActionResult<TableDto>> CreateTable(TableDto tableDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var table = _mapper.Map<Table>(tableDto);
                await _tableService.CreateAsync(table);

                var createdTableDto = _mapper.Map<TableDto>(table);
                return CreatedAtAction(nameof(GetTableById), new { tableId = createdTableDto.Id }, createdTableDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new table");
                return StatusCode(500, "Failed to create new table");
            }
        }

        /// <summary>
        /// Update a table by ID.
        /// </summary>
        /// <param name="tableId">The ID of the table to update.</param>
        /// <param name="tableDto">The updated table data.</param>
        /// <remarks>
        /// This endpoint requires Worker or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK with ssuccess message.
        /// If the table is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// /// <returns>
        /// HTTP 200 OK with ssuccess message.
        /// </returns> 
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpPut("{tableId:length(24)}")]
        public async Task<IActionResult> UpdateTable(string tableId, TableDto tableDto)
        {
            try
            {
                var existingTable = await _tableService.GetByIdAsync(tableId);
                if (existingTable == null)
                    return NotFound();

                tableDto.Id = tableId;
                _mapper.Map(tableDto, existingTable);

                await _tableService.UpdateAsync(tableId, existingTable);

                return Ok("Table updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update table with ID {0}", tableId);
                return StatusCode(500, $"Failed to update table with ID {tableId}");
            }
        }

        /// <summary>
        /// Delete a table by ID.
        /// </summary>
        /// <param name="tableId">The ID of the table to delete.</param>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK with ssuccess message.
        /// If the table is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// /// <returns>
        /// HTTP 200 OK with ssuccess message.
        /// </returns> 
        [Authorize(Roles = Roles.Administrator)]
        [HttpDelete("{tableId:length(24)}")]
        public async Task<IActionResult> DeleteTable(string tableId)
        {
            try
            {
                var table = await _tableService.GetByIdAsync(tableId);
                if (table == null)
                    return NotFound($"Table with ID {tableId} not found");

                await _tableService.RemoveAsync(tableId);

                return Ok("Table deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete table with ID {0}", tableId);
                return StatusCode(500, "Failed to delete table");
            }
        }
    }
}

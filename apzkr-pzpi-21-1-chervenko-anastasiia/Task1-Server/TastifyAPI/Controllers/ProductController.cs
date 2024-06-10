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
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly ILogger<ProductController> _logger;
        private readonly IMapper _mapper;

        public ProductController(
            ProductService productsService, 
            ILogger<ProductController> logger, 
            IMapper mapper)
        {
            _productService = productsService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all products.
        /// </summary>
        /// <remarks>
        /// This endpoint requires Worker or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a list of ProductDto.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A list of ProductDto.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAsync();
                var productDtos = _mapper.Map<List<ProductDto>>(products);
                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all products");
                return StatusCode(500, "Failed to get all products");
            }
        }

        /// <summary>
        /// Get a product by ID.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <remarks>
        /// This endpoint requires Worker or Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a ProductDto.
        /// If the product is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// A ProductDto.
        /// </returns>
        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpGet("{productId:length(24)}")]
        public async Task<ActionResult<ProductDto>> GetProductById(string productId)
        {
            try
            {
                var product = await _productService.GetByIdAsync(productId);
                if (product == null)
                    return NotFound();

                var productDto = _mapper.Map<ProductDto>(product);
                return Ok(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get product with ID {0}", productId);
                return StatusCode(500, $"Failed to get product with ID {productId}");
            }
        }

        /// <summary>
        /// Create a new product.
        /// </summary>
        /// <param name="productDto">The product data.</param>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 201 Created containing the created ProductDto.
        /// If the ModelState is invalid, it will return a BadRequest response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// The created ProductDto.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var product = _mapper.Map<Product>(productDto);
                await _productService.CreateAsync(product);

                var createdProductDto = _mapper.Map<ProductDto>(product);
                return CreatedAtAction(nameof(GetProductById), 
                    new { productId = createdProductDto.Id }, createdProductDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new product");
                return StatusCode(500, "Failed to create new product");
            }
        }

        /// <summary>
        /// Update an existing product.
        /// </summary>
        /// <param name="productId">The ID of the product to update.</param>
        /// <param name="productDto">The updated product data.</param>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK containing a success message.
        /// If the product is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// HTTP 200 OK with success message.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpPut("{productId:length(24)}")]
        public async Task<IActionResult> UpdateProduct(string productId, ProductDto productDto)
        {
            try
            {
                var existingProduct = await _productService.GetByIdAsync(productId);
                if (existingProduct == null)
                    return NotFound();

                productDto.Id = productId;
                _mapper.Map(productDto, existingProduct);

                await _productService.UpdateAsync(productId, existingProduct);

                return Ok("Product was updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update product with ID {0}", productId);
                return StatusCode(500, $"Failed to update product with ID {productId}");
            }
        }

        /// <summary>
        /// Delete a product.
        /// </summary>
        /// <param name="productId">The ID of the product to delete.</param>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return an ActionResult with HTTP 200 OK with a success message.
        /// If the product is not found, it will return a NotFound response.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with an error message.
        /// </remarks>
        /// <returns>
        /// HTTP 200 OK with a success message.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpDelete("{productId:length(24)}")]
        public async Task<IActionResult> DeleteProduct(string productId)
        {
            try
            {
                var product = await _productService.GetByIdAsync(productId);
                if (product == null)
                    return NotFound($"Product with ID {productId} not found");

                await _productService.RemoveAsync(productId);

                return Ok("Product was updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete product with ID {0}", productId);
                return StatusCode(500, "Failed to delete product");
            }
        }
    }
}

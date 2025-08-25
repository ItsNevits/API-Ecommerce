using ApiEcommerce.Constants;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Models.Dtos.Responses;
using ApiEcommerce.Repository.IRepository;
using ApiEcommerce.Services;
using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersionNeutral]
    [ApiController]
    [Authorize(Roles = RolesName.Admin)]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFileService _fileService;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository, IFileService fileService)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _fileService = fileService;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetProducts()
        {
            var products = _productRepository.GetProducts();
            var productsDto = products.Adapt<IEnumerable<ProductDto>>();
            return Ok(productsDto);
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetProduct(Guid id)
        {
            var product = _productRepository.GetProduct(id);
            if (product == null) return NotFound("Product not found");

            var productDto = product.Adapt<ProductDto>();
            return Ok(productDto);
        }

        [AllowAnonymous]
        [HttpGet("Paged", Name = "GetProductInPage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetProductInPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            if (pageNumber <= 0 || pageSize <= 0) return BadRequest("Page number and page size must be greater than zero");

            var totalProduct = _productRepository.GetTotalProducts();
            if (totalProduct == 0) return NotFound("No products found");
            var totalPages = (int)Math.Ceiling((double)totalProduct / pageSize);
            if (pageNumber > totalPages) return BadRequest("Page number exceeds total pages");

            var products = _productRepository.GetProductsInPages(pageNumber, pageSize);
            var productDto = products.Adapt<IEnumerable<ProductDto>>();
            
            var paginationResponse = new PaginationResponse<ProductDto>
            {
                TotalItems = totalProduct,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                Items = productDto.ToList()
            };

            return Ok(paginationResponse);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto createProductDto)
        {
            if (createProductDto == null) return BadRequest(ModelState);

            if (_productRepository.ProductExists(createProductDto.Name))
            {
                ModelState.AddModelError("CustomError", "Product already exists!");
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.CategoryExists(createProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", "Category does not exist!");
                return BadRequest(ModelState);
            }

            Product product = createProductDto.Adapt<Product>();
            // Adding image
            product.ImageUrl = await _fileService.UploadFileAsync<Product>(createProductDto.ImageFile, product.ProductId, "ProductsImages", this);

            // Save product
            if (!_productRepository.CreateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when saving the record {product.Name}");
                return StatusCode(500, ModelState);
            }

            var productDto = product.Adapt<ProductDto>();
            return CreatedAtRoute("GetProduct", new { id = product.ProductId }, productDto);
        }

        [HttpGet("search/categoy/{id:guid}", Name = "GetProductsInCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetProductsInCategory(Guid id)
        {
            if (id == Guid.Empty) return BadRequest("Invalid category ID");

            if (!_categoryRepository.CategoryExists(id))
            {
                return NotFound("Category not found");
            }
            var products = _productRepository.GetProductsInCategory(id);
            var productsDto = products.Adapt<IEnumerable<ProductDto>>();
            return Ok(productsDto);
        }

        [AllowAnonymous]
        [HttpGet("search/{searchText}", Name = "SearchProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult SearchProducts(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText)) return BadRequest("Invalid product name o description");

            var products = _productRepository.SearchProducts(searchText);
            if (products.Count == 0) return NotFound("No products found matching the search criteria");

            var productsDto = products.Adapt<IEnumerable<ProductDto>>();
            return Ok(productsDto);
        }

        [HttpPatch("buy/{id:guid}/{quantity:int}", Name = "BuyProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BuyProduct(Guid id, int quantity)
        {
            if (id == Guid.Empty) return BadRequest("Invalid product ID");
            if (quantity <= 0) return BadRequest("Quantity must be greater than zero");

            if (!_productRepository.ProductExists(id))
            {
                return NotFound("Product not found");
            }

            if (!_productRepository.BuyProduct(id, quantity))
            {
                ModelState.AddModelError("CustomError", "Could not complete the purchase. Please try again.");
                return BadRequest(ModelState);
            }

            return Ok($"Successfully purchased {quantity}");
        }

        [HttpPut("{id:guid}", Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] UpdateProductDto updateProductDto)
        {
            if (id == Guid.Empty) return BadRequest("Invalid product ID");
            if (updateProductDto == null) return BadRequest(ModelState);

            if (!_productRepository.ProductExists(id))
            {
                return NotFound("Product not found");
            }

            if (!_categoryRepository.CategoryExists(updateProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", "Category does not exist!");
                return BadRequest(ModelState);
            }

            // Obtener el producto existente de la base de datos
            var existingProduct = _productRepository.GetProduct(id);
            if (existingProduct == null)
            {
                return NotFound("Product not found");
            }

            // Mapear las propiedades del DTO al producto existente
            updateProductDto.Adapt(existingProduct);
            existingProduct.ProductId = id;

            // Adding image
            existingProduct.ImageUrl = await _fileService.UploadFileAsync<Product>(updateProductDto.ImageFile, existingProduct.ProductId, "ProductsImages", this);

            if (!_productRepository.UpdateProduct(existingProduct))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when updating the record {existingProduct.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{id:guid}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteProduct(Guid id)
        {
            if (id == Guid.Empty) return BadRequest("Invalid product ID");

            var product = _productRepository.GetProduct(id);
            if (product == null) return NotFound("Product not found");

            if (!_productRepository.DeleteProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when deleting the record {product.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
